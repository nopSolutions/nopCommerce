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
    public class LocalizedProductPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_localizedProduct()
        {
            var localizedProduct = new LocalizedProduct
            {
                Name = "Name localized",
                ShortDescription = "ShortDescription 1 localized",
                FullDescription = "FullDescription 1 localized",
                MetaKeywords = "Meta keywords localized",
                MetaDescription = "Meta description localized",
                MetaTitle = "Meta title localized",
                SeName = "SE name localized",
                Product = new Product
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
                    UpdatedOnUtc = new DateTime(2010, 01, 02),
                },
                Language = new Language()
                {
                    Name = "English",
                    LanguageCulture = "en-Us",
                    FlagImageFileName = "us.png",
                    Published = true,
                    DisplayOrder = 1
                }

            };

            var fromDb = SaveAndLoadEntity(localizedProduct);
            fromDb.Name.ShouldEqual("Name localized");
            fromDb.ShortDescription.ShouldEqual("ShortDescription 1 localized");
            fromDb.FullDescription.ShouldEqual("FullDescription 1 localized");
            fromDb.MetaKeywords.ShouldEqual("Meta keywords localized");
            fromDb.MetaDescription.ShouldEqual("Meta description localized");
            fromDb.SeName.ShouldEqual("SE name localized");

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Name 1");

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }
    }
}
