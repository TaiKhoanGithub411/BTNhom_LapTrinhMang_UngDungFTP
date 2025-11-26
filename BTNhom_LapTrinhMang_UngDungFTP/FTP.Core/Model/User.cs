using System;
using FTP.Core.Authentication;

namespace FTP.Core.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public UserPermissions Permissions { get; set; }
        public string HomeDirectory { get; set; }// Thư mục gốc (physical path) của user.
        public long QuotaBytes { get; set; }// Giới hạn dung lượng (bytes). -1 = unlimited.
        public long UsedBytes { get; set; }// Dung lượng đã sử dụng (bytes).
        public bool IsDisabled { get; set; }// Trạng thái tài khoản user
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        public User()
        {
            Permissions = UserPermissions.ReadOnly;
            QuotaBytes = -1;
            UsedBytes = 0;
            IsDisabled = false;
            CreatedAt = DateTime.Now;
        }
    }
}
