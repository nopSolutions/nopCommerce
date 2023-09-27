using FluentAssertions;
using Nop.Core.Caching;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Caching
{
    [TestFixture]
    public class MemoryCacheManagerTests : BaseNopTest
    {
        private MemoryCacheManager _staticCacheManager;

        [OneTimeSetUp]
        public void Setup()
        {
            _staticCacheManager = GetService<IStaticCacheManager>() as MemoryCacheManager;
        }

        [TearDown]
        public async Task TaskTearDown()
        {
            await _staticCacheManager.ClearAsync();
        }

        [Test]
        public async Task CanSetAndGetObjectFromCache()
        {
            await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 3);
            var rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => 0);
            rez.Should().Be(3);
        }

        [Test]
        public async Task DoesNotIgnoreKeyCase()
        {
            await _staticCacheManager.SetAsync(new CacheKey("Some_Key_1"), 3);
            var rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => 0);
            rez.Should().Be(0);
        }

        [Test]
        public async Task CanValidateWhetherObjectIsCached()
        {
            await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 3);
            await _staticCacheManager.SetAsync(new CacheKey("some_key_2"), 4);

            var rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => 2);
            rez.Should().Be(3);
            rez = await _staticCacheManager.GetAsync(new CacheKey("some_key_2"), () => 2);
            rez.Should().Be(4);
        }

        [Test]
        public async Task CanClearCache()
        {
            await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 3);

            await _staticCacheManager.ClearAsync();

            var rez = await _staticCacheManager.GetAsync<object>(new CacheKey("some_key_1"));
            rez.Should().BeNull();
        }

        [Test]
        public async Task GetReturnsValueIfSet()
        {
            var key = new CacheKey("some_key_1");
            await _staticCacheManager.SetAsync(key, 3);
            var res = await _staticCacheManager.GetAsync<int>(key);
            res.Should().Be(3);
        }

        [Test]
        public async Task GetReturnsDefaultIfNotSet()
        {
            var key = new CacheKey("some_key_1");
            var res = await _staticCacheManager.GetAsync(key, 1);
            res.Should().Be(1);
            res = await _staticCacheManager.GetAsync<int>(key);
            res.Should().Be(0);
        }

        [Test]
        public async Task CanRemoveByPrefix()
        {
            await _staticCacheManager.SetAsync(new CacheKey("some_key_1"), 1);
            await _staticCacheManager.SetAsync(new CacheKey("some_key_2"), 2);
            await _staticCacheManager.SetAsync(new CacheKey("some_other_key"), 3);

            await _staticCacheManager.RemoveByPrefixAsync("some_key");

            var result = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), 0);
            result.Should().Be(0);
            result = await _staticCacheManager.GetAsync(new CacheKey("some_key_2"), 0);
            result.Should().Be(0);
            result = await _staticCacheManager.GetAsync(new CacheKey("some_other_key"), 0);
            result.Should().Be(3);
        }

        [Test]
        public async Task ExecutesSetInOrder()
        {
            await Task.WhenAll(Enumerable.Range(1, 5).Select(i => _staticCacheManager.SetAsync(new CacheKey("some_key_1"), i)));
            var value = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), 0);
            value.Should().Be(5);
        }

        [Test]
        public async Task GetsLazily()
        {
            var xs = new int[5];
            await Task.WhenAll(xs.Select((_, i) => _staticCacheManager.GetAsync(
                new CacheKey("some_key_1"),
                async () =>
                {
                    xs[i] = 1;
                    await Task.Delay(10);
                    return i;
                })));
            var value = await _staticCacheManager.GetAsync(new CacheKey("some_key_1"), () => Task.FromResult(-1));
            value.Should().Be(0);
            xs.Sum().Should().Be(1);
        }

        [Test]
        public async Task SholThrowsExceptionButNotCacheIt()
        {
            var cacheKey = new CacheKey("some_key_1");

            Assert.ThrowsAsync<ApplicationException>(() => _staticCacheManager.GetAsync(
                cacheKey,
                Task<object> () => throw new ApplicationException()));

            //should not cache exception
            var rez = await _staticCacheManager.GetAsync(cacheKey, Task<object> () => Task.FromResult((object)1));
            rez.Should().Be(1);

            await _staticCacheManager.RemoveAsync(cacheKey);

            Assert.ThrowsAsync<ApplicationException>(() => _staticCacheManager.GetAsync(
                cacheKey,
                Task<object> () => throw new ApplicationException()));

            //should not cache exception
            rez = await _staticCacheManager.GetAsync(cacheKey, (object)1);
            rez.Should().Be(1);

            await _staticCacheManager.RemoveAsync(cacheKey);

            Assert.ThrowsAsync<ApplicationException>(() => _staticCacheManager.GetAsync<object>(
                cacheKey,
                () => throw new ApplicationException()));

            //should not cache exception
            rez = await _staticCacheManager.GetAsync(cacheKey, () => (object)1);
            rez.Should().Be(1);

            await _staticCacheManager.RemoveAsync(cacheKey);

            Assert.ThrowsAsync<ApplicationException>(() => _staticCacheManager.GetAsync<object>(
                cacheKey,
                () => throw new ApplicationException()));

            //should not cache exception
            rez = await _staticCacheManager.GetAsync(cacheKey);
            rez.Should().BeNull();
        }

        [Test]
        public async Task CanGetAsObject()
        {
            var key = new CacheKey("some_key_1");
            await _staticCacheManager.SetAsync(key, 1);
            var obj = await _staticCacheManager.GetAsync(key);
            obj.Should().Be(1);
            obj = await _staticCacheManager.GetAsync(new CacheKey("some_key_2"));
            obj.Should().BeNull();
        }
    }
}
