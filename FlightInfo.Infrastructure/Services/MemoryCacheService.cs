using FlightInfo.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightInfo.Infrastructure.Services
{
    /// <summary>
    /// Memory cache service implementation
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly Dictionary<string, DateTime> _cacheTimestamps = new();

        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets a value from cache
        /// </summary>
        /// <typeparam name="T">Type of the cached value</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>Cached value or null</returns>
        public async Task<T?> GetAsync<T>(string key)
        {
            await Task.CompletedTask;
            var result = _cache.Get<T>(key);

            if (result != null)
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
            }
            else
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
            }

            return result;
        }

        /// <summary>
        /// Sets a value in cache
        /// </summary>
        /// <typeparam name="T">Type of the value to cache</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">Value to cache</param>
        /// <param name="expiration">Expiration time</param>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            await Task.CompletedTask;
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration.Value;
            }
            _cache.Set(key, value, options);
            _cacheTimestamps[key] = DateTime.UtcNow;
            _logger.LogDebug("Cache set for key: {Key} with expiration: {Expiration}", key, expiration);
        }

        /// <summary>
        /// Removes a value from cache
        /// </summary>
        /// <param name="key">Cache key</param>
        public async Task RemoveAsync(string key)
        {
            await Task.CompletedTask;
            _cache.Remove(key);
        }

        /// <summary>
        /// Clears all cache entries
        /// </summary>
        public void Clear()
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
            _cacheTimestamps.Clear();
            _logger.LogInformation("Cache cleared");
        }

        public async Task<Dictionary<string, object>> GetCacheStatisticsAsync()
        {
            await Task.CompletedTask;
            return new Dictionary<string, object>
            {
                ["TotalEntries"] = _cacheTimestamps.Count,
                ["OldestEntry"] = _cacheTimestamps.Values.Any() ? _cacheTimestamps.Values.Min() : (DateTime?)null,
                ["NewestEntry"] = _cacheTimestamps.Values.Any() ? _cacheTimestamps.Values.Max() : (DateTime?)null,
                ["CacheKeys"] = _cacheTimestamps.Keys.ToList()
            };
        }

        public async Task<bool> IsExpiredAsync(string key)
        {
            await Task.CompletedTask;
            return !_cacheTimestamps.ContainsKey(key);
        }

        /// <summary>
        /// Gets or sets a value in cache
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="factory">Factory function to create value if not cached</param>
        /// <param name="expiration">Expiration time</param>
        /// <returns>Cached or newly created value</returns>
        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration) where T : class
        {
            var cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
            {
                return cachedValue;
            }

            var newValue = await factory();
            await SetAsync(key, newValue, expiration);
            return newValue;
        }
    }
}

