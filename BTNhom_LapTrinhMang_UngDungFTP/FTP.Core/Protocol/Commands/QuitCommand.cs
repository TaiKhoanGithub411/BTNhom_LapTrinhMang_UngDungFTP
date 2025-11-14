using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;

namespace FTP.Core.Protocol.Commands
{
    //Xử lý lệnh QUIT (ngắt kết nối)
    public class QuitCommand : IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            await session.SendResponseAsync(221, "Goodbye");
            session.Close();
        }
    }
}
