using System;
using System.IO;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;
using FTP.Core.Authentication;

namespace FTP.Core.Protocol.Commands
{
    //Xử lý lệnh DELETE (xóa file)
    public class DeleCommand:IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            if (!session.IsAuthenticated)
            {
                await session.SendResponseAsync(530, "Not logged in");
                return;
            }
            if (!PermissionChecker.CanDelete(session.CurrentUser.Permissions))
            {
                await session.SendResponseAsync(550, "Permission denied");
                return;
            }
            if (string.IsNullOrWhiteSpace(arguments))
            {
                await session.SendResponseAsync(501, "Syntax error: DELE <filename>");
                return;
            }
            string fileName = arguments.Trim();
            string physicalPath = null;
            try
            {
                physicalPath = session.GetPhysicalPath(fileName);

                if (physicalPath == null || !File.Exists(physicalPath))
                {
                    await session.SendResponseAsync(550, "File not found");
                    return;
                }
                // Thực hiện xóa file
                File.Delete(physicalPath);

                await session.SendResponseAsync(250, "File deleted successfully");
            }
            catch (UnauthorizedAccessException)
            {
                await session.SendResponseAsync(550, "Permission denied");
            }
            catch (IOException ex)
            {
                await session.SendResponseAsync(550, $"Error deleting file: {ex.Message}");
            }
            catch (Exception ex)
            {
                await session.SendResponseAsync(451, $"Error in processing: {ex.Message}");
            }
        }
    }
}
