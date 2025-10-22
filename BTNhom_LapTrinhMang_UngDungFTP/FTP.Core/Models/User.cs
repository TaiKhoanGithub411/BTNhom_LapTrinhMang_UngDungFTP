using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTP.Core
{
    public enum FtpPermissions
    {
        None,
        Write,
        Delete,
        List
    }
    public class User
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }//Thuộc tính này lưu trữ chuỗi hash ra từ mật khẩu của người dùng
        public string HomeDirectory { get; set; }//Thư mục mà người dùng cho phép truy cập
        public List<FtpPermissions> Permissions { get; set; }//Quyền mà người dùng được dùng
        public bool IsActived { get; set; }
        public DateTime CreateDate { get; set; }
        public User()
        {
            CreateDate = DateTime.Now;
            Permissions = new List<FtpPermissions>(); 
            IsActived = true; 
        }
        public User(string username, string passwordHash, string homeDirectory,
                List<FtpPermissions> permissions, bool isActive, DateTime createdAt)
        {
            UserName = username;
            PasswordHash = passwordHash;
            HomeDirectory = homeDirectory;
            Permissions = permissions ?? new List<FtpPermissions>(); // Nếu null thì khởi tạo mới
            IsActived = isActive;
            CreateDate = createdAt;
        }
    }
}
