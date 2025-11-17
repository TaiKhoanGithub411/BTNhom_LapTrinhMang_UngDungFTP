using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;

namespace FTP.Core.Protocol.Commands
{
    // Xử lý lệnh PORT (Active Mode) - Client cung cấp IP và Port để server kết nối đến.
    public class PortCommand:IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                await session.SendResponseAsync(501, "Syntax error: PORT h1,h2,h3,h4,p1,p2");
                return;
            }
            try
            {
                // Parse PORT command: PORT 192,168,1,100,20,15
                string[] parts = arguments.Split(',');
                if (parts.Length != 6)
                {
                    await session.SendResponseAsync(501, "Syntax error in PORT command");
                    return;
                }
                // Xây dựng IP address từ các phần tử của mảng
                string clientIP = $"{parts[0]}.{parts[1]}.{parts[2]}.{parts[3]}";

                // Tính port từ các phần tử của mảng
                int p1 = int.Parse(parts[4]);
                int p2 = int.Parse(parts[5]);
                int clientPort = p1 * 256 + p2;


                // Lưu thông tin để sử dụng khi transfer
                session.ActiveModeIP = clientIP;
                session.ActiveModePort = clientPort;

                await session.SendResponseAsync(200, "PORT command successful");
            }
            catch (Exception ex)
            {
                await session.SendResponseAsync(501, "Syntax error in PORT command");
            }
        }
    }
}
