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
            var localizedProperty = this.GetTestLocalizedProperty();
            localizedProperty.Language = this.GetTestLanguage();

            var fromDb = SaveAndLoadEntity(localizedProperty);
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(this.GetTestLocalizedProperty());
            
            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.PropertiesShouldEqual(this.GetTestLanguage());
        }
    }
}
