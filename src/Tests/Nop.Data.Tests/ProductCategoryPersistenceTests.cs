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
    public class ProductCategoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productCategory()
        {
            var productCategory = new ProductCategory
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
                                                       },
                                                       Category = new Category()
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
                                                                      }
                                     };

            var fromDb = SaveAndLoadEntity(productCategory);
            fromDb.ShouldNotBeNull();
            fromDb.IsFeaturedProduct.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Name 1");

            fromDb.Category.ShouldNotBeNull();
            fromDb.Category.Name.ShouldEqual("Books");
        }
    }
}
