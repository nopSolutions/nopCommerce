using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class SpecificationAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_specificationAttribute()
        {
            var specificationAttribute = new SpecificationAttribute
            {
                Name = "Name 1",
                DisplayOrder = 1,
            };

            var fromDb = SaveAndLoadEntity(specificationAttribute);
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.DisplayOrder.ShouldEqual(1);
        }

        [Test]
        public void Can_save_and_load_specificationAttribute_with_specificationAttributeOptions()
        {
            var specificationAttribute = new SpecificationAttribute
            {
                Name = "Name 1",
                DisplayOrder = 1,
                SpecificationAttributeOptions = new List<SpecificationAttributeOption>()
                {
                    new SpecificationAttributeOption
                    {
                        Name = "SpecificationAttributeOption name 1",
                        DisplayOrder = 1,
                    }
                }
            };

            var fromDb = SaveAndLoadEntity(specificationAttribute);
            fromDb.Name.ShouldEqual("Name 1");


            fromDb.SpecificationAttributeOptions.ShouldNotBeNull();
            (fromDb.SpecificationAttributeOptions.Count == 1).ShouldBeTrue();
            fromDb.SpecificationAttributeOptions.First().Name.ShouldEqual("SpecificationAttributeOption name 1");
        }

        //[Test]
        //public void Can_save_and_load_specificationAttribute_with_localizedSpecificationAttributes()
        //{
        //    var lang = new Language()
        //               {
        //                   Name = "English",
        //                   LanguageCulture = "en-Us",
        //                   FlagImageFileName = "us.png",
        //                   Published = true,
        //                   DisplayOrder = 1
        //               };

        //    var specificationAttribute = new SpecificationAttribute
        //    {
        //        Name = "Name 1",
        //        DisplayOrder = 1,
        //        LocalizedSpecificationAttributes = new List<LocalizedSpecificationAttribute>()
        //        {
        //            new LocalizedSpecificationAttribute
        //            {
        //                Name = "Name localized 1",
        //                Language = lang
        //            }
        //        }
        //    };

        //    var fromDb = SaveAndLoadEntity(specificationAttribute);
        //    fromDb.Name.ShouldEqual("Name 1");

        //    fromDb.LocalizedSpecificationAttributes.ShouldNotBeNull();
        //    (fromDb.LocalizedSpecificationAttributes.Count == 1).ShouldBeTrue();
        //    fromDb.LocalizedSpecificationAttributes.First().Name.ShouldEqual("Name localized 1");
        //}
    }
}
