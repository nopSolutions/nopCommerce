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
    public class LocalizedSpecificationAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_localizedSpecificationAttribute()
        {
            var localizedSpecificationAttribute = new LocalizedSpecificationAttribute
            {
                Name = "Name localized",
                SpecificationAttribute = new SpecificationAttribute
                {
                    Name = "Name 1",
                    DisplayOrder = 1,
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

            var fromDb = SaveAndLoadEntity(localizedSpecificationAttribute);
            fromDb.Name.ShouldEqual("Name localized");

            fromDb.SpecificationAttribute.ShouldNotBeNull();
            fromDb.SpecificationAttribute.Name.ShouldEqual("Name 1");

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }
    }
}
