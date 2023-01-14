using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Nop.Core.Caching;
using Nop.Core.Configuration;

namespace Nop.Services.Caching
{
    public class MemoryDistributedCacheManager : DistributedCacheManager
    {
        #region Ctor

        public MemoryDistributedCacheManager(AppSettings appSettings, IDistributedCache distributedCache)
        : base(appSettings, distributedCache)
        {
        }

        #endregion

        public override async Task ClearAsync()
        {
            foreach (var key in _localKeys.Keys)
                await RemoveAsync(key, false);
            ClearInstanceData();
        }

        public override async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            var prefix_ = PrepareKeyPrefix(prefix, prefixParameters);
            foreach (var key in RemoveByPrefixInstanceData(prefix_))
                await RemoveAsync(key, false);
        }
    }
}
