using System;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ManufacturerPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_manufacturer()
        {
            var manufacturer = new Manufacturer
            {
                Name = "Name",
                Description = "Description 1",
                ManufacturerTemplateId = 1,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                PictureId = 3,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                PriceRanges = "1-3;",
                Published = true,
                SubjectToAcl = true,
                LimitedToStores = true, 
                Deleted = false,
                DisplayOrder = 5,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            var fromDb = SaveAndLoadEntity(manufacturer);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name");
            fromDb.Description.ShouldEqual("Description 1");
            fromDb.ManufacturerTemplateId.ShouldEqual(1);
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.PictureId.ShouldEqual(3);
            fromDb.PageSize.ShouldEqual(4);
            fromDb.AllowCustomersToSelectPageSize.ShouldEqual(true);
            fromDb.PageSizeOptions.ShouldEqual("4, 2, 8, 12");
            fromDb.PriceRanges.ShouldEqual("1-3;");
            fromDb.Published.ShouldEqual(true);
            fromDb.SubjectToAcl.ShouldEqual(true);
            fromDb.LimitedToStores.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.DisplayOrder.ShouldEqual(5);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }
    }
}
