using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class LocalizedManufacturerPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_localizedManufacturer()
        {
            var localizedManufacturer = new LocalizedManufacturer
            {
                Name = "Name localized",
                Description = "Description 1 localized",
                MetaKeywords = "Meta keywords localized",
                MetaDescription = "Meta description localized",
                MetaTitle = "Meta title localized",
                SeName = "SE name localized",
                Manufacturer = new Manufacturer
                {
                    Name = "Name",
                    Description = "Description 1",
                    TemplateId = 1,
                    MetaKeywords = "Meta keywords",
                    MetaDescription = "Meta description",
                    MetaTitle = "Meta title",
                    SeName = "SE name",
                    PictureId = 3,
                    PageSize = 4,
                    PriceRanges = "1-3;",
                    Published = true,
                    Deleted = false,
                    DisplayOrder = 5,
                    CreatedOnUtc = new DateTime(2010, 01, 01),
                    UpdatedOnUtc = new DateTime(2010, 01, 02),
                },
                Language = new Language()
                {
                    Name = "English",
                    LanguageCulture = "en-Us",
                    FlagImageFileName = "us.png",
                    Published = true,
                    DisplayOrder = 1
                }

            };

            var fromDb = SaveAndLoadEntity(localizedManufacturer);
            fromDb.Name.ShouldEqual("Name localized");
            fromDb.Description.ShouldEqual("Description 1 localized");
            fromDb.MetaKeywords.ShouldEqual("Meta keywords localized");
            fromDb.MetaDescription.ShouldEqual("Meta description localized");
            fromDb.SeName.ShouldEqual("SE name localized");

            fromDb.Manufacturer.ShouldNotBeNull();
            fromDb.Manufacturer.Name.ShouldEqual("Name");

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }
    }
}
