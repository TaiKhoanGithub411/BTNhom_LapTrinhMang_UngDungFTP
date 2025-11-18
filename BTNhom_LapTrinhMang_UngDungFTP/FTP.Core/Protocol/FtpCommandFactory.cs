using System;
using System.Collections.Generic;
using FTP.Core.Protocol.Commands.Base;
using FTP.Core.Protocol.Commands;

namespace FTP.Core.Protocol
{
    //Factory để tạo FTP command objects từ command string.
    public class FtpCommandFactory
    {
        private readonly Dictionary<string, Func<IFtpCommand>> _commandCreators;
        public FtpCommandFactory()
        {
            _commandCreators = new Dictionary<string, Func<IFtpCommand>>(StringComparer.OrdinalIgnoreCase)
            {
                // Authentication
                { "USER", () => new UserCommand() },
                { "PASS", () => new PassCommand() },
                { "QUIT", () => new QuitCommand() },

                // System info
                { "SYST", () => new SystCommand() },
                { "PWD", () => new PwdCommand() },
                { "XPWD", () => new PwdCommand() },

                // Directory navigation
                { "CWD", () => new CwdCommand() },    
                { "XCWD", () => new CwdCommand() },

                // Transfer mode
                { "TYPE", () => new TypeCommand() },
                { "PORT", () => new PortCommand() },
                { "PASV", () => new PasvCommand() },

                // Directory listing
                { "LIST", () => new ListCommand() },
                { "NLST", () => new ListCommand() },

                // File operations
                { "RETR", () => new RetrCommand() },
                { "STOR", () => new StorCommand() },
                { "DELE", () => new DeleCommand() },

                // Directory management
                { "MKD", () => new MkdCommand() },
                { "XMKD", () => new MkdCommand() },
                { "RMD", () => new RmdCommand() },
                { "XRMD", () => new RmdCommand() },
            };
        }
        // Tạo command object từ command string.
        public IFtpCommand CreateCommand(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                return null;
            }
            string upperCommandName = commandName.ToUpperInvariant();

            if (_commandCreators.TryGetValue(upperCommandName, out var creator))
            {
                return creator();
            }
            return null; // Command không được hỗ trợ
        }
        // Kiểm tra xem command có được hỗ trợ không.
        public bool IsCommandSupported(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                return false;
            }
            return _commandCreators.ContainsKey(commandName.ToUpperInvariant());
        }
    }
}
