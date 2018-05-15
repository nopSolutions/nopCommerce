using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Media
{
    [TestFixture]
    public class DownloadPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_download()
        {
            var download = this.GetTestDownload();

            var fromDb = SaveAndLoadEntity(this.GetTestDownload());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(download);
        }
    }
}
