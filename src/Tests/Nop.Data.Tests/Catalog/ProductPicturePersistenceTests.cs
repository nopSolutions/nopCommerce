using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductPicturePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productPicture()
        {
            var productPicture = this.GetTestProductPicture();
            productPicture.Product = this.GetTestProduct();
            var fromDb = SaveAndLoadEntity(productPicture);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestProductPicture());

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.PropertiesShouldEqual(this.GetTestProduct());

            fromDb.Picture.ShouldNotBeNull();
            fromDb.Picture.PropertiesShouldEqual(this.GetTestPicture());
        }
    }    
}
