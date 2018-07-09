using Nop.Core.Domain.Stores;
using Nop.Services.Stores;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Stores
{
    [TestFixture]
    public class StoreExtensionsTests : ServiceTest
    {
        private IStoreService _storeService;

        [SetUp]
        public new void SetUp()
        {
            _storeService = new StoreService(null, null, null);
        }

        [Test]
        public void Can_parse_host_values()
        {
            var store = new Store
            {
                Hosts = "yourstore.com, www.yourstore.com, "
            };

            var hosts = _storeService.ParseHostValues(store);
            hosts.Length.ShouldEqual(2);
            hosts[0].ShouldEqual("yourstore.com");
            hosts[1].ShouldEqual("www.yourstore.com");
        }

        [Test]
        public void Can_find_host_value()
        {
            var store = new Store
            {
                Hosts = "yourstore.com, www.yourstore.com, "
            };

            _storeService.ContainsHostValue(store, null).ShouldEqual(false);
            _storeService.ContainsHostValue(store, "").ShouldEqual(false);
            _storeService.ContainsHostValue(store, "store.com").ShouldEqual(false);
            _storeService.ContainsHostValue(store, "yourstore.com").ShouldEqual(true);
            _storeService.ContainsHostValue(store, "yoursTore.com").ShouldEqual(true);
            _storeService.ContainsHostValue(store, "www.yourstore.com").ShouldEqual(true);
        }
    }
}
