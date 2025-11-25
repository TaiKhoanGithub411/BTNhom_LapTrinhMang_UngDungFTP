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


        public void SaveToFile(string path)
        {
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (var writer = new StreamWriter(path, false))
                {
                    writer.WriteLine(Port);
                    writer.WriteLine(RootFolder ?? "");
                    writer.WriteLine(MaxConnections);
                    writer.WriteLine(MaxConnectionsPerUser);
                    writer.WriteLine(LoginTimeout);
                    writer.WriteLine(AllowAllConnections);
                    writer.WriteLine(string.Join(";", BannedIPs ?? new List<string>()));
                }
            }
            catch
            {
                // Có thể log lỗi nếu muốn, còn không thì bỏ trống cũng được
            }
        }

        public static ServerConfiguration LoadFromFile(string path, UserManager userManager)
        {
            // mặc định
            var config = CreateDefault();
            config.UserManager = userManager;

            if (!File.Exists(path))
                return config;

            try
            {
                var lines = File.ReadAllLines(path);
                int i = 0;

                if (lines.Length > i && int.TryParse(lines[i++], out int port))
                    config.Port = port;

                if (lines.Length > i)
                    config.RootFolder = lines[i++];

                if (lines.Length > i && int.TryParse(lines[i++], out int maxConn))
                    config.MaxConnections = maxConn;

                if (lines.Length > i && int.TryParse(lines[i++], out int maxPerUser))
                    config.MaxConnectionsPerUser = maxPerUser;

                if (lines.Length > i && int.TryParse(lines[i++], out int timeout))
                    config.LoginTimeout = timeout;

                if (lines.Length > i && bool.TryParse(lines[i++], out bool allowAll))
                    config.AllowAllConnections = allowAll;

                if (lines.Length > i)
                {
                    var ips = lines[i++]
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    config.BannedIPs = new List<string>(ips);
                }
            }
            catch
            {
                // Nếu file lỗi format thì cứ dùng default cho lành
            }

            return config;
        }
    }
}
