using System;
using System.Collections.Generic;
using System.IO;
using FTP.Core.Authentication;

namespace FTP.Core.Server
{
    //Cấu hình cho FTP Server - Lưu thông số như port, root folder, ...
    public class ServerConfiguration
    {
        public int Port { get; set; }
        public string RootFolder { get; set; }
        public int MaxConnections { get; set; } //Số lượng kết nối tối đa mà server cho phép đồng thời.
        public int MaxConnectionsPerUser { get; set; }//Số kết nối tối đa cho mỗi user.
        public int LoginTimeout { get; set; }
        public bool AllowAllConnections { get; set; }
        public UserManager UserManager { get; set; }
        public List<string> BannedIPs { get; set; }//Danh sách IP bị cấm kết nối
        public ServerConfiguration()
        {
            Port = 21;
            RootFolder = @"C:\FTPRoot";
            MaxConnections = 100;
            MaxConnectionsPerUser = 5;
            LoginTimeout = 300;
            AllowAllConnections = true;
            BannedIPs = new List<string>();
        }
        public bool IsValid()//Kiểm tra tính hợp lệ của cấu hình.
        {
            if (Port < 1 || Port > 65535)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(RootFolder))
            {
                return false;
            }
            if (!Directory.Exists(RootFolder))
            {
                try
                {
                    Directory.CreateDirectory(RootFolder);
                }
                catch
                {
                    return false;
                }
            }
            if (MaxConnections <= 0)
            {
                return false;
            }
            return true;
        }
        public static ServerConfiguration CreateDefault()//Tạo cấu hình mặc định.
        {
            return new ServerConfiguration
            {
                Port = 21,
                RootFolder = @"C:\FTPRoot",
                MaxConnections = 100,
                MaxConnectionsPerUser = 5,
                LoginTimeout = 300,
                AllowAllConnections = true,
                BannedIPs = new List<string>()
            };
        }
        public ServerConfiguration Clone()//Clone cấu hình hiện tại.
        {
            return new ServerConfiguration
            {
                Port = this.Port,
                RootFolder = this.RootFolder,
                MaxConnections = this.MaxConnections,
                MaxConnectionsPerUser = this.MaxConnectionsPerUser,
                LoginTimeout = this.LoginTimeout,
                AllowAllConnections = this.AllowAllConnections,
                BannedIPs = new List<string>(this.BannedIPs)
            };
        }
    }
}
