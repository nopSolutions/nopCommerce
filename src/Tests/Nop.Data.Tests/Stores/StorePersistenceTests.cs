using Nop.Core.Domain.Stores;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Stores
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
