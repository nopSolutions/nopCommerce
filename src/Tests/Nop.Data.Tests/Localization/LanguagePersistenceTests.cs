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

        [Test]
        public void Can_save_and_load_language_with_localeStringResources()
        {
            var lang = this.GetTestLanguage();
            lang.LocaleStringResources.Add(this.GetTestLocaleStringResource());
            var fromDb = SaveAndLoadEntity(lang);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestLanguage());

            fromDb.LocaleStringResources.ShouldNotBeNull();
            (fromDb.LocaleStringResources.Count == 1).ShouldBeTrue();
            fromDb.LocaleStringResources.First().PropertiesShouldEqual(this.GetTestLocaleStringResource());
        }
    }
}
