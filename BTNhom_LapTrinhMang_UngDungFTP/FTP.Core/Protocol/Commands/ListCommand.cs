using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;
using FTP.Core.Enum;

namespace FTP.Core.Protocol.Commands
{
    // Xử lý lệnh LIST (liệt kê file và thư mục).
    public class ListCommand : IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            if (!session.IsAuthenticated)
            {
                await session.SendResponseAsync(530, "Not logged in");
                return;
            }

            // Kiểm tra có PASV hoặc PORT chưa
            bool hasPassive = session.PassiveListener != null;
            bool hasActive = !string.IsNullOrEmpty(session.ActiveModeIP);

            if (!hasPassive && !hasActive)
            {
                await session.SendResponseAsync(425, "Use PORT or PASV first");
                return;
            }

            try
            {
                string targetPath = string.IsNullOrWhiteSpace(arguments)
                    ? session.CurrentDirectory
                    : arguments.Trim();

                string physicalPath = session.GetPhysicalPath(targetPath);

                if (physicalPath == null || !Directory.Exists(physicalPath))
                {
                    await session.SendResponseAsync(550, "Directory not found");
                    return;
                }

                await session.SendResponseAsync(150, "Opening data connection for directory list");

                // Thiết lập data connection (Active hoặc Passive)
                bool connected = hasPassive
                    ? await session.SetupDataConnectionAsync()
                    : await session.SetupActiveDataConnectionAsync();

                if (!connected)
                {
                    await session.SendResponseAsync(425, "Can't open data connection");
                    return;
                }

                string listing = GenerateDirectoryListing(physicalPath);
                byte[] data = Encoding.ASCII.GetBytes(listing);
                await session.SendDataAsync(data);

                session.CloseDataConnection();
                await session.SendResponseAsync(226, "Directory send OK");
            }
            catch (Exception ex)
            {
                session.CloseDataConnection();
                await session.SendResponseAsync(451, $"Error: {ex.Message}");
            }
        }
        // Tạo directory listing theo định dạng Unix-style (giống lệnh ls -l).
        private string GenerateDirectoryListing(string path)
        {
            var sb = new StringBuilder();

            try
            {
                // Liệt kê directories trước
                var directories = Directory.GetDirectories(path);
                foreach (var dir in directories)
                {
                    var dirInfo = new DirectoryInfo(dir);
                    // Format: drwxr-xr-x 1 owner group size date name
                    sb.AppendLine($"drwxr-xr-x 1 ftp ftp 0 {dirInfo.LastWriteTime:MMM dd HH:mm} {dirInfo.Name}");
                }

                // Liệt kê files
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    // Format: -rw-r--r-- 1 owner group size date name
                    sb.AppendLine($"-rw-r--r-- 1 ftp ftp {fileInfo.Length} {fileInfo.LastWriteTime:MMM dd HH:mm} {fileInfo.Name}");
                }
            }
            catch (Exception ex)
            {
                // Trả về empty listing nếu có lỗi
                sb.AppendLine($"; Error: {ex.Message}");
            }

            return sb.ToString();
        }
    }
}
