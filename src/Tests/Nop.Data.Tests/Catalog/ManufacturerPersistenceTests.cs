using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ManufacturerPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_manufacturer()
        {
            var manufacturer = this.GetTestManufacturer();

            var fromDb = SaveAndLoadEntity(this.GetTestManufacturer());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(manufacturer);
        }        
    }
}
