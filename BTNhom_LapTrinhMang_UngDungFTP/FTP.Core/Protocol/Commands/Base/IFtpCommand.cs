using System;
using System.Collections.Generic;
using FTP.Core.Server;
using System.Threading.Tasks;

namespace FTP.Core.Protocol.Commands.Base
{
    //Interface cho tất cả FTP Commands
    public interface IFtpCommand
    {
        //Thực thi command bất đồng bộ
        Task ExecuteAsync(ClientSession session, string arguments);
    }
}
