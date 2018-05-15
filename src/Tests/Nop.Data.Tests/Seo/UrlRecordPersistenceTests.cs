using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Seo
{
    [TestFixture]
    public class UrlRecordPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_urlRecord()
        {
            var urlRecord = this.UrlRecord();

            var fromDb = SaveAndLoadEntity(this.UrlRecord());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(urlRecord);
        }
    }
}
