using System;
using System.IO;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;

namespace FTP.Core.Protocol.Commands
{
    //Xử lý lệnh RMD (xóa thư mục)
    public class RmdCommand:IFtpCommand
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
                await session.SendResponseAsync(501, "Syntax error: RMD <directory-name>");
                return;
            }

            string dirName = arguments.Trim();
            string physicalPath = null;

            try
            {
                physicalPath = session.GetPhysicalPath(dirName);

                if (physicalPath == null || !Directory.Exists(physicalPath))
                {
                    await session.SendResponseAsync(550, "Directory not found");
                    return;
                }

                // Kiểm tra xem thư mục có rỗng không
                if (Directory.GetFileSystemEntries(physicalPath).Length > 0)
                {
                    await session.SendResponseAsync(550, "Directory not empty");
                    return;
                }

                // Thực hiện xóa thư mục
                Directory.Delete(physicalPath);

                await session.SendResponseAsync(250, "Directory deleted successfully");
            }
            catch (UnauthorizedAccessException)
            {
                await session.SendResponseAsync(550, "Permission denied");
            }
            catch (IOException ex)
            {
                await session.SendResponseAsync(550, $"Error deleting directory: {ex.Message}");
            }
            catch (Exception ex)
            {
                await session.SendResponseAsync(451, "Error in processing");
            }
        }
    }
}
