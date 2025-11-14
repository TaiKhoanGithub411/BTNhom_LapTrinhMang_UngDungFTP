using System;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;
using FTP.Core.Enum;

namespace FTP.Core.Protocol.Commands
{
    //Xử lênh PASS (nhập password và xác thực)
    public class PassCommand : IFtpCommand
    {
        private bool AuthenticateUser(string username, string password)
        {
            // Demo: chấp nhận "admin/admin" và "user/user" --> Chỉ thử vì chưa làm UserManager
            //Có UserManager thì bỏ hàm này
            return (username == "admin" && password == "admin") ||
                   (username == "user" && password == "user");
        }
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            // Kiểm tra đã nhập username chưa
            if (string.IsNullOrWhiteSpace(session.Username))
            {
                await session.SendResponseAsync(503, "Bad sequence of commands. Send USER first");
                return;
            }

            if (string.IsNullOrWhiteSpace(arguments))
            {
                await session.SendResponseAsync(501, "Syntax error: PASS <password>");
                return;
            }
            string password = arguments.Trim();
            //Xác thực với UserManager (tạm thời hardcode để test)
            bool isAuthenticated = AuthenticateUser(session.Username, password);
            if (isAuthenticated)
            {
                session.IsAuthenticated = true;
                session.Status = ClientStatus.Idle;
                await session.SendResponseAsync(230, "User logged in");
            }
            else
            {
                session.Username = null;
                await session.SendResponseAsync(530, "Login incorrect");
            }
        }
    }
}
