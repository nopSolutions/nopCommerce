using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductAttributeCombinationPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productAttributeCombination()
        {
            var combination = this.GetTestProductAttributeCombination();

            var fromDb = SaveAndLoadEntity(this.GetTestProductAttributeCombination());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(combination);
            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.PropertiesShouldEqual(combination.Product);
        }        
    }
}