using System;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;

namespace FTP.Core.Protocol.Commands
{
    //Xử lý lệnh USER
    public class UserCommand : IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                await session.SendResponseAsync(501, "Syntax error: USER <username>");
                return;
            }
            // Lưu username tạm thời (chưa xác thực)
            session.Username = arguments.Trim();
            // Yêu cầu nhập password
            await session.SendResponseAsync(331, "Password required");
        }
    }
}
