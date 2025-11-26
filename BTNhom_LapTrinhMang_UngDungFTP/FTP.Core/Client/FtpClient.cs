using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FTP.Core.Models;
using FTP.Core.Helpers;
using FTP.Core.Enum;

namespace FTP.Core.Client
{
    // FTP Client chính - quản lý toàn bộ operations với FTP server
    public class FtpClient : IDisposable
    {
        private FtpConnection _connection;
        private string _currentDirectory = "/";
        private ClientStatus _status = ClientStatus.Disconnected;
        // Trạng thái hiện tại của client
        public ClientStatus Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                OnStatusChanged(value);
            }
        }
        // Thư mục hiện tại trên server
        public string CurrentDirectory
        {
            get { return _currentDirectory; }
            private set { _currentDirectory = value; }
        }
        // Kiểm tra có đang kết nối không
        public bool IsConnected => _connection?.IsConnected ?? false;
        // Thông tin kết nối hiện tại
        public ConnectionInfo ConnectionInfo => _connection?.ConnectionInfo;
        // Events cho UI subscribe
        public event EventHandler<ClientStatus> StatusChanged;
        public event EventHandler<string> MessageReceived;
        public event EventHandler<string> CommandSent;
        public event EventHandler<string> ResponseReceived;
        public event EventHandler<TransferProgressEventArgs> TransferProgress;
        // Kết nối và đăng nhập vào FTP server
        public async Task<bool> ConnectAsync(ConnectionInfo connectionInfo)
        {
            try
            {
                if (connectionInfo == null || !connectionInfo.IsValid())
                    throw new ArgumentException("Invalid connection info");

                Status = ClientStatus.Idle;
                OnMessage($"Connecting to {connectionInfo.Host}:{connectionInfo.Port}...");

                // Tạo connection
                _connection = new FtpConnection();
                _connection.CommandSent += (s, cmd) => OnCommandSent(cmd);
                _connection.ResponseReceived += (s, resp) => OnResponseReceived(resp);

                // Kết nối
                await _connection.ConnectAsync(connectionInfo);
                OnMessage("Connected to server");

                // Authenticate
                bool loginSuccess = await AuthenticateAsync(
                    connectionInfo.UserName,
                    connectionInfo.PassWord);

                if (loginSuccess)
                {
                    Status = ClientStatus.Connected;

                    // Lấy current directory
                    _currentDirectory = await GetCurrentDirectoryAsync();
                    OnMessage($"Login successful. Current directory: {_currentDirectory}");

                    return true;
                }
                else
                {
                    await DisconnectAsync();
                    OnMessage("Login failed");
                    return false;
                }
            }
            catch (Exception ex)
            {
                OnMessage($"Connection error: {ex.Message}");
                Status = ClientStatus.Disconnected;
                return false;
            }
        }
        // Xác thực với server (USER + PASS)
        private async Task<bool> AuthenticateAsync(string username, string password)
        {
            // Gửi USER command
            await _connection.SendCommandAsync($"USER {username}");
            string userResponse = await _connection.ReadResponseAsync();

            if (!userResponse.StartsWith("331")) // 331 = Need password
                return false;

            // Gửi PASS command
            await _connection.SendCommandAsync($"PASS {password}");
            string passResponse = await _connection.ReadResponseAsync();

            return passResponse.StartsWith("230"); // 230 = Login successful
        }
        // Ngắt kết nối khỏi server
        public async Task DisconnectAsync()
        {
            if (_connection != null)
            {
                await _connection.DisconnectAsync();
                _connection.Dispose();
                _connection = null;
            }

            Status = ClientStatus.Disconnected;
            OnMessage("Disconnected from server");
        }
        // Lấy danh sách file/folder trong thư mục hiện tại hoặc thư mục chỉ định
        public async Task<List<FtpFileItem>> ListFilesAsync(string remotePath = "")
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            try
            {
                // Nếu không chỉ định path, dùng current directory
                string targetPath = string.IsNullOrWhiteSpace(remotePath)
                    ? _currentDirectory
                    : remotePath;

                OnMessage($"Listing directory: {targetPath}");

                // Setup data connection (PASV hoặc PORT)
                var (dataIp, dataPort) = await SetupDataConnectionAsync();

                // Gửi LIST command
                await _connection.SendCommandAsync($"LIST {targetPath}");
                string response = await _connection.ReadResponseAsync();

                if (!response.StartsWith("150")) // 150 = Opening data connection
                    throw new Exception($"LIST failed: {response}");

                // Kết nối đến data port và đọc dữ liệu
                List<FtpFileItem> files = new List<FtpFileItem>();

                using (TcpClient dataClient = new TcpClient(dataIp, dataPort))
                using (NetworkStream dataStream = dataClient.GetStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        FtpFileItem item = FtpResponseParser.ParseListLine(line);
                        if (item != null)
                        {
                            // Set full path
                            item.FullPath = PathHelper.Combine(targetPath, item.Name);
                            files.Add(item);
                        }
                    }
                }

                // Đọc response kết thúc (226 Transfer complete)
                string endResponse = await _connection.ReadResponseAsync();
                OnMessage($"Listed {files.Count} items");

                return files;
            }
            catch (Exception ex)
            {
                OnMessage($"List error: {ex.Message}");
                throw;
            }
        }
        // Thay đổi thư mục làm việc (CWD command)
        public async Task<bool> ChangeDirectoryAsync(string remotePath)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            try
            {
                await _connection.SendCommandAsync($"CWD {remotePath}");
                string response = await _connection.ReadResponseAsync();

                if (response.StartsWith("250")) // 250 = Directory changed
                {
                    _currentDirectory = await GetCurrentDirectoryAsync();
                    OnMessage($"Changed directory to: {_currentDirectory}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                OnMessage($"Change directory error: {ex.Message}");
                return false;
            }
        }
        // Lấy thư mục làm việc hiện tại (PWD command)
        public async Task<string> GetCurrentDirectoryAsync()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            await _connection.SendCommandAsync("PWD");
            string response = await _connection.ReadResponseAsync();

            return FtpResponseParser.ParsePwdResponse(response);
        }
        // Tạo thư mục mới trên server (MKD command)
        public async Task<bool> CreateDirectoryAsync(string remotePath)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            try
            {
                await _connection.SendCommandAsync($"MKD {remotePath}");
                string response = await _connection.ReadResponseAsync();

                if (response.StartsWith("257")) // 257 = Directory created
                {
                    OnMessage($"Created directory: {remotePath}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                OnMessage($"Create directory error: {ex.Message}");
                return false;
            }
        }
        // Xóa thư mục trên server (RMD command)
        public async Task<bool> RemoveDirectoryAsync(string remotePath)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            try
            {
                await _connection.SendCommandAsync($"RMD {remotePath}");
                string response = await _connection.ReadResponseAsync();

                if (response.StartsWith("250")) // 250 = Directory removed
                {
                    OnMessage($"Removed directory: {remotePath}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                OnMessage($"Remove directory error: {ex.Message}");
                return false;
            }
        }
        // Xóa file trên server (DELE command)
        public async Task<bool> DeleteFileAsync(string remotePath)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            try
            {
                await _connection.SendCommandAsync($"DELE {remotePath}");
                string response = await _connection.ReadResponseAsync();

                if (response.StartsWith("250")) // 250 = File deleted
                {
                    OnMessage($"Deleted file: {remotePath}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                OnMessage($"Delete file error: {ex.Message}");
                return false;
            }
        }
        // Download file từ server về local (RETR command)
        public async Task<bool> DownloadFileAsync(string remotePath, string localPath,
            IProgress<TransferProgressEventArgs> progress = null)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            try
            {
                Status = ClientStatus.Downloading;
                OnMessage($"Downloading {remotePath} to {localPath}...");

                // Setup data connection
                var (dataIp, dataPort) = await SetupDataConnectionAsync();

                // Gửi RETR command
                await _connection.SendCommandAsync($"RETR {remotePath}");
                string response = await _connection.ReadResponseAsync();

                if (!response.StartsWith("150")) // 150 = Opening data connection
                {
                    Status = ClientStatus.Connected;
                    throw new Exception($"Download failed: {response}");
                }

                // Kết nối data connection và download
                using (TcpClient dataClient = new TcpClient(dataIp, dataPort))
                using (NetworkStream dataStream = dataClient.GetStream())
                using (FileStream fileStream = new FileStream(localPath, FileMode.Create))
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    long totalBytes = 0;

                    while ((bytesRead = await dataStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalBytes += bytesRead;

                        // Report progress
                        if (progress != null)
                        {
                            var args = new TransferProgressEventArgs
                            {
                                BytesTransferred = totalBytes,
                                FileName = Path.GetFileName(remotePath)
                            };
                            progress.Report(args);
                        }
                    }

                    OnMessage($"Downloaded {totalBytes} bytes");
                }

                // Đọc response kết thúc
                string endResponse = await _connection.ReadResponseAsync();
                Status = ClientStatus.Connected;

                return endResponse.StartsWith("226"); // 226 = Transfer complete
            }
            catch (Exception ex)
            {
                Status = ClientStatus.Connected;
                OnMessage($"Download error: {ex.Message}");
                throw;
            }
        }
        // Upload file từ local lên server (STOR command)
        public async Task<bool> UploadFileAsync(string localPath, string remotePath,
            IProgress<TransferProgressEventArgs> progress = null)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            if (!File.Exists(localPath))
                throw new FileNotFoundException("Local file not found", localPath);

            try
            {
                Status = ClientStatus.Uploading;
                OnMessage($"Uploading {localPath} to {remotePath}...");

                // Setup data connection
                var (dataIp, dataPort) = await SetupDataConnectionAsync();

                // Gửi STOR command
                await _connection.SendCommandAsync($"STOR {remotePath}");
                string response = await _connection.ReadResponseAsync();

                if (!response.StartsWith("150")) // 150 = Opening data connection
                {
                    Status = ClientStatus.Connected;
                    throw new Exception($"Upload failed: {response}");
                }

                // Kết nối data connection và upload
                using (TcpClient dataClient = new TcpClient(dataIp, dataPort))
                using (NetworkStream dataStream = dataClient.GetStream())
                using (FileStream fileStream = new FileStream(localPath, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    long totalBytes = 0;

                    while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await dataStream.WriteAsync(buffer, 0, bytesRead);
                        totalBytes += bytesRead;

                        // Report progress
                        if (progress != null)
                        {
                            var args = new TransferProgressEventArgs
                            {
                                BytesTransferred = totalBytes,
                                TotalBytes = fileStream.Length,
                                FileName = Path.GetFileName(localPath)
                            };
                            progress.Report(args);
                        }
                    }

                    OnMessage($"Uploaded {totalBytes} bytes");
                }

                // Đọc response kết thúc
                string endResponse = await _connection.ReadResponseAsync();
                Status = ClientStatus.Connected;

                return endResponse.StartsWith("226"); // 226 = Transfer complete
            }
            catch (Exception ex)
            {
                Status = ClientStatus.Connected;
                OnMessage($"Upload error: {ex.Message}");
                throw;
            }
        }
        // Setup data connection (PASV mode)
        private async Task<(string ip, int port)> SetupDataConnectionAsync()
        {
            // Gửi PASV command
            await _connection.SendCommandAsync("PASV");
            string response = await _connection.ReadResponseAsync();

            if (!response.StartsWith("227")) // 227 = Entering Passive Mode
                throw new Exception($"PASV failed: {response}");

            // Parse PASV response để lấy IP và port
            return FtpResponseParser.ParsePasvResponse(response);
        }

        // Event triggers
        protected virtual void OnStatusChanged(ClientStatus status)
        {
            StatusChanged?.Invoke(this, status);
        }

        protected virtual void OnMessage(string message)
        {
            MessageReceived?.Invoke(this, message);
        }

        protected virtual void OnCommandSent(string command)
        {
            CommandSent?.Invoke(this, command);
        }

        protected virtual void OnResponseReceived(string response)
        {
            ResponseReceived?.Invoke(this, response);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }

    // Event arguments cho transfer progress
    public class TransferProgressEventArgs : EventArgs
    {
        public long BytesTransferred { get; set; }
        public long TotalBytes { get; set; }
        public string FileName { get; set; }

        public int ProgressPercentage
        {
            get
            {
                if (TotalBytes == 0) return 0;
                return (int)((BytesTransferred * 100) / TotalBytes);
            }
        }
    }
}
