using System.IO;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;

namespace FTP.Core.Protocol.Commands
{
    // Xử lý lệnh CWD (Change Working Directory) - thay đổi thư mục làm việc hiện tại
    public class CwdCommand : IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            if (!session.IsAuthenticated)
            {
                await session.SendResponseAsync(530, "Not logged in");
                return;
            }
            if (string.IsNullOrWhiteSpace(arguments))
            {
                await session.SendResponseAsync(501, "Syntax error: CWD <directory>");
                return;
            }

            string targetPath = arguments.Trim();

            // Xử lý đường dẫn đặc biệt
            if (targetPath == "..")
            {
                // Lên thư mục cha
                if (session.CurrentDirectory != "/")
                {
                    int lastSlash = session.CurrentDirectory.TrimEnd('/').LastIndexOf('/');
                    session.CurrentDirectory = lastSlash <= 0 ? "/" : session.CurrentDirectory.Substring(0, lastSlash);
                }
                await session.SendResponseAsync(250, "Directory changed");
                return;
            }

            if (targetPath == ".")
            {
                // Giữ nguyên thư mục hiện tại
                await session.SendResponseAsync(250, "Directory unchanged");
                return;
            }

            // Chuyển đổi sang physical path
            string physicalPath = session.GetPhysicalPath(targetPath);

            if (physicalPath == null || !Directory.Exists(physicalPath))
            {
                await session.SendResponseAsync(550, "Directory not found");
                return;
            }

            // Cập nhật CurrentDirectory
            if (targetPath.StartsWith("/"))
            {
                session.CurrentDirectory = targetPath;
            }
            else
            {
                // Relative path
                session.CurrentDirectory = session.CurrentDirectory.TrimEnd('/') + "/" + targetPath;
            }

            await session.SendResponseAsync(250, "Directory changed");
        }
    }
}


