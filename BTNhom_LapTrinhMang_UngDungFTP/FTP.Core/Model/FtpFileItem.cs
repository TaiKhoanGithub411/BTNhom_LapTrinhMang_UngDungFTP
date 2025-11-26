using System;
using System.IO;

namespace FTP.Core.Models
{
    // Đại diện cho một file hoặc folder trên FTP server. Parse từ response của lệnh LIST
    public class FtpFileItem
    {
        public string Name { get; set; }//Tên file/folder
        public string FullPath { get; set; }//Đường dẫn của file trên server
        public long Size { get; set; }//Kích thước file, -1 là folder
        public bool IsDirectory { get; set; }//Kiểm tra có là thư mục
        public DateTime Modified { get; set; }//Thời gian chỉnh sửa file lần cuối
        public string ModifiedFormatted => Modified.ToString("dd/MM/yyyy HH:mm");//Định dạng ngày tháng
        public string Permissions { get; set; }// Quyền truy cập Unix style (rwxr-xr-x)
        public string Extension => IsDirectory ? "" : Path.GetExtension(Name)?.ToLower();//Phần mở rộng file (extension)
        public string SizeFormatted//Định dạng kích thước file (1.2KB, 3MB,...)
        {
            get
            {
                if (IsDirectory) return "<DIR>";
                if (Size < 0) return "N/A";
                if (Size < 1024) return $"{Size} B";
                if (Size < 1024 * 1024) return $"{Size / 1024.0:F2} KB";
                if (Size < 1024 * 1024 * 1024) return $"{Size / (1024.0 * 1024):F2} MB";
                return $"{Size / (1024.0 * 1024 * 1024):F2} GB";
            }
        }
        public string IconType// Icon type để hiển thị trong UI
        {
            get
            {
                if (IsDirectory) return "folder";

                // Dùng switch statement cổ điển (C# 7.3)
                switch (Extension)
                {
                    case ".txt":
                    case ".log":
                        return "text";

                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".gif":
                    case ".bmp":
                        return "image";

                    case ".mp3":
                    case ".wav":
                    case ".wma":
                        return "audio";

                    case ".mp4":
                    case ".avi":
                    case ".mkv":
                        return "video";

                    case ".zip":
                    case ".rar":
                    case ".7z":
                        return "archive";

                    case ".pdf":
                        return "pdf";

                    case ".doc":
                    case ".docx":
                        return "word";

                    case ".xls":
                    case ".xlsx":
                        return "excel";

                    default:
                        return "file";
                }
            }
        }
        public override string ToString()
        {
            return $"{Name} ({SizeFormatted})";
        }
    }
}
