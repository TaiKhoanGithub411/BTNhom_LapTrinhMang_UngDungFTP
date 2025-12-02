using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;

namespace FTP.Core.Protocol.Commands
{
    public class PasvCommand : IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            try
            {
                // Đóng passive listener cũ nếu có
                session.PassiveListener?.Stop();

                // Tạo TcpListener mới trên port ngẫu nhiên
                Random rand = new Random();
                int port = rand.Next(50000, 51001);
                session.PassiveListener = new TcpListener(IPAddress.Any, port);
                session.PassiveListener.Start();
                session.PassivePort = port;

                // ===== SỬA CHÍNH Ở ĐÂY =====
                // Lấy IP từ control connection (IP mà Client đang kết nối đến)
                string serverIP = GetServerIPFromControlConnection(session);
                string[] ipParts = serverIP.Split('.');

                // Tính p1 và p2
                int p1 = port / 256;
                int p2 = port % 256;

                string pasvResponse = $"Entering Passive Mode ({ipParts[0]},{ipParts[1]},{ipParts[2]},{ipParts[3]},{p1},{p2})";
                await session.SendResponseAsync(227, pasvResponse);
            }
            catch (Exception ex)
            {
                await session.SendResponseAsync(425, $"Can't open passive connection: {ex.Message}");
            }
        }

        // ===== HÀM MỚI - LẤY IP TỪ CONTROL CONNECTION =====
        private static string GetServerIPFromControlConnection(ClientSession session)
        {
            try
            {
                // Lấy IP từ LocalEndPoint của control connection
                var controlClient = session.GetType()
                    .GetField("_controlClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(session) as TcpClient;

                if (controlClient?.Client?.LocalEndPoint is IPEndPoint localEndPoint)
                {
                    string ip = localEndPoint.Address.ToString();
                    // Nếu là IPv6 mapped IPv4 (::ffff:192.168.x.x), chuyển sang IPv4
                    if (localEndPoint.Address.IsIPv4MappedToIPv6)
                    {
                        ip = localEndPoint.Address.MapToIPv4().ToString();
                    }
                    return ip;
                }
            }
            catch { }

            // Fallback: Thử tìm IP LAN
            return GetLocalIPv4Fallback();
        }

        private static string GetLocalIPv4Fallback()
        {
            try
            {
                // Tạo kết nối giả đến 8.8.8.8 để tìm IP interface đang dùng
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    if (endPoint != null)
                    {
                        return endPoint.Address.ToString();
                    }
                }
            }
            catch { }

            // Fallback cuối cùng
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !ip.ToString().StartsWith("127."))
                    return ip.ToString();
            }

            return "127.0.0.1";
        }
    }
}