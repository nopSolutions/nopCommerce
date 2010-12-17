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
    public class CategoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_category()
        {
            var category = new Category
            {
                Name = "Books",
                Description = "Description 1",
                TemplateId = 1,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                SeName = "SE name",
                ParentCategoryId =2,
                PictureId= 3,
                PageSize=4,
                PriceRanges = "1-3;",
                ShowOnHomePage= false,
                Published = true,
                Deleted= false,
                DisplayOrder = 5,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            var fromDb = SaveAndLoadEntity(category);
            fromDb.Name.ShouldEqual("Books");
            fromDb.Description.ShouldEqual("Description 1");
            fromDb.TemplateId.ShouldEqual(1);
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.SeName.ShouldEqual("SE name");
            fromDb.ParentCategoryId.ShouldEqual(2);
            fromDb.PictureId.ShouldEqual(3);
            fromDb.PageSize.ShouldEqual(4);
            fromDb.PriceRanges.ShouldEqual("1-3;");
            fromDb.ShowOnHomePage.ShouldEqual(false);
            fromDb.Published.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.DisplayOrder.ShouldEqual(5);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }
    }
}
