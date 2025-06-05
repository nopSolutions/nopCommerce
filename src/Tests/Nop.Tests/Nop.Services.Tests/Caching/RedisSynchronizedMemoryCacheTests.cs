using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Caching;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Caching;

[TestFixture]
internal class RedisSynchronizedMemoryCacheTests : ServiceTest
{
    protected IStaticCacheManager CreateCacheInstance()
    {
        var appSettings = Singleton<AppSettings>.Instance;
        var cacheKeyManager = new CacheKeyManager(new ConcurrentTrie<byte>());
        var redisSynchronizedMemoryCache = new RedisSynchronizedMemoryCache(new MemoryCache(new MemoryCacheOptions()),
            new TestRedisConnectionWrapper(), cacheKeyManager, appSettings);

        return new MemoryCacheManager(appSettings, redisSynchronizedMemoryCache, cacheKeyManager);
    }

    protected IList<KeyValuePair<CacheKey, string>> _values;

    [OneTimeSetUp]
    public void SetUp()
    {
        _values = new List<KeyValuePair<CacheKey, string>>();

        for (var i = 1; i <= 5; i++)
            _values.Add(new(new CacheKey($"key_{i}"), i.ToString()));
    }

    [Test]
    public async Task CanClearCacheAsync()
    {
        var c1 = CreateCacheInstance();
        await c1.SetAsync(_values[0].Key, _values[0].Value);
        await c1.SetAsync(_values[2].Key, _values[2].Value);
        await c1.SetAsync(_values[4].Key, _values[4].Value);

        var c2 = CreateCacheInstance();
        await c2.SetAsync(_values[0].Key, _values[0].Value);
        await c2.SetAsync(_values[1].Key, _values[1].Value);
        await c2.SetAsync(_values[2].Key, _values[2].Value);

        var c3 = CreateCacheInstance();
        await c3.SetAsync(_values[4].Key, _values[0].Value);
        await c3.SetAsync(_values[3].Key, _values[1].Value);

        await c2.ClearAsync();

        (await c1.GetAsync(_values[4].Key)).Should().BeNull();
        (await c3.GetAsync(_values[4].Key)).Should().BeNull();
        (await c1.GetAsync(_values[3].Key)).Should().BeNull();
    }

    [Test]
    public async Task CanRemoveByPrefixAsync()
    {
        var c1 = CreateCacheInstance();
        await c1.SetAsync(new("key_1"), "1");
        await c1.SetAsync(new("key_12"), "12");
        await c1.SetAsync(new("key_11"), "11");
        await c1.SetAsync(new("key_111"), "111");
        await c1.SetAsync(new("key_2"), "2");
        await c1.SetAsync(new("key_22"), "22");

        var c2 = CreateCacheInstance();
        await c2.SetAsync(new("key_12"), "12");
        await c2.SetAsync(new("key_112"), "112");
        await c2.SetAsync(new("key_113"), "113");

        await c2.RemoveByPrefixAsync("key_11");

        (await c1.GetAsync(new("key_1"))).Should().NotBeNull();
        (await c1.GetAsync(new("key_11"))).Should().BeNull();
        (await c1.GetAsync(new("key_111"))).Should().BeNull();
        (await c1.GetAsync(new("key_2"))).Should().NotBeNull();
        (await c1.GetAsync(new("key_22"))).Should().NotBeNull();

        (await c2.GetAsync(new("key_12"))).Should().NotBeNull();

        await c1.RemoveByPrefixAsync("key_1");

        (await c2.GetAsync(new("key_12"))).Should().BeNull();
        (await c1.GetAsync(new("key_12"))).Should().BeNull();
        (await c1.GetAsync(new("key_1"))).Should().BeNull();
    }
}
