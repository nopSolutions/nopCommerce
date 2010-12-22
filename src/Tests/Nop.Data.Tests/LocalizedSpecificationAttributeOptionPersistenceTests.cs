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
    public class LocalizedSpecificationAttributeOptionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_localizedSpecificationAttributeOption()
        {
            var localizedSpecificationAttributeOption = new LocalizedSpecificationAttributeOption()
            {
                Name = "SpecificationAttributeOption name 1 localized",
                SpecificationAttributeOption = new SpecificationAttributeOption
                {
                    Name = "SpecificationAttributeOption name 1",
                    DisplayOrder = 1,
                    SpecificationAttribute = new SpecificationAttribute()
                    {
                        Name = "SpecificationAttribute name 1",
                        DisplayOrder = 2,
                    }
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

            var fromDb = SaveAndLoadEntity(localizedSpecificationAttributeOption);
            fromDb.Name.ShouldEqual("SpecificationAttributeOption name 1 localized");

            fromDb.SpecificationAttributeOption.ShouldNotBeNull();
            fromDb.SpecificationAttributeOption.Name.ShouldEqual("SpecificationAttributeOption name 1");

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }
    }
}
