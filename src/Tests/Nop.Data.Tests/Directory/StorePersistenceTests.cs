using System.Linq;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Directory
{
    [TestFixture]
    public class StorePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_store()
        {
            var store = new Store
            {
                Name = "Computer store",
                DisplayOrder = 1
            };

            var fromDb = SaveAndLoadEntity(store);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Computer store");
            fromDb.DisplayOrder.ShouldEqual(1);
        }
    }
}
