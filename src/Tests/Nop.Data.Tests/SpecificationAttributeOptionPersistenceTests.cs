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
    public class SpecificationAttributeOptionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_specificationAttributeOption()
        {
            var specificationAttributeOption = new SpecificationAttributeOption
            {
                Name = "SpecificationAttributeOption name 1",
                DisplayOrder = 1,
                SpecificationAttribute = new SpecificationAttribute()
                {
                    Name = "SpecificationAttribute name 1",
                    DisplayOrder = 2,
                }
            };

            var fromDb = SaveAndLoadEntity(specificationAttributeOption);
            fromDb.Name.ShouldEqual("SpecificationAttributeOption name 1");
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.SpecificationAttribute.ShouldNotBeNull();
            fromDb.SpecificationAttribute.Name.ShouldEqual("SpecificationAttribute name 1");
        }

        [Test]
        public void Can_save_and_load_specificationAttributeOption_with_localizedSpecificationAttributeOptions()
        {
            var lang = new Language()
            {
                Name = "English",
                LanguageCulture = "en-Us",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };

            var specificationAttributeOption = new SpecificationAttributeOption
            {
                Name = "SpecificationAttributeOption name 1",
                DisplayOrder = 1,
                SpecificationAttribute = new SpecificationAttribute()
                {
                    Name = "SpecificationAttribute name 1",
                    DisplayOrder = 2,
                },
                LocalizedSpecificationAttributeOptions = new List<LocalizedSpecificationAttributeOption>()
                {
                    new LocalizedSpecificationAttributeOption
                    {
                        Name = "SpecificationAttributeOption name 1 localized",
                        Language = lang
                    },
                }
            };

            var fromDb = SaveAndLoadEntity(specificationAttributeOption);
            fromDb.Name.ShouldEqual("SpecificationAttributeOption name 1");

            fromDb.LocalizedSpecificationAttributeOptions.ShouldNotBeNull();
            (fromDb.LocalizedSpecificationAttributeOptions.Count == 1).ShouldBeTrue();
            fromDb.LocalizedSpecificationAttributeOptions.First().Name.ShouldEqual("SpecificationAttributeOption name 1 localized");
        }
    }
}
