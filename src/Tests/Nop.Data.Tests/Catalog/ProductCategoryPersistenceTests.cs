using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductCategoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productCategory()
        {
            var productCategory = this.GetTestProductCategory();
            productCategory.Product = this.GetTestProduct();
            productCategory.Category = this.GetTestCategory();

            var fromDb = SaveAndLoadEntity(productCategory);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestProductCategory());

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.PropertiesShouldEqual(this.GetTestProduct());

            fromDb.Category.ShouldNotBeNull();
            fromDb.Category.PropertiesShouldEqual(this.GetTestCategory());
        }        
    }
}
