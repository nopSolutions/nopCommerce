using System;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Discounts
{
    [TestFixture]
    public class DiscountPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_discount()
        {
            var discount = new Discount
                               {
                                   DiscountType = DiscountType.AssignedToCategories,
                                   Name = "Discount 1",
                                   UsePercentage = true,
                                   DiscountPercentage = 1.1M,
                                   DiscountAmount = 2.1M,
                                   StartDateUtc = new DateTime(2010, 01, 01),
                                   EndDateUtc = new DateTime(2010, 01, 02),
                                   RequiresCouponCode = true,
                                   CouponCode = "SecretCode",
                                   DiscountLimitation = DiscountLimitationType.Unlimited,
                                   LimitationTimes = 3,
                               };

            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();
            fromDb.DiscountType.ShouldEqual(DiscountType.AssignedToCategories);
            fromDb.Name.ShouldEqual("Discount 1");
            fromDb.UsePercentage.ShouldEqual(true);
            fromDb.DiscountPercentage.ShouldEqual(1.1M);
            fromDb.DiscountAmount.ShouldEqual(2.1M);
            fromDb.StartDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.EndDateUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.RequiresCouponCode.ShouldEqual(true);
            fromDb.CouponCode.ShouldEqual("SecretCode");
            fromDb.DiscountLimitation.ShouldEqual(DiscountLimitationType.Unlimited);
            fromDb.LimitationTimes.ShouldEqual(3);
        }

        [Test]
        public void Can_save_and_load_discount_with_discountRequirements()
        {
            var discount = new Discount
            {
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToCategories,
                UsePercentage = true,
                DiscountPercentage = 1,
                DiscountAmount = 2,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                RequiresCouponCode = true,
                CouponCode = "SecretCode",
                DiscountLimitation = DiscountLimitationType.Unlimited,
                LimitationTimes = 3
            };
            discount.DiscountRequirements.Add
                (
                     new DiscountRequirement()
                     {
                         DiscountRequirementRuleSystemName = "BillingCountryIs",
                         SpentAmount = 1,
                         BillingCountryId = 2,
                         ShippingCountryId = 3,
                     }
                );
            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Discount 1");
            
            fromDb.DiscountRequirements.ShouldNotBeNull();
            (fromDb.DiscountRequirements.Count == 1).ShouldBeTrue();
            fromDb.DiscountRequirements.First().DiscountRequirementRuleSystemName.ShouldEqual("BillingCountryIs");
        }

        [Test]
        public void Can_save_and_load_discount_with_appliedProductVariants()
        {
            var discount = new Discount
            {
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToCategories,
                UsePercentage = true,
                DiscountPercentage = 1,
                DiscountAmount = 2,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                RequiresCouponCode = true,
                CouponCode = "SecretCode",
                DiscountLimitation = DiscountLimitationType.Unlimited,
                LimitationTimes = 3
            };
            discount.AppliedToProductVariants.Add(GetTestProductVariant());
            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();

            fromDb.AppliedToProductVariants.ShouldNotBeNull();
            (fromDb.AppliedToProductVariants.Count == 1).ShouldBeTrue();
            fromDb.AppliedToProductVariants.First().Name.ShouldEqual("Product variant name 1");


        }

        [Test]
        public void Can_save_and_load_discount_with_appliedCategories()
        {
            var discount = new Discount
            {
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToCategories,
                UsePercentage = true,
                DiscountPercentage = 1,
                DiscountAmount = 2,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                RequiresCouponCode = true,
                CouponCode = "SecretCode",
                DiscountLimitation = DiscountLimitationType.Unlimited,
                LimitationTimes = 3
            };
            discount.AppliedToCategories.Add(GetTestCategory());
            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();

            fromDb.AppliedToCategories.ShouldNotBeNull();
            (fromDb.AppliedToCategories.Count == 1).ShouldBeTrue();
            fromDb.AppliedToCategories.First().Name.ShouldEqual("Books");


        }

        protected ProductVariant GetTestProductVariant()
        {
            return new ProductVariant
            {
                Name = "Product variant name 1",
                CreatedOnUtc = new DateTime(2010, 01, 03),
                UpdatedOnUtc = new DateTime(2010, 01, 04),
                Product = new Product()
                {
                    Name = "Name 1",
                    Published = true,
                    Deleted = false,
                    CreatedOnUtc = new DateTime(2010, 01, 01),
                    UpdatedOnUtc = new DateTime(2010, 01, 02)
                }
            };
        }

        protected Category GetTestCategory()
        {
            return new Category
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
        }
    }
}