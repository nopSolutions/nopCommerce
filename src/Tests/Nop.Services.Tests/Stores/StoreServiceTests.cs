using FluentAssertions;
using Nop.Core.Domain.Stores;
using Nop.Services.Stores;
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
            _storeService = new StoreService(null, null);
        }

        [Test]
        public void Can_parse_host_values()
        {
            var store = new Store
            {
                Hosts = "yourstore.com, www.yourstore.com, "
            };

            var hosts = _storeService.ParseHostValues(store);
            hosts.Length.Should().Be(2);
            hosts[0].Should().Be("yourstore.com");
            hosts[1].Should().Be("www.yourstore.com");
        }

        [Test]
        public void Can_find_host_value()
        {
            var store = new Store
            {
                Hosts = "yourstore.com, www.yourstore.com, "
            };

            _storeService.ContainsHostValue(store, null).Should().BeFalse();
            _storeService.ContainsHostValue(store, "").Should().BeFalse();
            _storeService.ContainsHostValue(store, "store.com").Should().BeFalse();
            _storeService.ContainsHostValue(store, "yourstore.com").Should().BeTrue();
            _storeService.ContainsHostValue(store, "yoursTore.com").Should().BeTrue();
            _storeService.ContainsHostValue(store, "www.yourstore.com").Should().BeTrue();
        }
    }
}
