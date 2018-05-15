using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Media
{
    [TestFixture]
    public class PicturePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_picture()
        {
            var picture = this.GetTestPicture();

            var fromDb = SaveAndLoadEntity(this.GetTestPicture());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(picture);
        }
    }
}
