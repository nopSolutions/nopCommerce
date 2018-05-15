using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductManufacturerPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productManufacturer()
        {
            var productManufacturer = this.GetTestProductManufacturer();
            productManufacturer.Product = this.GetTestProduct();
            productManufacturer.Manufacturer = this.GetTestManufacturer();

            var fromDb = SaveAndLoadEntity(productManufacturer);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestProductManufacturer());

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.PropertiesShouldEqual(this.GetTestProduct());

            fromDb.Manufacturer.ShouldNotBeNull();
            fromDb.Manufacturer.PropertiesShouldEqual(this.GetTestManufacturer());
        }
    }
}
