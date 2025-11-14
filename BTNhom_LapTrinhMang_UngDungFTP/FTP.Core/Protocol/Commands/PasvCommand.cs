using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;

namespace FTP.Core.Protocol.Commands
{
    //Xử lý PASV - Tạo một data connection port để client kết nối.
    public class PasvCommand : IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            try
            {
                // Đóng passive listener cũ nếu có
                session.PassiveListener?.Stop();
                // Tạo TcpListener mới trên port ngẫu nhiên (port 0 = auto assign)
                session.PassiveListener = new TcpListener(IPAddress.Any, 0);
                session.PassiveListener.Start();
                // Lấy port được gán tự động
                int port = ((IPEndPoint)session.PassiveListener.LocalEndpoint).Port;
                session.PassivePort = port;
                // Lấy IP của server (127.0.0.1 cho local testing)
                string serverIP = "127.0.0.1";
                string[] ipParts = serverIP.Split('.');
                // Tính p1 và p2 từ port number
                // Port = p1 * 256 + p2
                int p1 = port / 256;
                int p2 = port % 256;
                // Format FTP PASV response: 227 Entering Passive Mode (h1,h2,h3,h4,p1,p2)
                string pasvResponse = $"Entering Passive Mode ({ipParts[0]},{ipParts[1]},{ipParts[2]},{ipParts[3]},{p1},{p2})";
                await session.SendResponseAsync(227, pasvResponse);
            }
            catch (Exception ex)
            {
                await session.SendResponseAsync(425, "Can't open passive connection");
            }
        }
    }
}
