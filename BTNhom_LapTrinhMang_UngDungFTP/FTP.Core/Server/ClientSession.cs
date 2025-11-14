using FTP.Core.Enum;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FTP.Core.Protocol;

namespace FTP.Core.Server
{
    //Đại diện cho một phiên kết nối của client - Control connection và xử lý FTP command
    public class ClientSession
    {
        private readonly TcpClient _controlClient;
        private readonly ServerConfiguration _configuration;
        private NetworkStream _controlStream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private readonly FtpCommandFactory _commandFactory;

        public string SessionId { get; private set; }//ID duy nhất cho session.
        public string ClientIPAddress { get; private set; }
        public string Username { get; set; }
        public DateTime ConnectedTime { get; private set; }
        public ClientStatus Status { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsAuthenticated { get; set; }//Client đã xác thực chưa.
        public string CurrentDirectory { get; set; }
        public event Action<string> LogMessage;//Event được phát ra khi có log message.
        public ClientSession(TcpClient controlClient, ServerConfiguration configuration)// Khởi tạo ClientSession.
        {
            _controlClient = controlClient ?? throw new ArgumentNullException(nameof(controlClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _commandFactory = new FtpCommandFactory();

            // Tạo SessionId ngắn (8 ký tự đầu của GUID)
            SessionId = Guid.NewGuid().ToString("N").Substring(0, 8);

            // Lấy IP address của client
            var remoteEndPoint = (IPEndPoint)controlClient.Client.RemoteEndPoint;
            ClientIPAddress = remoteEndPoint.Address.ToString();

            // Khởi tạo thông tin session
            ConnectedTime = DateTime.Now;
            LastActivity = DateTime.Now;
            Status = ClientStatus.Connected;
            IsAuthenticated = false;
            CurrentDirectory = "/";

            // Khởi tạo NetworkStream
            _controlStream = controlClient.GetStream();
            _reader = new StreamReader(_controlStream, Encoding.ASCII);
            _writer = new StreamWriter(_controlStream, Encoding.ASCII) { AutoFlush = true };
        }
        public void Close()//Đóng kết nối client
        {
            try
            {
                Status = ClientStatus.Disconnected;

                _writer?.Close();
                _reader?.Close();
                _controlStream?.Close();
                _controlClient?.Close();

                LogMessage?.Invoke($"[{SessionId}] Session closed");
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[{SessionId}] Error closing session: {ex.Message}");
            }
        }
        public async Task SendResponseAsync(int code, string message)//Gửi FTP response đến client.
        {
            try
            {
                string response = $"{code} {message}\r\n";
                await _writer.WriteAsync(response);
                await _writer.FlushAsync();
                LastActivity = DateTime.Now;
                LogMessage?.Invoke($"[{SessionId}] Sent: {code} {message}");
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[{SessionId}] Error sending response: {ex.Message}");
            }
        }        
        public async Task StartHandlingAsync(CancellationToken cancellationToken)//Bắt đầu xử lý session (gửi welcome message và đợi commands).
        {
            try
            {
                // Gửi welcome message (FTP 220)
                await SendResponseAsync(Constants.FtpConstants.CODE_SERVICE_READY, "FTP Server Ready");
                LogMessage?.Invoke($"Session {SessionId} started for {ClientIPAddress}");

                // Vòng lặp xử lý commands (sẽ implement đầy đủ sau)
                while (!cancellationToken.IsCancellationRequested && _controlClient.Connected)
                {
                    // Đọc command từ client
                    string commandLine = await _reader.ReadLineAsync();

                    if (string.IsNullOrEmpty(commandLine))
                    {
                        break; // Client đã ngắt kết nối
                    }

                    LastActivity = DateTime.Now;
                    LogMessage?.Invoke($"[{SessionId}] Received: {commandLine}");

                    // Parse command
                    var parts = commandLine.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    string commandName = parts[0];
                    string arguments = parts.Length > 1 ? parts[1] : string.Empty;

                    // Tạo và thực thi command
                    var command = _commandFactory.CreateCommand(commandName);

                    if (command != null)
                    {
                        try
                        {
                            await command.ExecuteAsync(this, arguments);
                        }
                        catch (Exception ex)
                        {
                            LogMessage?.Invoke($"[{SessionId}] Error executing {commandName}: {ex.Message}");
                            await SendResponseAsync(451, "Requested action aborted: local error in processing");
                        }
                    }
                    else
                    {
                        await SendResponseAsync(500, "Command not recognized");
                    }
                }
            }
            catch (IOException)
            {
                // Client ngắt kết nối đột ngột
                LogMessage?.Invoke($"[{SessionId}] Client disconnected unexpectedly");
            }
            catch (OperationCanceledException)
            {
                // Server shutdown
                LogMessage?.Invoke($"[{SessionId}] Session cancelled (server shutdown)");
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[{SessionId}] Error: {ex.Message}");
            }
            finally
            {
                Close();
            }
        }
    }
}
