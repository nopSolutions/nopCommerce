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
    public class ProductSpecificationAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productSpecificationAttribute()
        {
            var productSpecificationAttribute = new ProductSpecificationAttribute
            {
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                Product = new Product()
                {
                    Name = "Name 1",
                    ShortDescription = "ShortDescription 1",
                    FullDescription = "FullDescription 1",
                    AdminComment = "AdminComment 1",
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
                SpecificationAttributeOption = new SpecificationAttributeOption()
                {
                    Name = "SpecificationAttributeOption name 1",
                    DisplayOrder = 1,
                    SpecificationAttribute = new SpecificationAttribute()
                    {
                        Name = "SpecificationAttribute name 1",
                        DisplayOrder = 2,
                    }
                }
            };

            var fromDb = SaveAndLoadEntity(productSpecificationAttribute);
            fromDb.ShouldNotBeNull();
            fromDb.AllowFiltering.ShouldEqual(true);
            fromDb.ShowOnProductPage.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Name 1");

            fromDb.SpecificationAttributeOption.ShouldNotBeNull();
            fromDb.SpecificationAttributeOption.Name.ShouldEqual("SpecificationAttributeOption name 1");
        }
    }
}
