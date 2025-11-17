using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;

namespace FTP.Core.Protocol.Commands
{
    // Xử lý lệnh SYST (System type) - tự động gửi khi client login thành công
    public class SystCommand:IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            await session.SendResponseAsync(215, "UNIX Type: L8");// Trả về system type là UNIX
        }
    }
}
