using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Localization
{
    [TestFixture]
    public class LocaleStringResourcePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_lst()
        {
            var lst = this.GetTestLocaleStringResource();

            lst.Language = this.GetTestLanguage();

            var fromDb = SaveAndLoadEntity(lst);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestLocaleStringResource());

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.PropertiesShouldEqual(this.GetTestLanguage());
        }
    }
}
