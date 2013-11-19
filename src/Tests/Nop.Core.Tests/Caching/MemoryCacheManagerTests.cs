using Nop.Core.Caching;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Caching
{
    [TestFixture]
    public class MemoryCacheManagerTests
    {
        [Test]
        public void Can_set_and_get_object_from_cache()
        {
            var cacheManager = new MemoryCacheManager();
            cacheManager.Set("some_key_1", 3, int.MaxValue);

            cacheManager.Get<int>("some_key_1").ShouldEqual(3);
        }

        [Test]
        public void Can_validate_whetherobject_is_cached()
        {
            var cacheManager = new MemoryCacheManager();
            cacheManager.Set("some_key_1", 3, int.MaxValue);
            cacheManager.Set("some_key_2", 4, int.MaxValue);

            cacheManager.IsSet("some_key_1").ShouldEqual(true);
            cacheManager.IsSet("some_key_3").ShouldEqual(false);
        }

        [Test]
        public void Can_clear_cache()
        {
            var cacheManager = new MemoryCacheManager();
            cacheManager.Set("some_key_1", 3, int.MaxValue);

            cacheManager.Clear();

            cacheManager.IsSet("some_key_1").ShouldEqual(false);
        }
    }
}
