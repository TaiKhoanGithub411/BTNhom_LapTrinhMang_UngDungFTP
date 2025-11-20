using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FTP.Core.Authentication
{
    // Quản lý danh sách users và thao tác với user database.
    public class UserManager
    {
        private readonly string _usersFilePath;
        private List<User> _users;
        private readonly object _lock = new object();

        public UserManager(string usersFilePath)
        {
            _usersFilePath = usersFilePath ?? throw new ArgumentNullException(nameof(usersFilePath));
            _users = new List<User>();
            LoadUsers();
        }
        // Load users từ file JSON.
        public void LoadUsers()
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(_usersFilePath))
                    {
                        // File không tồn tại → Tạo default users
                        CreateDefaultUsers();
                        SaveUsers();
                        return;
                    }

                    // Đọc file JSON
                    string json = File.ReadAllText(_usersFilePath);

                    // Kiểm tra nội dung file
                    if (string.IsNullOrWhiteSpace(json) || json.Trim() == "[]")
                    {
                        // File rỗng hoặc array rỗng → Tạo default users
                        CreateDefaultUsers();
                        SaveUsers();
                        return;
                    }

                    // Deserialize JSON
                    _users = JsonSerializer.Deserialize<List<User>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<User>();

                    // Nếu không có users nào → Tạo default
                    if (_users.Count == 0)
                    {
                        CreateDefaultUsers();
                        SaveUsers();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading users: {ex.Message}");
                    CreateDefaultUsers();
                }
            }
        }

        // Lưu users vào file JSON.
        public void SaveUsers()
        {
            lock (_lock)
            {
                try
                {
                    string json = JsonSerializer.Serialize(_users, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    File.WriteAllText(_usersFilePath, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving users: {ex.Message}");
                }
            }
        }

        // Tạo default users (admin).
        private void CreateDefaultUsers()
        {
            _users.Clear();

            // Admin user
            _users.Add(new User
            {
                UserName = "admin",
                PasswordHash = PasswordHelper.HashPassword("admin"),
                Permissions = UserPermissions.FullAccess,
                HomeDirectory = "C:\\FTPRoot",
                QuotaBytes = -1, // Unlimited
                CreatedAt = DateTime.Now
            });
        }

        // Authenticate user.
        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            lock (_lock)
            {
                var user = _users.FirstOrDefault(u =>
                    u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (user == null || user.IsDisabled)
                {
                    return null;
                }

                if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
                {
                    return null;
                }

                // Update last login
                user.LastLogin = DateTime.Now;
                SaveUsers();

                return user;
            }
        }
        // Thêm user mới.
        public bool AddUser(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.UserName))
            {
                return false;
            }

            lock (_lock)
            {
                if (_users.Any(u => u.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase)))
                {
                    return false; // Username đã tồn tại
                }

                _users.Add(user);
                SaveUsers();
                return true;
            }
        }
        // Xóa user.
        public bool RemoveUser(string username)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u =>
                    u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    return false;
                }

                _users.Remove(user);
                SaveUsers();
                return true;
            }
        }
        // Cập nhật user.
        public bool UpdateUser(User updatedUser)
        {
            if (updatedUser == null || string.IsNullOrWhiteSpace(updatedUser.UserName))
            {
                return false;
            }

            lock (_lock)
            {
                var user = _users.FirstOrDefault(u =>
                    u.UserName.Equals(updatedUser.UserName, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    return false;
                }

                // Update properties
                user.PasswordHash = updatedUser.PasswordHash;
                user.Permissions = updatedUser.Permissions;
                user.HomeDirectory = updatedUser.HomeDirectory;
                user.QuotaBytes = updatedUser.QuotaBytes;
                user.IsDisabled = updatedUser.IsDisabled;

                SaveUsers();
                return true;
            }
        }
        // Lấy danh sách tất cả users.
        public List<User> GetAllUsers()
        {
            lock (_lock)
            {
                return _users.ToList();
            }
        }
        // Lấy user theo username.
        public User GetUser(string username)
        {
            lock (_lock)
            {
                return _users.FirstOrDefault(u =>
                    u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
