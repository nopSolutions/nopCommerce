using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests
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
                                   DiscountPercentage = 1,
                                   DiscountAmount = 2,
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
            fromDb.DiscountPercentage.ShouldEqual(1);
            fromDb.DiscountAmount.ShouldEqual(2);
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
                LimitationTimes = 3,
                DiscountRequirements = new List<DiscountRequirement>()
                {
                     new DiscountRequirement()
                     {
                         DiscountRequirementRuleSystemName = "BillingCountryIs",
                         SpentAmount = 1,
                         BillingCountryId = 2,
                         ShippingCountryId =3,
                     }
                }
            };

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
                LimitationTimes = 3,
                AppliedToProductVariants = new List<ProductVariant>()
                {
                     GetTestProductVariant()
                }
            };

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
                LimitationTimes = 3,
                AppliedToCategories = new List<Category>()
                {
                     GetTestCategory()
                }
            };

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
                Sku = "sku 1",
                Description = "description",
                AdminComment = "adminComment",
                ManufacturerPartNumber = "manufacturerPartNumber",
                IsGiftCard = true,
                GiftCardTypeId = 1,
                IsDownload = true,
                DownloadId = 2,
                UnlimitedDownloads = true,
                MaxNumberOfDownloads = 3,
                DownloadExpirationDays = 4,
                DownloadActivationTypeId = 5,
                HasSampleDownload = true,
                SampleDownloadId = 6,
                HasUserAgreement = true,
                UserAgreementText = "userAgreementText",
                IsRecurring = true,
                RecurringCycleLength = 7,
                RecurringCyclePeriodId = 8,
                RecurringTotalCycles = 9,
                IsShipEnabled = true,
                IsFreeShipping = true,
                AdditionalShippingCharge = 10,
                IsTaxExempt = true,
                TaxCategoryId = 11,
                ManageInventoryMethodId = 12,
                StockQuantity = 13,
                DisplayStockAvailability = true,
                DisplayStockQuantity = true,
                MinStockQuantity = 14,
                LowStockActivityId = 15,
                NotifyAdminForQuantityBelow = 16,
                BackorderModeId = 17,
                OrderMinimumQuantity = 18,
                OrderMaximumQuantity = 19,
                WarehouseId = 20,
                DisableBuyButton = true,
                CallForPrice = true,
                Price = 21,
                OldPrice = 22,
                ProductCost = 23,
                CustomerEntersPrice = true,
                MinimumCustomerEnteredPrice = 24,
                MaximumCustomerEnteredPrice = 25,
                Weight = 26,
                Length = 27,
                Width = 28,
                Height = 29,
                PictureId = 0,
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 01),
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 02),
                Published = true,
                Deleted = false,
                DisplayOrder = 31,
                CreatedOnUtc = new DateTime(2010, 01, 03),
                UpdatedOnUtc = new DateTime(2010, 01, 04),
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
                }
            };
        }

        protected Category GetTestCategory()
        {
            return new Category
            {
                Name = "Books",
                Description = "Description 1",
                TemplateId = 1,
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