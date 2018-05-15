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
            var store = this.GetTestStore();

            var fromDb = SaveAndLoadEntity(this.GetTestStore());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(store);
        }
    }
}
