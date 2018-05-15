using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ManufacturerTemplatePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_manufacturerTemplate()
        {
            var manufacturerTemplate = this.GetTestManufacturerTemplate();

            var fromDb = SaveAndLoadEntity(this.GetTestManufacturerTemplate());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(manufacturerTemplate);
        }        
    }
}
