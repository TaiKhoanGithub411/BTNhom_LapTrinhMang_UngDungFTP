using System;

namespace FTP.Core.Authentication
{
    [Flags]
    public enum UserPermissions // Các quyền truy cập của user.
    {
        None = 0,
        Read = 1, //LIST, RETR, CWD
        Write = 2, //STOR, APPE
        Delete = 4, //DELE
        CreateDirectory = 8, //MKD
        DeleteDirectory = 16, //RMD
        //Rename = 32,

        //Shortcuts
        ReadOnly = Read,
        ReadWrite = Read | Write | CreateDirectory,
        FullAccess = Read | Write | Delete | CreateDirectory | DeleteDirectory
    }
}
