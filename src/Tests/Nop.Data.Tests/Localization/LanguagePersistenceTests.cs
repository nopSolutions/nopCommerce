using System.Linq;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Localization
{
    [TestFixture]
    public class LanguagePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_language()
        {
            var lang = this.GetTestLanguage();

            var fromDb = SaveAndLoadEntity(this.GetTestLanguage());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(lang);
        }
    }
}
