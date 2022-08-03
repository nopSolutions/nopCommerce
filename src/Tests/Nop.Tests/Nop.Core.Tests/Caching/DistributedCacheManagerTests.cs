using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Nop.Core.Caching;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Caching
{
    [TestFixture]
    public class DistributedCacheManagerTests : BaseNopTest
    {
        private DistributedCacheManager _staticCacheManager;

        [OneTimeSetUp]
        public void Setup()
        {
            _staticCacheManager = GetService<DistributedCacheManager>();
            
        }

        [Test]
        public async Task CanSetObjectInCacheAndWillTrackIfCleared()
        {
            await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 1);
            _distributedCache.Verify(x=> x.SetAsync("some_key_1", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
            await _staticCacheManager.RemoveByPrefixAsync("some_key_1");
            _distributedCache.Verify(x=> x.RemoveAsync("some_key_1", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task CanGetAsyncFromCacheAndWillTrackInPerRequestCacheIfCleared()
        {
            _distributedCache.Setup(x => x.GetAsync("some_key_2", It.IsAny<CancellationToken>())).ReturnsAsync(Encoding.UTF8.GetBytes("2"));
            await _staticCacheManager.GetAsync(new CacheKey("some_key_2"),()=> 2);
            await _staticCacheManager.RemoveByPrefixAsync("some_key_2");
            _distributedCache.Verify(x=> x.RemoveAsync("some_key_2", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task CanGetFromCacheAndWillTrackInPerRequestCacheIfCleared()
        {
            _distributedCache.Setup(x => x.Get("some_key_3")).Returns(Encoding.UTF8.GetBytes("3"));
            _staticCacheManager.Get(new CacheKey("some_key_3"),()=> 3);
            await _staticCacheManager.RemoveByPrefixAsync("some_key_3");
            _distributedCache.Verify(x=> x.RemoveAsync("some_key_3", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
