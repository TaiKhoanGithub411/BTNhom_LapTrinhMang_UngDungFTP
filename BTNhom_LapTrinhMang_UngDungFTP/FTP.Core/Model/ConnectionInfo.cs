using System;

namespace FTP.Core.Models
{
    // Thông tin kết nối FTP - dùng để lưu cấu hình connection
    public class ConnectionInfo
    {
        public string Host { get; set; }//Địa chỉ IP của FTP Server
        public int Port { get; set; } = 21;//Mặc định là cổng 21
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public bool UsePassiveMode { get; set; } = true;//Kiểm tra chế độ (true: PASV, false: PORT)
        public int Timeout { get; set; } = 30000;
        public bool IsValid()//Kiểm tra thông tin có hợp lệ không
        {
            return !string.IsNullOrWhiteSpace(Host) &&
                   Port > 0 && Port <= 65535 &&
                   !string.IsNullOrWhiteSpace(UserName);
        }

        public override string ToString()
        {
            return $"{UserName}@{Host}:{Port}";
        }
    }
}
