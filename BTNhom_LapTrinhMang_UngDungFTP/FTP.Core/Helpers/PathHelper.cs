using System;
using System.Collections.Generic;
using System.Linq;

namespace FTP.Core.Helpers
{
    // Xử lý path FTP (luôn dùng forward slash /)
    public static class PathHelper
    {
        // Kết hợp các phần path FTP
        public static string Combine(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return "/";

            // Lọc các phần hợp lệ
            List<string> validPaths = new List<string>();
            foreach (string p in paths)
            {
                if (!string.IsNullOrWhiteSpace(p))
                    validPaths.Add(p);
            }

            if (validPaths.Count == 0)
                return "/";

            // Trim slashes và join
            List<string> trimmedPaths = new List<string>();
            foreach (string p in validPaths)
            {
                trimmedPaths.Add(p.Trim('/'));
            }

            string combined = string.Join("/", trimmedPaths.ToArray());

            // Đảm bảo bắt đầu bằng /
            if (!combined.StartsWith("/"))
                combined = "/" + combined;

            return NormalizePath(combined);
        }

        // Normalize path (xử lý .. và .)
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "/";

            string[] parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            Stack<string> stack = new Stack<string>();

            foreach (string part in parts)
            {
                if (part == "..")
                {
                    // Lùi lại 1 cấp
                    if (stack.Count > 0)
                        stack.Pop();
                }
                else if (part != ".")
                {
                    // Thêm vào stack (bỏ qua ".")
                    stack.Push(part);
                }
            }

            // Rebuild path
            if (stack.Count == 0)
                return "/";

            List<string> reversed = new List<string>(stack);
            reversed.Reverse();
            return "/" + string.Join("/", reversed.ToArray());
        }

        // Lấy tên file/folder từ path
        public static string GetFileName(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            path = path.TrimEnd('/');
            int lastSlash = path.LastIndexOf('/');

            return lastSlash >= 0 ? path.Substring(lastSlash + 1) : path;
        }
        // Lấy parent directory từ path
        public static string GetParentPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path == "/")
                return "/";

            path = path.TrimEnd('/');
            int lastSlash = path.LastIndexOf('/');

            if (lastSlash <= 0)
                return "/";

            return path.Substring(0, lastSlash);
        }
        // Kiểm tra có phải absolute path không
        public static bool IsAbsolutePath(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && path.StartsWith("/");
        }
        // Convert relative → absolute path
        public static string ToAbsolutePath(string currentPath, string relativePath)
        {
            if (IsAbsolutePath(relativePath))
                return relativePath;

            return Combine(currentPath, relativePath);
        }
    }
}
