using System;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;
using FTP.Core.Authentication;
using System.IO;

namespace FTP.Core.Protocol.Commands
{
    public class PassCommand : IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            if (string.IsNullOrWhiteSpace(session.Username))
            {
                await session.SendResponseAsync(503, "Login with USER first");
                return;
            }

            if (string.IsNullOrWhiteSpace(arguments))
            {
                await session.SendResponseAsync(501, "Syntax error: PASS <password>");
                return;
            }

            string password = arguments.Trim();

            // Lấy UserManager từ configuration (cần thêm vào ServerConfiguration)
            var userManager = session.Configuration.UserManager;

            if (userManager == null)
            {
                await session.SendResponseAsync(530, "Authentication service unavailable");
                return;
            }

            // Authenticate
            User user = userManager.Authenticate(session.Username, password);

            if (user == null)
            {
                await session.SendResponseAsync(530, "Login incorrect");
                session.IsAuthenticated = false;
                return;
            }

            // Đăng nhập thành công
            session.IsAuthenticated = true;
            session.CurrentUser = user;

            // Tạo PathMapper với home directory của user
            session.PathMapper = new PathMapper(user.HomeDirectory);

            // Đảm bảo home directory tồn tại
            if (!Directory.Exists(user.HomeDirectory))
            {
                try
                {
                    Directory.CreateDirectory(user.HomeDirectory);
                }
                catch (Exception ex)
                {
                    await session.SendResponseAsync(550, $"Cannot create home directory: {ex.Message}");
                    session.IsAuthenticated = false;
                    return;
                }
            }

            await session.SendResponseAsync(230, "User logged in");

        }
    }
}
