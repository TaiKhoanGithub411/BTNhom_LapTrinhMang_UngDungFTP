using System;
using System.Security.Cryptography;
using System.Text;

namespace FTP.Core.Authentication
{
    public class PasswordHelper//Hash và verify password
    {
        //Hash passwrod bằng SHA256
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
        // Verify password với hash
        public static bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            string computedHash = HashPassword(password);
            return string.Equals(computedHash, passwordHash, StringComparison.OrdinalIgnoreCase);
        }
        // Tạo password ngẫu nhiên
        public static string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            StringBuilder password = new StringBuilder();

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                foreach (byte b in randomBytes)
                {
                    password.Append(validChars[b % validChars.Length]);
                }
            }

            return password.ToString();
        }   
    }
}
