using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FTP.Core.Server
{
    public class SessionManager
    {
        private readonly ConcurrentDictionary<string, ClientSession> _sessions;
        private readonly object _addSessionLock = new object(); // THÊM LOCK

        public event Action<ClientSession> SessionAdded;
        public event Action<string> SessionRemoved;

        public int ActiveSessionCount => _sessions.Count;

        public SessionManager()
        {
            _sessions = new ConcurrentDictionary<string, ClientSession>();
        }

        /// <summary>
        /// Thêm session mới vào danh sách quản lý.
        /// Xử lý trùng lặp SessionId bằng cách thêm suffix.
        /// </summary>
        public bool AddSession(ClientSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            // ===== THÊM LOCK ĐỂ TRÁNH RACE CONDITION =====
            lock (_addSessionLock)
            {
                string originalSessionId = session.SessionId;
                int attemptCount = 0;
                const int maxAttempts = 10;
                bool added = false;

                // Thử thêm session, nếu trùng thì tạo SessionId mới
                while (!added && attemptCount < maxAttempts)
                {
                    added = _sessions.TryAdd(session.SessionId, session);

                    if (!added)
                    {
                        // SessionId đã tồn tại, tạo SessionId mới
                        attemptCount++;
                        session.SessionId = $"{originalSessionId}_{attemptCount}";
                    }
                }

                if (!added)
                {
                    // Không thể tạo SessionId duy nhất
                    return false;
                }

                // Phát event
                SessionAdded?.Invoke(session);
                return true;
            }
            // =============================================
        }

        /// <summary>
        /// Xóa session khỏi danh sách quản lý.
        /// </summary>
        public bool RemoveSession(string sessionId)
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

        public ClientSession GetSession(string sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return session;
        }

        public IEnumerable<ClientSession> GetAllSessions()
        {
            return _sessions.Values.ToList();
        }

        public bool IsConnectionLimitReached(int maxConnections)
        {
            return _sessions.Count >= maxConnections;
        }
        public bool IsUserConnectionLimitReached(string username, int maxPerUser)
        {
            if (string.IsNullOrWhiteSpace(username) || maxPerUser <= 0)
                return false;

            var count = _sessions.Values.Count(s =>
                string.Equals(s.Username, username, StringComparison.OrdinalIgnoreCase));

            return count >= maxPerUser;
        }

        public bool DisconnectSession(string sessionId)
        {
            var session = GetSession(sessionId);
            if (session != null)
            {
                session.Close();
                return RemoveSession(sessionId);
            }
            return false;
        }

        public void CloseAllSessions()
        {
            var allSessions = _sessions.Values.ToList();

            foreach (var session in allSessions)
            {
                try
                {
                    session.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error closing session {session.SessionId}: {ex.Message}");
                }
            }

            _sessions.Clear();
        }
    }
}
