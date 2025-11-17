using System.Threading.Tasks;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Server;

namespace FTP.Core.Protocol.Commands
{
    // Xử lý lệnh TYPE (Set transfer type) - TYPE A = ASCII mode, TYPE I = Binary mode
    public class TypeCommand:IFtpCommand
    {
        public async Task ExecuteAsync(ClientSession session, string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                await session.SendResponseAsync(501, "Syntax error: TYPE <type-code>");
                return;
            }

            string typeCode = arguments.Trim().ToUpperInvariant();

            switch (typeCode)
            {
                case "A":
                    // ASCII mode
                    await session.SendResponseAsync(200, "Type set to A (ASCII)");
                    break;

                case "I":
                    // Binary mode
                    await session.SendResponseAsync(200, "Type set to I (Binary)");
                    break;

                default:
                    await session.SendResponseAsync(504, "Type not supported");
                    break;
            }
        }
    }
}
