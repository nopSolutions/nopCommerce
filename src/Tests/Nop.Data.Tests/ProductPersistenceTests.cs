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
    public class ProductPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_product()
        {
            var product = new Product
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
                RatingSum =2,
                TotalRatingVotes = 3,
                Published = true,
                Deleted= false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            var fromDb = SaveAndLoadEntity(product);
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.ShortDescription.ShouldEqual("ShortDescription 1");
            fromDb.FullDescription.ShouldEqual("FullDescription 1");
            fromDb.AdminComment.ShouldEqual("AdminComment 1");
            fromDb.TemplateId.ShouldEqual(1);
            fromDb.ShowOnHomePage.ShouldEqual(false);
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.SeName.ShouldEqual("SE name");
            fromDb.AllowCustomerReviews.ShouldEqual(true);
            fromDb.AllowCustomerRatings.ShouldEqual(true);
            fromDb.RatingSum.ShouldEqual(2);
            fromDb.TotalRatingVotes.ShouldEqual(3);
            fromDb.Published.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }
    }
}
