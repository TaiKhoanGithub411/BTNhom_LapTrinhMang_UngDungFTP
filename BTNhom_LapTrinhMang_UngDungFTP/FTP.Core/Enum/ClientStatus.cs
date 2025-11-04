using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTP.Core.Enum
{
    public enum ClientStatus //Trạng thái của client
    {
        Connected,
        Idle, //Đang chờ
        Uploading,
        Downloading,
        Disconnected
    }
}
