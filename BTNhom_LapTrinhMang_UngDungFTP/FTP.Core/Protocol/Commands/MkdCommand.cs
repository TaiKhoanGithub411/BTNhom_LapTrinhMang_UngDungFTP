using System;
using System.IO;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;

namespace FTP.Core.Protocol.Commands
{
    //Xử lý lệnh MKD (tạo thư mục)
    public class MkdCommand:IFtpCommand
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
                await session.SendResponseAsync(501, "Syntax error: MKD <directory-name>");
                return;
            }
            string dirName = arguments.Trim();
            string physicalPath = null;
            try
            {
                physicalPath = session.GetPhysicalPath(dirName);

                if (physicalPath == null)
                {
                    await session.SendResponseAsync(550, "Invalid directory name");
                    return;
                }

                if (Directory.Exists(physicalPath))
                {
                    await session.SendResponseAsync(550, "Directory already exists");
                    return;
                }

                // Thực hiện tạo thư mục
                Directory.CreateDirectory(physicalPath);
                // Format response theo RFC 959: 257 "/path/new_dir" created.
                await session.SendResponseAsync(257, $"\"{dirName}\" created");
            }
            catch (UnauthorizedAccessException)
            {
                await session.SendResponseAsync(550, "Permission denied");
            }
            catch (IOException ex)
            {
                await session.SendResponseAsync(550, $"Error creating directory: {ex.Message}");
            }
            catch (Exception ex)
            {
                await session.SendResponseAsync(451, "Error in processing");
            }
        }
    }
}
