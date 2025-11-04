using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using FTP.Core.Enum;

namespace FTP.Core.Server
{
    //Lớp quản lý FTP server - lắng nghe và chấp nhận kết nối từ client
    public class FtpServer
    {
        private TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ServerConfiguration _configuration;
        private readonly SessionManager _sessionManager;
        private Task _listeningTask;
        public bool IsRunning { get; private set; }//Trạng thái hoạt động của server
        public event Action<string> LogMessage;// Event để log messages ra UI.
        public event Action<ServerStatus> StatusChanged;//Cập nhật trạng thái server
        public FtpServer(ServerConfiguration configuration, SessionManager sessionManager)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
        }
        private async Task ListenForClientsAsync(CancellationToken cancellationToken)// Vòng lặp lắng nghe và chấp nhận kết nối client
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Chấp nhận kết nối client (async, không block UI thread)
                    TcpClient tcpClient = await _listener.AcceptTcpClientAsync()
                        .ConfigureAwait(false);

                    // Lấy IP của client
                    var remoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
                    string clientIP = remoteEndPoint.Address.ToString();

                    LogMessage?.Invoke($"Client connected from {clientIP}");

                    // Kiểm tra connection limit
                    if (_sessionManager.IsConnectionLimitReached(_configuration.MaxConnections))
                    {
                        LogMessage?.Invoke($"Connection limit reached. Rejecting client {clientIP}");
                        tcpClient.Close();
                        continue;
                    }

                    // Kiểm tra IP filtering (chức năng nâng cao)
                    if (!_configuration.AllowAllConnections && _configuration.BannedIPs.Contains(clientIP))
                    {
                        LogMessage?.Invoke($"Client {clientIP} is banned. Connection rejected.");
                        tcpClient.Close();
                        continue;
                    }

                    // Tạo ClientSession mới
                    var clientSession = new ClientSession(tcpClient, _configuration);

                    // Subscribe vào LogMessage event của session
                    clientSession.LogMessage += (message) => LogMessage?.Invoke(message);

                    // Thêm session vào SessionManager
                    _sessionManager.AddSession(clientSession);

                    // Bắt đầu xử lý session trong một Task riêng
                    _ = Task.Run(async () =>
                    {
                        await clientSession.StartHandlingAsync(cancellationToken);

                        // Sau khi session kết thúc, xóa khỏi SessionManager
                        _sessionManager.RemoveSession(clientSession.SessionId);
                    }, cancellationToken);
                }
            }
            catch (ObjectDisposedException)
            {
                // Listener đã bị dispose (Stop được gọi) - hành vi bình thường
                LogMessage?.Invoke("Listener stopped.");
            }
            catch (OperationCanceledException)
            {
                // Cancellation token triggered - hành vi bình thường
                LogMessage?.Invoke("Server operation cancelled.");
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error in listener loop: {ex.Message}");
            }
        }
        public async Task StartAsync()// Bắt đầu chạy server
        {
            if (IsRunning)
            {
                LogMessage?.Invoke("Server is already running.");
                return;
            }

            // Validate configuration
            if (!_configuration.IsValid())
            {
                LogMessage?.Invoke("Invalid server configuration. Please check settings.");
                return;
            }

            try
            {
                // Cập nhật trạng thái: Starting
                StatusChanged?.Invoke(ServerStatus.Starting);
                LogMessage?.Invoke("Starting server...");

                // Tạo TcpListener trên Port được cấu hình
                _listener = new TcpListener(IPAddress.Any, _configuration.Port);
                _cancellationTokenSource = new CancellationTokenSource();

                // Start listening
                _listener.Start();
                IsRunning = true;

                // Cập nhật trạng thái: Running
                StatusChanged?.Invoke(ServerStatus.Running);
                LogMessage?.Invoke($"Server started successfully on port {_configuration.Port}");
                LogMessage?.Invoke($"Root folder: {_configuration.RootFolder}");
                LogMessage?.Invoke("Waiting for client connections...");

                // Bắt đầu vòng lặp lắng nghe client
                _listeningTask = ListenForClientsAsync(_cancellationTokenSource.Token);
                await _listeningTask;
            }
            catch (SocketException ex)
            {
                LogMessage?.Invoke($"Socket error: {ex.Message}");
                LogMessage?.Invoke($"Port {_configuration.Port} may already be in use.");
                Stop();
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error starting server: {ex.Message}");
                Stop();
            }
        }
        public void Stop()//Dừng server
        {
            if (!IsRunning)
            {
                LogMessage?.Invoke("Server is not running.");
                return;
            }

            try
            {
                // Cập nhật trạng thái: Stopping
                StatusChanged?.Invoke(ServerStatus.Stopping);
                LogMessage?.Invoke("Stopping server...");

                // Hủy tất cả operations
                _cancellationTokenSource?.Cancel();

                // Dừng listener
                _listener?.Stop();

                // Đóng tất cả client sessions
                _sessionManager.CloseAllSessions();

                IsRunning = false;

                // Cập nhật trạng thái: Stopped
                StatusChanged?.Invoke(ServerStatus.Stopped);
                LogMessage?.Invoke("Server stopped successfully.");
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Error stopping server: {ex.Message}");
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }
    }
}
