using System.Text.Json;
using AutoMapper;
using Catalogo.Application.Constants;
using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;
using Catalogo.Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace Catalogo.Application.Services
{
    public class HybridCacheService : IHybridCacheService
    {

        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;


        public HybridCacheService(IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        public async Task<T>? GetOrCreateAsync<T>(

            string cacheKeyL1,
            string cacheKeyL2,
            Func<Task<T>> factory,
            TimeSpan? absoluteExpirationL1 = null,
            TimeSpan? absoluteExpirationL2 = null
         )
        {
            if (_memoryCache.TryGetValue(cacheKeyL1, out T? resultL1) && resultL1 != null)
            {
                return resultL1;
            }

            string? resultJsonL2 = await _distributedCache.GetStringAsync(cacheKeyL2);
            if (!string.IsNullOrEmpty(resultJsonL2))
            {
                var resultL2 = JsonSerializer.Deserialize<T>(resultJsonL2);
                if (resultL2 != null)
                {
                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions();
                    if (absoluteExpirationL1.HasValue)
                    {
                        memoryCacheEntryOptions.SetSlidingExpiration(absoluteExpirationL1.Value);
                    }
                    return resultL2;
                }
            }

            await Task.CompletedTask;
            return default;

        }



        public async Task RemoveAsync(string cacheKeyL1, string cacheKeyL2)
        {
            _memoryCache.Remove(cacheKeyL1);
            await _distributedCache.RemoveAsync(cacheKeyL2);

        }
    }
}
