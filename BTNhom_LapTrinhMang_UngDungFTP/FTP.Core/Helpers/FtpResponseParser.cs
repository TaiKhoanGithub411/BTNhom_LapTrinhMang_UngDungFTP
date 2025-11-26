using System;
using System.Text.RegularExpressions;
using FTP.Core.Models;

namespace FTP.Core.Helpers
{
    // Parse các response từ FTP server
    public static class FtpResponseParser
    {
        // Parse response từ lệnh PASV Format: 227 Entering Passive Mode (h1,h2,h3,h4,p1,p2)
        public static (string ip, int port) ParsePasvResponse(string response)
        {
            // Regex tìm 6 số trong dấu ngoặc: (192,168,2,7,195,81)
            Match match = Regex.Match(response, @"\((\d+),(\d+),(\d+),(\d+),(\d+),(\d+)\)");

            if (!match.Success)
            {
                throw new FormatException($"Invalid PASV response: {response}");
            }

            // Xây dựng IP từ 4 số đầu
            string ip = string.Format("{0}.{1}.{2}.{3}",
                match.Groups[1].Value,
                match.Groups[2].Value,
                match.Groups[3].Value,
                match.Groups[4].Value);

            // Tính port từ 2 số cuối: port = p1 * 256 + p2
            int p1 = int.Parse(match.Groups[5].Value);
            int p2 = int.Parse(match.Groups[6].Value);
            int port = p1 * 256 + p2;

            return (ip, port);
        }

        // Parse response từ lệnh PWD Format: 257 "/current/path" is current directory
        public static string ParsePwdResponse(string response)
        {
            Match match = Regex.Match(response, "\"(.+?)\"");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return "/";
        }
        // Parse một dòng từ LIST response Format Unix: drwxr-xr-x 2 user group 4096 Nov 17 10:19 filename
        public static FtpFileItem ParseListLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            try
            {
                // Split bằng khoảng trắng, tối đa 9 phần
                string[] parts = line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 9)
                    return null;

                FtpFileItem item = new FtpFileItem
                {
                    Permissions = parts[0],
                    IsDirectory = parts[0].StartsWith("d"),
                    Name = parts[8].Trim()
                };

                // Parse size
                long size;
                if (long.TryParse(parts[4], out size))
                {
                    item.Size = item.IsDirectory ? -1 : size;
                }

                // Parse date: "Nov 17 10:19" hoặc "Nov 17 2024"
                item.Modified = ParseDate(parts[5], parts[6], parts[7]);
                item.FullPath = item.Name;

                return item;
            }
            catch
            {
                return null;
            }
        }
        // Parse date từ LIST response
        private static DateTime ParseDate(string month, string day, string yearOrTime)
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                string dateStr;

                // Nếu có dấu ":" thì là giờ (HH:mm), năm = năm hiện tại
                if (yearOrTime.Contains(":"))
                {
                    dateStr = string.Format("{0} {1} {2} {3}", month, day, currentYear, yearOrTime);
                }
                else
                {
                    // Nếu không có ":", đó là năm
                    dateStr = string.Format("{0} {1} {2}", month, day, yearOrTime);
                }

                return DateTime.Parse(dateStr);
            }
            catch
            {
                return DateTime.Now;
            }
        }
        // Kiểm tra response code có phải success không
        public static bool IsSuccessResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response) || response.Length < 3)
                return false;

            string code = response.Substring(0, 3);
            // Code 2xx và 3xx là success
            return code.StartsWith("2") || code.StartsWith("3");
        }
        // Lấy response code từ response string
        public static int GetResponseCode(string response)
        {
            if (string.IsNullOrWhiteSpace(response) || response.Length < 3)
                return 0;

            int code;
            if (int.TryParse(response.Substring(0, 3), out code))
                return code;

            return 0;
        }
    }
}
