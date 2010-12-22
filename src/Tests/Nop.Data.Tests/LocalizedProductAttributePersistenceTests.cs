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
    public class LocalizedProductAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_localizedProductAttribute()
        {
            var lpa = new LocalizedProductAttribute
            {
                Name = "Name localized",
                Description = "Description 1 localized",
                ProductAttribute = new ProductAttribute()
                {
                    Name = "Name 1",
                    Description = "Description 1",
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

            var fromDb = SaveAndLoadEntity(lpa);
            fromDb.Name.ShouldEqual("Name localized");
            fromDb.Description.ShouldEqual("Description 1 localized");

            fromDb.ProductAttribute.ShouldNotBeNull();
            fromDb.ProductAttribute.Name.ShouldEqual("Name 1");

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }
    }
}
