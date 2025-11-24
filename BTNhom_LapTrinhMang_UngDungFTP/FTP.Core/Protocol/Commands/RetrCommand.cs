using System;
using System.IO;
using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;
using FTP.Core.Enum;
using FTP.Core.Constants;
using FTP.Core.Authentication;

namespace FTP.Core.Protocol.Commands
{
    // Xử lý lệnh RETR (Retrieve - Download file từ server).
    public class RetrCommand : IFtpCommand
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
            //Kiểm tra quyền
            if (!PermissionChecker.CanRead(session.CurrentUser.Permissions))
            {
                await session.SendResponseAsync(550, "Permission denied");
                return;
            }

            // Kiểm tra argument
            if (string.IsNullOrWhiteSpace(arguments))
            {
                await session.SendResponseAsync(501, "Syntax error: RETR <filename>");
                return;
            }          

            // Kiểm tra passive mode
            if (!isPassiveMode && !isActiveMode)
            {
                await session.SendResponseAsync(425, "Use PORT or PASV first");
                return;
            }

            string fileName = arguments.Trim();

            try
            {
                // Chuyển đổi sang physical path
                string physicalPath = session.GetPhysicalPath(fileName);

                if (physicalPath == null || !File.Exists(physicalPath))
                {
                    await session.SendResponseAsync(550, $"File not found: {fileName}");
                    return;
                }

                // Lấy thông tin file
                var fileInfo = new FileInfo(physicalPath);

                // Gửi response bắt đầu
                await session.SendResponseAsync(150, $"Opening data connection for {fileName} ({fileInfo.Length} bytes)");

                // Chấp nhận data connection
                bool connected = isPassiveMode
            ? await session.SetupDataConnectionAsync()      // Chờ client kết nối (Passive)
            : await session.SetupActiveDataConnectionAsync(); // Server tự kết nối (Active)
                if (!connected)
                {
                    await session.SendResponseAsync(425, "Can't open data connection");
                    return;
                }

                // Cập nhật status
                session.Status = ClientStatus.Downloading;

                // Đọc và gửi file
                using (FileStream fs = new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] buffer = new byte[FtpConstants.DEFAULT_BUFFER_SIZE];
                    int bytesRead;

                    while ((bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        byte[] dataToSend = new byte[bytesRead];
                        Array.Copy(buffer, dataToSend, bytesRead);
                        await session.SendDataAsync(dataToSend);
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
                await session.SendResponseAsync(550, "Permission denied");
            }
            catch (IOException ex)
            {
                session.CloseDataConnection();
                session.Status = ClientStatus.Idle;
                await session.SendResponseAsync(451, $"Error reading file: {ex.Message}");
            }
            catch (Exception ex)
            {
                session.CloseDataConnection();
                session.Status = ClientStatus.Idle;
                await session.SendResponseAsync(451, $"Error during transfer: {ex.Message}");
            }
        }
    }
}
