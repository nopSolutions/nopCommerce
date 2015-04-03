using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class PredefinedProductAttributeValuePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_predefinedProductAttributeValue()
        {
            var pav = new PredefinedProductAttributeValue
            {
                Name = "Name 1",
                PriceAdjustment = 1.1M,
                WeightAdjustment = 2.1M,
                Cost = 3.1M,
                IsPreSelected = true,
                DisplayOrder = 3,
                ProductAttribute = new ProductAttribute
                {
                    Name = "Name 1"
                }
            };

            var fromDb = SaveAndLoadEntity(pav);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.PriceAdjustment.ShouldEqual(1.1M);
            fromDb.WeightAdjustment.ShouldEqual(2.1M);
            fromDb.Cost.ShouldEqual(3.1M);
            fromDb.IsPreSelected.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(3);

            fromDb.ProductAttribute.ShouldNotBeNull();
            fromDb.ProductAttribute.Name.ShouldEqual("Name 1");
        }
    }
}
