using FluentAssertions;
using Nop.Core.Domain.Stores;
using Nop.Services.Stores;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Stores
{
    [TestFixture]
    public class StoreExtensionsTests : ServiceTest
    {
        private IStoreService _storeService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _storeService = GetService<IStoreService>();
        }

        [Test]
        public void CanFindHostValue()
        {
            var store = new Store
            {
                Hosts = "yourstore.com, www.yourstore.com, "
            };

            _storeService.ContainsHostValue(store, null).Should().BeFalse();
            _storeService.ContainsHostValue(store, string.Empty).Should().BeFalse();
            _storeService.ContainsHostValue(store, "store.com").Should().BeFalse();
            _storeService.ContainsHostValue(store, "yourstore.com").Should().BeTrue();
            _storeService.ContainsHostValue(store, "yoursTore.com").Should().BeTrue();
            _storeService.ContainsHostValue(store, "www.yourstore.com").Should().BeTrue();
        }
    }
}
