using Nop.Core.Domain.Stores;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Stores
{
    [TestFixture]
    public class StoreExtensionsTests
    {
        [Test]
        public void Can_parse_host_values()
        {
            var store = new Store
            {
                Hosts = "yourstore.com, www.yourstore.com, "
            };

            var hosts = store.ParseHostValues();
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

            store.ContainsHostValue(null).ShouldEqual(false);
            store.ContainsHostValue("").ShouldEqual(false);
            store.ContainsHostValue("store.com").ShouldEqual(false);
            store.ContainsHostValue("yourstore.com").ShouldEqual(true);
            store.ContainsHostValue("yoursTore.com").ShouldEqual(true);
            store.ContainsHostValue("www.yourstore.com").ShouldEqual(true);
        }
    }
}
