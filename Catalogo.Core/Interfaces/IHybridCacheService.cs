using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogo.Core.Interfaces
{
    public interface IHybridCacheService
    {
        Task<T>? GetOrCreatAsync<T>(
            string cacheKeyL1,
            string cacheKeyL2,
            Func<Task<T>> factory,
            TimeSpan? absoluteExporationL1 = null,
            TimeSpan? absoluteExporationL2 = null

            );

        Task RemoveAsync(string cacheKeyL1, string cacheKeyL2);
    }
}
