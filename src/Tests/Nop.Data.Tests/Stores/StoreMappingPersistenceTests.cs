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
            var storeMapping = this.GetTestStoreMapping();
            storeMapping.Store = this.GetTestStore();

            var fromDb = SaveAndLoadEntity(storeMapping);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestStoreMapping());

            fromDb.Store.ShouldNotBeNull();
            fromDb.Store.PropertiesShouldEqual(this.GetTestStore());
        }
    }
}
