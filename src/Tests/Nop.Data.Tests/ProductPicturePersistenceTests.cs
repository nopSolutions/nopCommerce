using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class ProductPicturePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productPicture()
        {
            var productPicture = new ProductPicture
                                     {
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
                                         Picture = new Picture()
                                                                      {
                                                                          PictureBinary = new byte[] { 1, 2, 3 },
                                                                          MimeType = "image/pjpeg",
                                                                          IsNew = true
                                                                      }
                                     };

            var fromDb = SaveAndLoadEntity(productPicture);
            fromDb.ShouldNotBeNull();
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Name 1");

            fromDb.Picture.ShouldNotBeNull();
            fromDb.Picture.MimeType.ShouldEqual("image/pjpeg");
        }
    }
}
