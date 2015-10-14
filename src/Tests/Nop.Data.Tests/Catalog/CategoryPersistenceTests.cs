using System;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class CategoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_category()
        {
            var category = new Category
                               {
                                   Name = "Books",
                                   Description = "Description 1",
                                   CategoryTemplateId = 1,
                                   MetaKeywords = "Meta keywords",
                                   MetaDescription = "Meta description",
                                   MetaTitle = "Meta title",
                                   ParentCategoryId = 2,
                                   PictureId = 3,
                                   PageSize = 4,
                                   AllowCustomersToSelectPageSize = true,
                                   PageSizeOptions = "4, 2, 8, 12",
                                   PriceRanges = "1-3;",
                                   ShowOnHomePage = false,
                                   IncludeInTopMenu = true,
                                   Published = true,
                                   SubjectToAcl = true,
                                   LimitedToStores = true,
                                   Deleted = false,
                                   DisplayOrder = 5,
                                   CreatedOnUtc = new DateTime(2010, 01, 01),
                                   UpdatedOnUtc = new DateTime(2010, 01, 02),
                               };

            var fromDb = SaveAndLoadEntity(category);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Books");
            fromDb.Description.ShouldEqual("Description 1");
            fromDb.CategoryTemplateId.ShouldEqual(1);
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.ParentCategoryId.ShouldEqual(2);
            fromDb.PictureId.ShouldEqual(3);
            fromDb.PageSize.ShouldEqual(4);
            fromDb.AllowCustomersToSelectPageSize.ShouldEqual(true);
            fromDb.PageSizeOptions.ShouldEqual("4, 2, 8, 12");
            fromDb.PriceRanges.ShouldEqual("1-3;");
            fromDb.ShowOnHomePage.ShouldEqual(false);
            fromDb.IncludeInTopMenu.ShouldEqual(true);
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