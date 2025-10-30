using AutoMapper;
using Catalogo.Application.Constants;
using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;
using Catalogo.Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using System.Text.Json;

namespace Catalogo.Application.Services
{
    internal class HybridCacheService : IHybridCacheService
    {

        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;


        public HybridCacheService(IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        public async Task<T>? GetOrCreatAsync<T>(

            string cacheKeyL1,
            string cacheKeyL2,
            Func<Task<T>> factory,
            TimeSpan? absoluteExporationL1 = null,
            TimeSpan? absoluteExporationL2 = null
         )
        {

            await Task.CompletedTask;
            return default;

        }



        public async Task RemoveAsync(string cacheKeyL1, string cacheKeyL2)
        {
            await Task.CompletedTask;
        }
    }
}
