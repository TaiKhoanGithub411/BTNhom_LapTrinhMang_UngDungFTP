using System;

namespace FTP.Core.Authentication
{
    // Helper để kiểm tra quyền của user.
    public static class PermissionChecker
    {
        // Kiểm tra xem user có quyền không.
        public static bool HasPermission(Authentication.UserPermissions userPermissions,
                                         Authentication.UserPermissions requiredPermission)
        {
            return (userPermissions & requiredPermission) == requiredPermission;
        }

        // Kiểm tra quyền đọc.
        public static bool CanRead(Authentication.UserPermissions userPermissions)
        {
            return HasPermission(userPermissions, Authentication.UserPermissions.Read);
        }

        // Kiểm tra quyền ghi.
        public static bool CanWrite(Authentication.UserPermissions userPermissions)
        {
            return HasPermission(userPermissions, Authentication.UserPermissions.Write);
        }
        // Kiểm tra quyền xóa file.
        public static bool CanDelete(Authentication.UserPermissions userPermissions)
        {
            return HasPermission(userPermissions, Authentication.UserPermissions.Delete);
        }
        // Kiểm tra quyền tạo thư mục.
        public static bool CanCreateDirectory(Authentication.UserPermissions userPermissions)
        {
            return HasPermission(userPermissions, Authentication.UserPermissions.CreateDirectory);
        }

        // Kiểm tra quyền xóa thư mục.

        public static bool CanDeleteDirectory(Authentication.UserPermissions userPermissions)
        {
            return HasPermission(userPermissions, Authentication.UserPermissions.DeleteDirectory);
        }
    }
}
