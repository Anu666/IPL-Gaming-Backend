using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

namespace IPL.Gaming.Services
{
    /// <summary>
    /// Service to cache users and their API keys for fast lookup
    /// </summary>
    public class UserCacheService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, User> _apiKeyCache;
        private Timer? _refreshTimer;
        private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(5); // Refresh every 5 minutes

        public UserCacheService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _apiKeyCache = new ConcurrentDictionary<string, User>();
        }

        /// <summary>
        /// Get user by API key from cache
        /// </summary>
        public User? GetUserByApiKey(string apiKey)
        {
            _apiKeyCache.TryGetValue(apiKey, out var user);
            return user;
        }

        /// <summary>
        /// Refresh the cache with all users from database
        /// </summary>
        public async Task RefreshCache()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                var users = await userService.GetAllUsers();
                
                _apiKeyCache.Clear();
                
                foreach (var user in users)
                {
                    if (!string.IsNullOrWhiteSpace(user.ApiKey) && user.IsActive)
                    {
                        _apiKeyCache.TryAdd(user.ApiKey, user);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing user cache: {ex.Message}");
            }
        }

        /// <summary>
        /// Get count of cached users
        /// </summary>
        public int GetCachedUserCount()
        {
            return _apiKeyCache.Count;
        }

        /// <summary>
        /// Called when the application starts
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Loading users into cache...");
            await RefreshCache();
            Console.WriteLine($"Loaded {GetCachedUserCount()} users into cache.");

            // Set up periodic refresh timer
            _refreshTimer = new Timer(
                async _ => await RefreshCache(),
                null,
                _refreshInterval,
                _refreshInterval
            );
        }

        /// <summary>
        /// Called when the application stops
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _refreshTimer?.Change(Timeout.Infinite, 0);
            _apiKeyCache.Clear();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _refreshTimer?.Dispose();
        }
    }
}
