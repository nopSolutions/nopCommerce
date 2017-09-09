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
            var productAvailabilityRange = this.GetTestProductAvailabilityRange();

            var fromDb = SaveAndLoadEntity(this.GetTestProductAvailabilityRange());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(productAvailabilityRange);
        }
    }
}