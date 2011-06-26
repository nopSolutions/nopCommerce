using System;
using System.Linq;
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
                                   MetaKeywords = "Meta keywords",
                                   MetaDescription = "Meta description",
                                   MetaTitle = "Meta title",
                                   SeName = "SE name",
                                   ParentCategoryId = 2,
                                   PictureId = 3,
                                   PageSize = 4,
                                   PriceRanges = "1-3;",
                                   ShowOnHomePage = false,
                                   Published = true,
                                   Deleted = false,
                                   DisplayOrder = 5,
                                   CreatedOnUtc = new DateTime(2010, 01, 01),
                                   UpdatedOnUtc = new DateTime(2010, 01, 02),
                               };

            var fromDb = SaveAndLoadEntity(category);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Books");
            fromDb.Description.ShouldEqual("Description 1");
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

        [Test]
        public void Can_save_and_load_category_with_productCategories()
        {
            var category = new Category
                               {
                                   Name = "Books",
                                   Description = "Description 1",
                                   MetaKeywords = "Meta keywords",
                                   MetaDescription = "Meta description",
                                   MetaTitle = "Meta title",
                                   SeName = "SE name",
                                   ParentCategoryId = 2,
                                   PictureId = 3,
                                   PageSize = 4,
                                   PriceRanges = "1-3;",
                                   ShowOnHomePage = false,
                                   Published = true,
                                   Deleted = false,
                                   DisplayOrder = 5,
                                   CreatedOnUtc = new DateTime(2010, 01, 01),
                                   UpdatedOnUtc = new DateTime(2010, 01, 02)
                               };
            category.ProductCategories.Add
                (
                    new ProductCategory
                    {
                        IsFeaturedProduct = true,
                        DisplayOrder = 1,
                        Product = new Product()
                        {
                            Name = "Name 1",
                            Published = true,
                            Deleted = false,
                            CreatedOnUtc = new DateTime(2010, 01, 01),
                            UpdatedOnUtc = new DateTime(2010, 01, 02)
                        }
                    }
                );
            var fromDb = SaveAndLoadEntity(category);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Books");

            fromDb.ProductCategories.ShouldNotBeNull();
            (fromDb.ProductCategories.Count == 1).ShouldBeTrue();
            fromDb.ProductCategories.First().IsFeaturedProduct.ShouldEqual(true);
        }
    }
}