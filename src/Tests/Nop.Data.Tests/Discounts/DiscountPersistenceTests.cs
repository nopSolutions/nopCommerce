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
                                   MaximumDiscountAmount = 3.1M,
                                   StartDateUtc = new DateTime(2010, 01, 01),
                                   EndDateUtc = new DateTime(2010, 01, 02),
                                   RequiresCouponCode = true,
                                   CouponCode = "SecretCode",
                                   IsCumulative = true,
                                   DiscountLimitation = DiscountLimitationType.Unlimited,
                                   LimitationTimes = 3,
                                   MaximumDiscountedQuantity = 4,
                                   AppliedToSubCategories = true
            };

            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();
            fromDb.DiscountType.ShouldEqual(DiscountType.AssignedToCategories);
            fromDb.Name.ShouldEqual("Discount 1");
            fromDb.UsePercentage.ShouldEqual(true);
            fromDb.DiscountPercentage.ShouldEqual(1.1M);
            fromDb.DiscountAmount.ShouldEqual(2.1M);
            fromDb.MaximumDiscountAmount.ShouldEqual(3.1M);
            fromDb.StartDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.EndDateUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.RequiresCouponCode.ShouldEqual(true);
            fromDb.CouponCode.ShouldEqual("SecretCode");
            fromDb.IsCumulative.ShouldEqual(true);
            fromDb.DiscountLimitation.ShouldEqual(DiscountLimitationType.Unlimited);
            fromDb.LimitationTimes.ShouldEqual(3);
            fromDb.MaximumDiscountedQuantity.ShouldEqual(4);
            fromDb.AppliedToSubCategories.ShouldEqual(true);
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
                     new DiscountRequirement
                     {
                         DiscountRequirementRuleSystemName = "BillingCountryIs"
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
        public void Can_save_and_load_discount_with_appliedProducts()
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
            discount.AppliedToProducts.Add(GetTestProduct());
            var fromDb = SaveAndLoadEntity(discount);
            fromDb.ShouldNotBeNull();

            fromDb.AppliedToProducts.ShouldNotBeNull();
            (fromDb.AppliedToProducts.Count == 1).ShouldBeTrue();
            fromDb.AppliedToProducts.First().Name.ShouldEqual("Product name 1");


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

        protected Product GetTestProduct()
        {
            return new Product
            {
                Name = "Product name 1",
                CreatedOnUtc = new DateTime(2010, 01, 03),
                UpdatedOnUtc = new DateTime(2010, 01, 04),
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