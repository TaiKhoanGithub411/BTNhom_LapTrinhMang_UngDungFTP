using FTP.Core.Enum;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FTP.Core.Protocol;
using FTP.Core.Constants;
using FTP.Core.Authentication;
namespace FTP.Core.Server
{
    //Đại diện cho một phiên kết nối của client - Control connection và xử lý FTP command
    public class ClientSession
    {
        private readonly TcpClient _controlClient;
        private readonly ServerConfiguration _configuration;
        public ServerConfiguration Configuration => _configuration;
        private NetworkStream _controlStream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private readonly FtpCommandFactory _commandFactory;
        private TcpClient _dataClient;
        private NetworkStream _dataStream;
        public PathMapper PathMapper { get; internal set; }
        public User CurrentUser { get; set; }
        public string ActiveModeIP { get; set; }
        public int ActiveModePort { get; set; }
        public string SessionId { get; internal set; }//ID duy nhất cho session.
        public string ClientIPAddress { get; private set; }
        public string Username { get; set; }
        public DateTime ConnectedTime { get; private set; }
        public ClientStatus Status { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsAuthenticated { get; set; }//Client đã xác thực chưa.
        public string CurrentDirectory { get; set; }
        public event Action<string> LogMessage;//Event được phát ra khi có log message.
        public TcpListener PassiveListener { get; set; } // Listener cho passive mode data connection.
        public int PassivePort { get; set; }// Port được sử dụng cho passive mode.
        public ClientSession(TcpClient controlClient, ServerConfiguration configuration)// Khởi tạo ClientSession.
        {
            _controlClient = controlClient ?? throw new ArgumentNullException(nameof(controlClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _commandFactory = new FtpCommandFactory();

            // Tạo SessionId
            SessionId = GenerateUniqueSessionId();

            // Lấy IP address của client
            var remoteEndPoint = (IPEndPoint)controlClient.Client.RemoteEndPoint;
            ClientIPAddress = remoteEndPoint.Address.ToString();

            // Khởi tạo thông tin session
            ConnectedTime = DateTime.Now;
            LastActivity = DateTime.Now;
            Status = ClientStatus.Connected;
            IsAuthenticated = false;
            CurrentDirectory = "/";

            // CurrentUser và PathMapper sẽ được set sau khi login
            CurrentUser = null;
            PathMapper = null;

            // Khởi tạo NetworkStream
            _controlStream = controlClient.GetStream();
            _reader = new StreamReader(_controlStream, Encoding.ASCII);
            _writer = new StreamWriter(_controlStream, Encoding.ASCII) { AutoFlush = true };
        }
        public event Action<string> SessionClosed;
        public void Close()
        {
            try
            {
                Status = ClientStatus.Disconnected;
                CloseDataConnection();
                _writer?.Close();
                _reader?.Close();
                _controlStream?.Close();
                _controlClient?.Close();
                LogMessage?.Invoke($"[{SessionId}] Session closed");
                SessionClosed?.Invoke(SessionId);
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
        public async Task StartHandlingAsync(CancellationToken cancellationToken)// Bắt đầu xử lý session (gửi welcome message và đợi commands).
        {
            try
            {
                // Gửi welcome message
                await SendResponseAsync(FtpConstants.CODE_SERVICE_READY, "FTP Server Ready");
                LogMessage?.Invoke($"Session {SessionId} started for {ClientIPAddress}");

                // ===== SỬA LẠI VÒNG LẶP XỬ LÝ COMMANDS =====

                // Vòng lặp chạy liên tục để đọc commands
                while (!cancellationToken.IsCancellationRequested && _controlClient.Connected)
                {
                    // Đọc command từ client
                    string commandLine = await _reader.ReadLineAsync();

                    // Nếu commandLine là null, client đã ngắt kết nối
                    if (commandLine == null)
                    {
                        break;
                    }

                    LastActivity = DateTime.Now;
                    LogMessage?.Invoke($"[{SessionId}] Received: {commandLine}");

                    // Parse command
                    var parts = commandLine.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    string commandName = parts.Length > 0 ? parts[0] : string.Empty;
                    string arguments = parts.Length > 1 ? parts[1] : string.Empty;

                    if (string.IsNullOrEmpty(commandName))
                    {
                        continue; // Bỏ qua dòng trống
                    }

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
                            await SendResponseAsync(FtpConstants.CODE_ACTION_ABORTED,
                                "Requested action aborted: local error in processing");
                        }
                    }
                    else
                    {
                        await SendResponseAsync(FtpConstants.CODE_COMMAND_UNRECOGNIZED,
                            "Command not recognized");
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
                LogMessage?.Invoke($"[{SessionId}] Unhandled error in session: {ex.Message}");
            }
            finally
            {
                Close();
            }
        }
        public async Task<bool> SetupDataConnectionAsync() // Thiết lập data connection (chờ client kết nối vào passive port).
        {
            try
            {
                if (PassiveListener == null)
                    return false;

                // Chờ client kết nối với timeout 30 giây
                var acceptTask = PassiveListener.AcceptTcpClientAsync();
                var timeoutTask = Task.Delay(30000);
                var completedTask = await Task.WhenAny(acceptTask, timeoutTask);
                if (completedTask == acceptTask)
                {
                    _dataClient = await acceptTask;
                    _dataStream = _dataClient.GetStream();
                    return true;
                }
                else
                {
                    LogMessage?.Invoke($"[{SessionId}] Data connection timeout");
                    return false;
                }
            }
            catch(Exception ex)
            {
                LogMessage?.Invoke($"[{SessionId}] Error setting up data connection: {ex.Message}");
                return false;
            }
        }
        public void CloseDataConnection()// Đóng data connection.
        {
            try
            {
                _dataStream?.Close();
                _dataClient?.Close();
                PassiveListener?.Stop();

                _dataStream = null;
                _dataClient = null;
                PassiveListener = null;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[{SessionId}] Error closing data connection: {ex.Message}");
            }
        }
        public async Task SendDataAsync(byte[] data)// Gửi dữ liệu qua data connection.
        {
            if(_dataStream != null && _dataStream.CanWrite)
            {
                await _dataStream.WriteAsync(data, 0, data.Length);
                await _dataStream.FlushAsync();
            }
        }
        public async Task<int> ReceiveDataAsync(byte[] buffer)// Nhận dữ liệu từ data connection.
        {
            if(_dataStream != null && _dataStream.CanRead)
            {
                return await _dataStream.ReadAsync(buffer, 0, buffer.Length);
            }
            return 0;
        }
        public string GetPhysicalPath(string virtualPath)// Lấy physical path từ virtual path (FTP path) - Sử dụng PathMapper của user
        {
            // Nếu chưa login, không cho phép
            if (PathMapper == null)
            {
                return null;
            }

            // Nếu path là relative (không bắt đầu với /), combine với CurrentDirectory
            if (!virtualPath.StartsWith("/"))
            {
                virtualPath = CurrentDirectory.TrimEnd('/') + "/" + virtualPath;
            }

            return PathMapper.GetPhysicalPath(virtualPath);
        }
        public async Task<bool> SetupActiveDataConnectionAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ActiveModeIP) || ActiveModePort == 0)
                {
                    return false;
                }

                _dataClient = new TcpClient();
                await _dataClient.ConnectAsync(ActiveModeIP, ActiveModePort);
                _dataStream = _dataClient.GetStream();
                return true;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[{SessionId}] Error connecting active mode: {ex.Message}");
                return false;
            }
        }
        private static string GenerateUniqueSessionId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
