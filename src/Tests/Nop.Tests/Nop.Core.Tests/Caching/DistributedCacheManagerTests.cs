using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Caching;
using Nop.Services.Caching;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Caching
{
    [TestFixture]
    public class DistributedCacheManagerTests : BaseNopTest
    {
        private MemoryDistributedCacheManager _staticCacheManager;
        private IDistributedCache _distributedCache;
        private IServiceScopeFactory _serviceScopeFactory;

        [OneTimeSetUp]
        public void Setup()
        {
            _staticCacheManager = GetService<MemoryDistributedCacheManager>();
            _distributedCache = GetService<IDistributedCache>();
            _serviceScopeFactory = GetService<IServiceScopeFactory>();
        }

        [TearDown]
        public async Task TaskTearDown()
        {
            await _staticCacheManager.ClearAsync();
        }

        [Test]
        public async Task CanSetObjectInCacheAndWillTrackIfRemoved()
        {
            await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 1);
            (await _distributedCache.GetAsync("some_key_1")).Should().NotBeNullOrEmpty();
            await _staticCacheManager.RemoveByPrefixAsync("some_key_1");
            (await _distributedCache.GetAsync("some_key_1")).Should().BeNullOrEmpty();
        }

        [Test]
        public async Task CanGetAsyncFromCacheAndWillTrackIfRemoved()
        {
            await _distributedCache.SetAsync("some_key_2", Encoding.UTF8.GetBytes("2"));
            await _staticCacheManager.GetAsync(new CacheKey("some_key_2"), () => 2);
            await _staticCacheManager.RemoveByPrefixAsync("some_key_2");
            (await _distributedCache.GetAsync("some_key_2")).Should().BeNullOrEmpty();
        }

        [Test]
        public async Task CanGetFromCacheAndWillTrackRemoved()
        {
            await _distributedCache.SetAsync("some_key_3", Encoding.UTF8.GetBytes("3"));
            _staticCacheManager.Get(new CacheKey("some_key_3"), () => 3);
            await _staticCacheManager.RemoveByPrefixAsync("some_key_3");
            (await _distributedCache.GetAsync("some_key_3")).Should().BeNullOrEmpty();
        }

        [Test]
        [Ignore("Doesn't work for current in memory implementation")]
        public async Task CanClearCache()
        {
            await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 1);
            await _staticCacheManager.SetAsync(new CacheKey("some_key_2"), 2);
            await _staticCacheManager.SetAsync(new CacheKey("some_key_3"), 3);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var manager = GetService<MemoryDistributedCacheManager>(scope);
                manager.Equals(_staticCacheManager).Should().BeFalse();
                _staticCacheManager.Get<int?>(new CacheKey("some_key_1"), () => null).Should().Be(1);
                _staticCacheManager.Get<int?>(new CacheKey("some_key_2"), () => null).Should().Be(2);
                _staticCacheManager.Get<int?>(new CacheKey("some_key_3"), () => null).Should().Be(3);
                await manager.ClearAsync();
            }

            _staticCacheManager.Get<int?>(new CacheKey("some_key_1"), () => null).Should().BeNull();
            _staticCacheManager.Get<int?>(new CacheKey("some_key_2"), () => null).Should().BeNull();
            _staticCacheManager.Get<int?>(new CacheKey("some_key_3"), () => null).Should().BeNull();
        }

        [Test]
        public async Task CanGet()
        {
            await _distributedCache.SetAsync("some_key_1", Encoding.UTF8.GetBytes("1"));
            await _distributedCache.SetAsync("some_key_2", Encoding.UTF8.GetBytes("2"));
            await _distributedCache.SetAsync("some_key_3", Encoding.UTF8.GetBytes("3"));

            _staticCacheManager.Get<int?>(new CacheKey("some_key_1"), () => null).Should().Be(1);
            _staticCacheManager.Get<int?>(new CacheKey("some_key_2"), () => null).Should().Be(2);
            _staticCacheManager.Get<int?>(new CacheKey("some_key_3"), () => null).Should().Be(3);

            _staticCacheManager.Get<int?>(new CacheKey("some_key_4"), () => 4).Should().Be(4);

            var rez = await _staticCacheManager.GetAsync<int?>(new CacheKey("some_key_1"), () => 3);
            rez.Should().Be(1);
            rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"),
                async () => int.Parse(Encoding.UTF8.GetString(await _distributedCache.GetAsync("some_key_3"))));
            rez.Should().Be(1);
        }

        [Test]
        public async Task CanRemove()
        {
            await _distributedCache.SetAsync("some_key_1", Encoding.UTF8.GetBytes("1"));
            await _distributedCache.SetAsync("some_key_2", Encoding.UTF8.GetBytes("2"));
            await _staticCacheManager.SetAsync(new CacheKey("some_key_3"), "3");

            _staticCacheManager.Get<string>(new CacheKey("some_key_1"), () => null).Should().Be("1");
            _staticCacheManager.Get<string>(new CacheKey("some_key_2"), () => null).Should().Be("2");
            _staticCacheManager.Get<string>(new CacheKey("some_key_3"), () => null).Should().Be("3");

            await _staticCacheManager.RemoveAsync(new CacheKey("some_key_1"));
            await _staticCacheManager.RemoveAsync(new CacheKey("some_key_2"));
            await _staticCacheManager.RemoveAsync(new CacheKey("some_key_3"));

            _distributedCache.Get("some_key_1").Should().BeNullOrEmpty();
            _distributedCache.Get("some_key_2").Should().BeNullOrEmpty();
            _distributedCache.Get("some_key_3").Should().BeNullOrEmpty();
        }
    }
}