using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;

namespace FTP.Core.Protocol.Commands
{
    // Xử lý lệnh PWD (Print Working Directory) - hiển thị thư mục làm việc hiện tại
    public class PwdCommand:IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            if (!session.IsAuthenticated)
            {
                await session.SendResponseAsync(530, "Not logged in");
                return;
            }
            await session.SendResponseAsync(257, $"\"{session.CurrentDirectory}\" is current directory");
        }
    }
}
