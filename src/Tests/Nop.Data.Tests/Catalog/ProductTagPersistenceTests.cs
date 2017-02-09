using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductTagPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productTag()
        {
            var productTag = this.GetTestProductTag();

            var fromDb = SaveAndLoadEntity(this.GetTestProductTag());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(productTag);
        }
    }
}