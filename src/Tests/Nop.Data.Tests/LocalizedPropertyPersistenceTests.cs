using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Tests
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
                Language = new Language()
                {
                    Name = "English",
                    LanguageCulture = "en-Us",
                    FlagImageFileName = "us.png",
                    Published = true,
                    DisplayOrder = 1
                },
            };

            var fromDb = SaveAndLoadEntity(localizedProperty);
            fromDb.EntityId.ShouldEqual(1);
            fromDb.LocaleKeyGroup.ShouldEqual("LocaleKeyGroup 1");
            fromDb.LocaleKey.ShouldEqual("LocaleKey 1");
            fromDb.LocaleValue.ShouldEqual("LocaleValue 1");
            
            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }
    }
}
