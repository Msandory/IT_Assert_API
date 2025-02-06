using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Text.Json;


namespace Inventory_System_API.Services_Interfaces
{
    public class ServerSessionStore
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<ServerSessionStore> _logger;

        public ServerSessionStore(IDistributedCache cache, ILogger<ServerSessionStore> logger)
        {
            _cache = cache;
            _logger = logger;
        }
        public class UserSession
        {
            public string UserId { get; set; }
            public string Username { get; set; }
            public string Role { get; set; }
            public DateTime LoginTime { get; set; }
            public DateTime LastActivityTime { get; set; }
        }

        public async Task StoreSessionAsync(string userId, UserSession session)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) // Adjust as needed
            };

            await _cache.SetStringAsync(userId, JsonSerializer.Serialize(session), options);
            _logger.LogInformation($"Stored session for user {userId}");
            var activeUsers = await GetActiveUserIdsAsync() ?? new List<string>();

            if (!activeUsers.Contains(userId))
            {
                activeUsers.Add(userId);
                await _cache.SetStringAsync("activeUsers", JsonSerializer.Serialize(activeUsers), options);
                _logger.LogInformation($"Added user {userId} to the active users list");
            }
        }
        public async Task<List<UserSession>> GetSessionAsync()
        {
            // Get the list of active user IDs
            var activeUsersJson = await _cache.GetStringAsync("activeUsers");

            if (activeUsersJson == null)
            {
                _logger.LogWarning("No active users found");
                return new List<UserSession>();
            }

            var activeUserIds = JsonSerializer.Deserialize<List<string>>(activeUsersJson);
            var activeSessions = new List<UserSession>();

            // Retrieve each user's session
            foreach (var userId in activeUserIds)
            {
                var sessionJson = await _cache.GetStringAsync(userId);
                if (sessionJson != null)
                {
                    var session = JsonSerializer.Deserialize<UserSession>(sessionJson);
                    activeSessions.Add(session);
                }
            }

            return activeSessions;
        }
        public async Task<UserSession> GetSessionAsync(string userId)
        {
            var sessionJson = await _cache.GetStringAsync(userId);
            if (sessionJson == null)
            {
                _logger.LogWarning($"Session not found for user {userId}");
                return null;
            }

            return JsonSerializer.Deserialize<UserSession>(sessionJson);
        }

        public async Task RemoveSessionAsync(string userId)
        {
            await _cache.RemoveAsync(userId);
            _logger.LogInformation($"Removed session for user {userId}");
        }

        internal async Task StoreSessionAsync(string userId, ConcurrentDictionary<string, UserSession> activeSessions)
        {
            throw new NotImplementedException();
        }
        public async Task<List<string>> GetActiveUserIdsAsync()
        {
            var activeUsersJson = await _cache.GetStringAsync("activeUsers");
            if (activeUsersJson == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<List<string>>(activeUsersJson);
        }
    }
}
