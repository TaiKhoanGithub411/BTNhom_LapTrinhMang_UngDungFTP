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
                { "USER", () => new UserCommand() },
                { "PASS", () => new PassCommand() },
                { "QUIT", () => new QuitCommand() },
                { "SYST", () => new SystCommand() },    
                { "PWD",  () => new PwdCommand() },     
                { "CWD",  () => new CwdCommand() }, 
                { "TYPE", () => new TypeCommand() },
                { "PORT", () => new PortCommand() },    
                { "PASV", () => new PasvCommand() },
                { "LIST", () => new ListCommand() },
                { "RETR", () => new RetrCommand() },
                { "STOR", () => new StorCommand() },
                { "XPWD", () => new PwdCommand() },
                { "NLST", () => new ListCommand() },
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
