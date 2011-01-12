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
    public class ProductManufacturerPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productManufacturer()
        {
            var productManufacturer = new ProductManufacturer
                                     {
                                         IsFeaturedProduct = true,
                                         DisplayOrder = 1,
                                         Product = new Product()
                                                       {
                                                           Name = "Name 1",
                                                           ShortDescription = "ShortDescription 1",
                                                           FullDescription = "FullDescription 1",
                                                           AdminComment = "AdminComment 1",
                                                           TemplateId = 1,
                                                           ShowOnHomePage = false,
                                                           MetaKeywords = "Meta keywords",
                                                           MetaDescription = "Meta description",
                                                           MetaTitle = "Meta title",
                                                           SeName = "SE name",
                                                           AllowCustomerReviews = true,
                                                           AllowCustomerRatings = true,
                                                           RatingSum = 2,
                                                           TotalRatingVotes = 3,
                                                           Published = true,
                                                           Deleted = false,
                                                           CreatedOnUtc = new DateTime(2010, 01, 01),
                                                           UpdatedOnUtc = new DateTime(2010, 01, 02)
                                                       },
                                         Manufacturer = new Manufacturer()
                                                                      {
                                                                          Name = "Name",
                                                                          Description = "Description 1",
                                                                          TemplateId = 1,
                                                                          MetaKeywords = "Meta keywords",
                                                                          MetaDescription = "Meta description",
                                                                          MetaTitle = "Meta title",
                                                                          SeName = "SE name",
                                                                          PictureId = 3,
                                                                          PageSize = 4,
                                                                          PriceRanges = "1-3;",
                                                                          Published = true,
                                                                          Deleted = false,
                                                                          DisplayOrder = 5,
                                                                          CreatedOnUtc = new DateTime(2010, 01, 01),
                                                                          UpdatedOnUtc = new DateTime(2010, 01, 02),
                                                                      }
                                     };

            var fromDb = SaveAndLoadEntity(productManufacturer);
            fromDb.ShouldNotBeNull();
            fromDb.IsFeaturedProduct.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Name 1");

            fromDb.Manufacturer.ShouldNotBeNull();
            fromDb.Manufacturer.Name.ShouldEqual("Name");
        }
    }
}
