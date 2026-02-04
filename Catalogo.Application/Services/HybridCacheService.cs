using System.Text.Json;
using Catalogo.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;


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

        public async Task<T> GetOrCreateAsync<T>(

            string cacheKeyL1,
            string cacheKeyL2,
            Func<Task<T>> factory,
            TimeSpan? absoluteExpirationL1,
            TimeSpan? absoluteExpirationL2
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
                        memoryCacheEntryOptions.SetAbsoluteExpiration(absoluteExpirationL1.Value);
                    }
                    _memoryCache.Set(cacheKeyL1, resultL2, memoryCacheEntryOptions);

                    return resultL2;
                }
            }

            var resultL3 = await factory();

            if (resultL3 != null)
            {
                var distributedCacheEntryOptions = new DistributedCacheEntryOptions();

                if (absoluteExpirationL2.HasValue)
                {
                    distributedCacheEntryOptions.SetAbsoluteExpiration(absoluteExpirationL2.Value);

                }

                await _distributedCache.SetStringAsync(cacheKeyL2, JsonSerializer.Serialize(resultL3), distributedCacheEntryOptions);

                var memoryCacheEntryOptions = new MemoryCacheEntryOptions();

                if (absoluteExpirationL1.HasValue)
                {
                    memoryCacheEntryOptions.SetAbsoluteExpiration(absoluteExpirationL1.Value);
                }
                _memoryCache.Set(cacheKeyL1, resultL3, memoryCacheEntryOptions);

                return resultL3;
            }


            return default;


        }



        public async Task RemoveAsync(string cacheKeyL1, string cacheKeyL2)
        {
            _memoryCache.Remove(cacheKeyL1);
            await _distributedCache.RemoveAsync(cacheKeyL2);


        }
    }
}
