using System;
using System.IO;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;
using FTP.Core.Enum;
using FTP.Core.Constants;

namespace FTP.Core.Protocol.Commands
{
    // Xử lý lệnh STOR (Store - Upload file lên server).
    public class StorCommand : IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            bool isPassiveMode = session.PassiveListener != null;
            bool isActiveMode = !string.IsNullOrEmpty(session.ActiveModeIP);
            // Kiểm tra xác thực
            if (!session.IsAuthenticated)
            {
                await session.SendResponseAsync(530, "Not logged in");
                return;
            }

            // Kiểm tra argument
            if (string.IsNullOrWhiteSpace(arguments))
            {
                await session.SendResponseAsync(501, "Syntax error: STOR <filename>");
                return;
            }

            if (!isPassiveMode && !isActiveMode)
            {
                await session.SendResponseAsync(425, "Use PORT or PASV first");
                return;
            }

            string fileName = arguments.Trim();
            string physicalPath = null;

            try
            {
                // Chuyển đổi sang physical path
                physicalPath = session.GetPhysicalPath(fileName);

                if (physicalPath == null)
                {
                    await session.SendResponseAsync(553, "Invalid file name");
                    return;
                }

                // Kiểm tra thư mục cha có tồn tại không
                string directory = Path.GetDirectoryName(physicalPath);
                if (!Directory.Exists(directory))
                {
                    await session.SendResponseAsync(550, "Directory not found");
                    return;
                }

                // Gửi response bắt đầu
                await session.SendResponseAsync(150, $"Opening data connection for {fileName}");

                // Chấp nhận data connection
                bool connected = isPassiveMode
             ? await session.SetupDataConnectionAsync()
             : await session.SetupActiveDataConnectionAsync();
                if (!connected)
                {
                    await session.SendResponseAsync(425, "Can't open data connection");
                    return;
                }

                // Cập nhật status
                session.Status = ClientStatus.Uploading;

                // Nhận và ghi file
                using (FileStream fs = new FileStream(physicalPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[FtpConstants.DEFAULT_BUFFER_SIZE];
                    int bytesRead;

                    while ((bytesRead = await session.ReceiveDataAsync(buffer)) > 0)
                    {
                        await fs.WriteAsync(buffer, 0, bytesRead);
                    }
                }

                // Đóng data connection
                session.CloseDataConnection();

                // Reset status      
                session.Status = ClientStatus.Idle;

                // Gửi response hoàn thành
                await session.SendResponseAsync(226, "Transfer complete");
            }
            catch (UnauthorizedAccessException)
            {
                session.CloseDataConnection();
                session.Status = ClientStatus.Idle;

                // Xóa file nếu tạo không thành công
                if (physicalPath != null && File.Exists(physicalPath))
                {
                    try { File.Delete(physicalPath); } catch { }
                }

                await session.SendResponseAsync(550, "Permission denied");
            }
            catch (IOException ex)
            {
                session.CloseDataConnection();
                session.Status = ClientStatus.Idle;

                // Xóa file nếu tạo không thành công
                if (physicalPath != null && File.Exists(physicalPath))
                {
                    try { File.Delete(physicalPath); } catch { }
                }

                await session.SendResponseAsync(451, $"Error writing file: {ex.Message}");
            }
            catch (Exception ex)
            {
                session.CloseDataConnection();
                session.Status = ClientStatus.Idle;

                // Xóa file nếu tạo không thành công
                if (physicalPath != null && File.Exists(physicalPath))
                {
                    try { File.Delete(physicalPath); } catch { }
                }

                await session.SendResponseAsync(451, "Error during transfer");
            }
        }
    }
}
