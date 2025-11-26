using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FTP.Core.Models;

namespace FTP.Core.Client
{
    // Quản lý Control Connection với FTP server - Xử lý việc gửi/nhận FTP commands qua port 21
    public class FtpConnection : IDisposable
    {
        private TcpClient _controlClient;
        private NetworkStream _controlStream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private ConnectionInfo _connectionInfo;
        public bool IsConnected => _controlClient?.Connected ?? false;//Kiểm tra có đang kết nối hay không
        public ConnectionInfo ConnectionInfo => _connectionInfo;//Thông tin kết nối hiện tại
        public event EventHandler<string> ResponseReceived;// Event khi nhận được response từ server (dùng để log)
        public event EventHandler<string> CommandSent;// Event khi gửi command đến server (dùng để log)
        public async Task<bool> ConnectAsync(ConnectionInfo connectionInfo)// Kết nối đến FTP server
        {
            try
            {
                _connectionInfo = connectionInfo ?? throw new ArgumentNullException(nameof(connectionInfo));

                // Tạo TCP connection đến FTP server
                _controlClient = new TcpClient();
                await _controlClient.ConnectAsync(connectionInfo.Host, connectionInfo.Port);

                // Setup streams để đọc/ghi
                _controlStream = _controlClient.GetStream();
                _reader = new StreamReader(_controlStream, Encoding.UTF8);
                _writer = new StreamWriter(_controlStream, Encoding.UTF8) { AutoFlush = true };

                // Đọc welcome message từ server (220)
                string welcomeMsg = await ReadResponseAsync();

                return IsConnected;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to connect to {connectionInfo.Host}:{connectionInfo.Port} - {ex.Message}", ex);
            }
        }
        public async Task SendCommandAsync(string command)// Gửi command đến server qua control connection
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            await _writer.WriteLineAsync(command);
            CommandSent?.Invoke(this, command);
        }
        public async Task<string> ReadResponseAsync()// Đọc một dòng response từ server
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            string response = await _reader.ReadLineAsync();
            ResponseReceived?.Invoke(this, response);

            return response ?? string.Empty;
        }
        public async Task<string> ReadMultiLineResponseAsync()// Đọc response nhiều dòng (dùng cho một số commands)
        {
            StringBuilder sb = new StringBuilder();
            string line;

            // Đọc cho đến khi gặp dòng kết thúc (code + space)
            while ((line = await _reader.ReadLineAsync()) != null)
            {
                sb.AppendLine(line);
                ResponseReceived?.Invoke(this, line);

                // Dòng cuối có format: "XXX message" (space ở vị trí 3)
                if (line.Length >= 4 && line[3] == ' ')
                    break;
            }

            return sb.ToString();
        }
        public async Task DisconnectAsync()// Đóng kết nối (gửi QUIT trước khi disconnect)
        {
            if (IsConnected)
            {
                try
                {
                    // Gửi QUIT command
                    await SendCommandAsync("QUIT");
                    await ReadResponseAsync();
                }
                catch
                {
                    // Ignore errors when disconnecting
                }
                finally
                {
                    Dispose();
                }
            }
        }
        public void Dispose()// Dispose resources
        {
            _reader?.Dispose();
            _writer?.Dispose();
            _controlStream?.Dispose();
            _controlClient?.Dispose();
        }
    }
}
