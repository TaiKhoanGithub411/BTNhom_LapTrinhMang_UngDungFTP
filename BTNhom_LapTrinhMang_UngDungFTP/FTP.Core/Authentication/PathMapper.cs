using System;
using System.IO;

namespace FTP.Core.Authentication
{
    // Map virtual path (FTP path) sang physical path và ngược lại.
    public class PathMapper
    {
        private readonly string _homeDirectory;

        public PathMapper(string homeDirectory)
        {
            _homeDirectory = homeDirectory ?? throw new ArgumentNullException(nameof(homeDirectory));
        }

        /* Chuyển virtual path (FTP path) sang physical path.
         Virtual path: /folder/file.txt
         Physical path: C:\FTPRoot\Users\john\folder\file.txt*/
        public string GetPhysicalPath(string virtualPath)
        {
            if (string.IsNullOrWhiteSpace(virtualPath))
            {
                return _homeDirectory;
            }

            // Chuẩn hóa path
            virtualPath = virtualPath.Replace('/', Path.DirectorySeparatorChar);

            // Xóa leading slash
            if (virtualPath.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                virtualPath = virtualPath.Substring(1);
            }

            // Combine với home directory
            string physicalPath = Path.Combine(_homeDirectory, virtualPath);

            // Chuẩn hóa path
            physicalPath = Path.GetFullPath(physicalPath);

            // Security check: đảm bảo path không escape khỏi home directory
            if (!physicalPath.StartsWith(_homeDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return null; // Path traversal attack!
            }

            return physicalPath;
        }

        
        /* Chuyển physical path sang virtual path (FTP path).
         Physical path: C:\FTPRoot\Users\john\folder\file.txt
         Virtual path: /folder/file.txt*/
        public string GetVirtualPath(string physicalPath)
        {
            if (string.IsNullOrWhiteSpace(physicalPath))
            {
                return "/";
            }

            physicalPath = Path.GetFullPath(physicalPath);

            // Security check
            if (!physicalPath.StartsWith(_homeDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Lấy relative path
            string relativePath = physicalPath.Substring(_homeDirectory.Length);

            // Chuẩn hóa path separators
            relativePath = relativePath.Replace(Path.DirectorySeparatorChar, '/');

            // Đảm bảo bắt đầu với /
            if (!relativePath.StartsWith("/"))
            {
                relativePath = "/" + relativePath;
            }

            return relativePath;
        }

        // Kiểm tra xem path có nằm trong home directory không.
        public bool IsPathAllowed(string physicalPath)
        {
            if (string.IsNullOrWhiteSpace(physicalPath))
            {
                return false;
            }

            try
            {
                physicalPath = Path.GetFullPath(physicalPath);
                return physicalPath.StartsWith(_homeDirectory, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
