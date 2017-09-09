using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductWarehouseInventoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productWarehouseInventory()
        {
            var pwi = this.GetTestProductWarehouseInventory();
            pwi.Product = this.GetTestProduct();
            var fromDb = SaveAndLoadEntity(pwi);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestProductWarehouseInventory());

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.PropertiesShouldEqual(this.GetTestProduct());

            fromDb.Warehouse.ShouldNotBeNull();
            fromDb.Warehouse.PropertiesShouldEqual(this.GetTestWarehouse());
        }
    }
}
