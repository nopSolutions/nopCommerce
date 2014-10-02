using Nop.Core.Domain.Localization;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Localization
{
    [TestFixture]
    public class LocalizedPropertyPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_localizedProperty()
        {
            var localizedProperty = new LocalizedProperty
            {
                EntityId = 1,
                LocaleKeyGroup = "LocaleKeyGroup 1",
                LocaleKey = "LocaleKey 1",
                LocaleValue = "LocaleValue 1",
                Language = new Language
                {
                    Name = "English",
                    LanguageCulture = "en-Us",
                    FlagImageFileName = "us.png",
                    Published = true,
                    DisplayOrder = 1
                },
            };

            var fromDb = SaveAndLoadEntity(localizedProperty);
            fromDb.ShouldNotBeNull();
            fromDb.EntityId.ShouldEqual(1);
            fromDb.LocaleKeyGroup.ShouldEqual("LocaleKeyGroup 1");
            fromDb.LocaleKey.ShouldEqual("LocaleKey 1");
            fromDb.LocaleValue.ShouldEqual("LocaleValue 1");
            
            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }
    }
}
