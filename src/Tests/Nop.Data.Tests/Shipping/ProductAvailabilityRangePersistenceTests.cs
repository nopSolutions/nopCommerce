using Nop.Core.Domain.Shipping;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Shipping
{
    [TestFixture]
    public class ProductAvailabilityRangePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productAvailabilityRange()
        {
            var productAvailabilityRange = new ProductAvailabilityRange
            {
                Name = "product availability range",
                DisplayOrder = 1
            };

            var fromDb = SaveAndLoadEntity(productAvailabilityRange);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("product availability range");
            fromDb.DisplayOrder.ShouldEqual(1);
        }
    }
}