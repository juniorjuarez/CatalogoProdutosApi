using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogo.Core.Interfaces
{
    public interface IHybridCacheService
    {
        Task<T> GetOrCreateAsync<T>(
            string cacheKeyL1,
            string cacheKeyL2,
            Func<Task<T>> factory,
            TimeSpan? absoluteExpirationL1 = null,
            TimeSpan? absoluteExpirationL2 = null

            );

        Task RemoveAsync(string cacheKeyL1, string cacheKeyL2);
    }
}
