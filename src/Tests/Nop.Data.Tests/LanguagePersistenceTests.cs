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
    public class LanguagePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_language()
        {
            var lang = new Language
            {
                Name = "English",
                LanguageCulture = "en-Us",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };

            var fromDb = SaveAndLoadEntity(lang);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("English");
            fromDb.LanguageCulture.ShouldEqual("en-Us");
            fromDb.FlagImageFileName.ShouldEqual("us.png");
            fromDb.Published.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);
        }

        [Test]
        public void Can_save_and_load_language_with_localeStringResources()
        {
            var lang = new Language
                           {
                               Name = "English",
                               LanguageCulture = "en-Us",
                               FlagImageFileName = "us.png",
                               Published = true,
                               DisplayOrder = 1,
                               LocaleStringResources = new List<LocaleStringResource>()
                                                           {
                                                               new LocaleStringResource()
                                                                   {
                                                                       ResourceName = "ResourceName1",
                                                                       ResourceValue = "ResourceValue2"
                                                                   }
                                                           }
                           };

            var fromDb = SaveAndLoadEntity(lang);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("English");

            fromDb.LocaleStringResources.ShouldNotBeNull();
            (fromDb.LocaleStringResources.Count == 1).ShouldBeTrue();
            fromDb.LocaleStringResources.First().ResourceName.ShouldEqual("ResourceName1");
        }
    }
}
