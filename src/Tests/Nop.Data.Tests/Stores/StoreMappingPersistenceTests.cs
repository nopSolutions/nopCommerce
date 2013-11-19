using Nop.Core.Domain.Stores;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Stores
{
    [TestFixture]
    public class StoreMappingPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_storeMapping()
        {
            var storeMapping = new StoreMapping
            {
                EntityId = 1,
                EntityName = "EntityName 1",
                Store = GetTestStore(),
            };

            var fromDb = SaveAndLoadEntity(storeMapping);
            fromDb.ShouldNotBeNull();
            fromDb.EntityId.ShouldEqual(1);
            fromDb.EntityName.ShouldEqual("EntityName 1");
            fromDb.Store.ShouldNotBeNull();
        }

        protected Store GetTestStore()
        {
            return new Store
            {
                Name = "Store 1",
                Url = "http://www.test.com",
                DisplayOrder = 1
            };
        }
    }
}
