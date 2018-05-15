using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Tax
{
    [TestFixture]
    public class TaxCategoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_taxCategory()
        {
            var taxCategory = this.GetTestTaxCategory();

            var fromDb = SaveAndLoadEntity(this.GetTestTaxCategory());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(taxCategory);
        }
    }
}