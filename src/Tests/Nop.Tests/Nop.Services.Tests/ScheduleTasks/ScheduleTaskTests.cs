using FluentAssertions;
using Nop.Core.Caching;
using Nop.Services.Caching;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks;

[TestFixture]
public class ScheduleTaskTests : BaseNopTest
{
    private IStaticCacheManager _staticCacheManager;

    [OneTimeSetUp]
    public void SetUp()
    {
        _staticCacheManager = GetService<IStaticCacheManager>();
    }

    [Test]
    public async Task TestClearCacheTask()
    {
        var test = new ClearCacheTask(_staticCacheManager);

        var key = new CacheKey("test_key_1") { CacheTime = 30 };

        await _staticCacheManager.SetAsync(key, "test data");
        var data = await _staticCacheManager.GetAsync(key, () => string.Empty);
        data.Should().NotBeEmpty();
        await test.ExecuteAsync();
        data = await _staticCacheManager.GetAsync(key, () => string.Empty);
        data.Should().BeEmpty();
    }
}