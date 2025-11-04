using FTP.Core.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FTP.Core.Server
{ 
    //Quản lý danh sách các ClientSession đang hoạt động - Sử dụng ConcurrentDictionary để đảm bảo thread-safe.
    public class SessionManager
    {
        // Dictionary thread-safe để lưu sessions
        private readonly ConcurrentDictionary<string, ClientSession> _sessions;
        public event Action<ClientSession> SessionAdded;
        public event Action<string> SessionRemoved;
        public int ActiveSessionCount => _sessions.Count;
        public SessionManager()
        {
            _sessions = new ConcurrentDictionary<string, ClientSession>();
        }
        public bool AddSession(ClientSession session)//Thêm session mới vào danh sách quản lý.
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            bool added = _sessions.TryAdd(session.SessionId, session);

            if (added)
            {
                SessionAdded?.Invoke(session);
            }

            return added;
        }
        public bool RemoveSession(string sessionId)//Xóa session khỏi danh sách quản lý.
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return false;
            }

            bool removed = _sessions.TryRemove(sessionId, out _);

            if (removed)
            {
                SessionRemoved?.Invoke(sessionId);
            }

            return removed;
        }
        public ClientSession GetSession(string sessionId)//Lấy session theo SessionId.
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return null;
            }

            _sessions.TryGetValue(sessionId, out var session);
            return session;
        }
        public IReadOnlyList<ClientSession> GetAllSessions()//Lấy tất cả sessions đang hoạt động.
        {
            return _sessions.Values.ToList().AsReadOnly();
        }
        public IReadOnlyList<ClientSession> GetSessionsByUsername(string username)//Lấy danh sách sessions theo username.
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return new List<ClientSession>().AsReadOnly();
            }

            return _sessions.Values
                .Where(s => s.Username != null &&
                           s.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .AsReadOnly();
        }
        public void CloseAllSessions()//Đóng tất cả sessions (dùng khi Stop server).
        {
            foreach (var session in _sessions.Values)
            {
                try
                {
                    session.Close();
                }
                catch
                {
                    // Bỏ qua lỗi khi đóng server
                }
            }
            _sessions.Clear();
        }
        public bool DisconnectSession(string sessionId)//Ngắt kết nối một session cụ thể.
        {
            var session = GetSession(sessionId);
            if (session == null)
            {
                return false;
            }

            try
            {
                session.Close();
                RemoveSession(sessionId);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool IsConnectionLimitReached(int maxConnections)//Kiểm tra xem có vượt quá số lượng kết nối cho phép không.
        {
            return _sessions.Count >= maxConnections;
        }
        public bool IsUserConnectionLimitReached(string username, int maxConnectionsPerUser)//Kiểm tra user đã vượt quá số kết nối cho phép chưa.
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            var userSessions = GetSessionsByUsername(username);
            return userSessions.Count >= maxConnectionsPerUser;
        }
    }
}
