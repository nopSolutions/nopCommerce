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
    public class ProductVariantPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productVariant()
        {
            var productVariant = new ProductVariant
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

            var fromDb = SaveAndLoadEntity(productVariant);
            fromDb.Name.ShouldEqual("Product variant name 1");
            fromDb.Sku.ShouldEqual("sku 1");
            fromDb.Description.ShouldEqual("description");
            fromDb.AdminComment.ShouldEqual("adminComment");
            fromDb.ManufacturerPartNumber.ShouldEqual("manufacturerPartNumber");
            fromDb.IsGiftCard.ShouldEqual(true);
            fromDb.GiftCardTypeId.ShouldEqual(1);
            fromDb.IsDownload.ShouldEqual(true);
            fromDb.DownloadId.ShouldEqual(2);
            fromDb.UnlimitedDownloads.ShouldEqual(true);
            fromDb.MaxNumberOfDownloads.ShouldEqual(3);
            fromDb.DownloadExpirationDays.ShouldEqual(4);
            fromDb.DownloadActivationTypeId.ShouldEqual(5);
            fromDb.HasSampleDownload.ShouldEqual(true);
            fromDb.SampleDownloadId.ShouldEqual(6);
            fromDb.HasUserAgreement.ShouldEqual(true);
            fromDb.UserAgreementText.ShouldEqual("userAgreementText");
            fromDb.IsRecurring.ShouldEqual(true);
            fromDb.RecurringCycleLength.ShouldEqual(7);
            fromDb.RecurringCyclePeriodId.ShouldEqual(8);
            fromDb.RecurringTotalCycles.ShouldEqual(9);
            fromDb.IsShipEnabled.ShouldEqual(true);
            fromDb.IsFreeShipping.ShouldEqual(true);
            fromDb.AdditionalShippingCharge.ShouldEqual(10);
            fromDb.IsTaxExempt.ShouldEqual(true);
            fromDb.TaxCategoryId.ShouldEqual(11);
            fromDb.ManageInventoryMethodId.ShouldEqual(12);
            fromDb.StockQuantity.ShouldEqual(13);
            fromDb.DisplayStockAvailability.ShouldEqual(true);
            fromDb.DisplayStockQuantity.ShouldEqual(true);
            fromDb.MinStockQuantity.ShouldEqual(14);
            fromDb.LowStockActivityId.ShouldEqual(15);
            fromDb.NotifyAdminForQuantityBelow.ShouldEqual(16);
            fromDb.BackorderModeId.ShouldEqual(17);
            fromDb.OrderMinimumQuantity.ShouldEqual(18);
            fromDb.OrderMaximumQuantity.ShouldEqual(19);
            fromDb.WarehouseId.ShouldEqual(20);
            fromDb.DisableBuyButton.ShouldEqual(true);
            fromDb.CallForPrice.ShouldEqual(true);
            fromDb.Price.ShouldEqual(21);
            fromDb.OldPrice.ShouldEqual(22);
            fromDb.ProductCost.ShouldEqual(23);
            fromDb.CustomerEntersPrice.ShouldEqual(true);
            fromDb.MinimumCustomerEnteredPrice.ShouldEqual(24);
            fromDb.MaximumCustomerEnteredPrice.ShouldEqual(25);
            fromDb.Weight.ShouldEqual(26);
            fromDb.Length.ShouldEqual(27);
            fromDb.Width.ShouldEqual(28);
            fromDb.Height.ShouldEqual(29);
            fromDb.PictureId.ShouldEqual(0);
            fromDb.AvailableStartDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.AvailableEndDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.Published.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.DisplayOrder.ShouldEqual(31);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 03));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 04));

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Name 1");
        }

        [Test]
        public void Can_save_and_load_productVariant_with_localizedProductVariants()
        {
            var lang = new Language()
            {
                Name = "English",
                LanguageCulture = "en-Us",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };

            var productVariant = new ProductVariant
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
                                                   UpdatedOnUtc = new DateTime(2010, 01, 02),
                                               },
                                     LocalizedProductVariants = new List<LocalizedProductVariant>()
                                                                {
                                                                    new LocalizedProductVariant
                                                                    {
                                                                        Name = "Name localized",
                                                                        Description = "Description 1 localized",
                                                                        Language = lang
                                                                    },
                                                                }
                                 };

            var fromDb = SaveAndLoadEntity(productVariant);
            fromDb.Name.ShouldEqual("Product variant name 1");

            fromDb.LocalizedProductVariants.ShouldNotBeNull();
            (fromDb.LocalizedProductVariants.Count == 1).ShouldBeTrue();
            fromDb.LocalizedProductVariants.First().Name.ShouldEqual("Name localized");
        }

        [Test]
        public void Can_save_and_load_productVariant_with_tierPrices()
        {
            var productVariant = new ProductVariant
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
                    UpdatedOnUtc = new DateTime(2010, 01, 02),
                },
                TierPrices = new List<TierPrice>()
                                                                {
                                                                    new TierPrice
                                                                    {
                                                                        Quantity= 1,
                                                                        Price= 2,
                                                                    },
                                                                }
            };

            var fromDb = SaveAndLoadEntity(productVariant);
            fromDb.Name.ShouldEqual("Product variant name 1");

            fromDb.TierPrices.ShouldNotBeNull();
            (fromDb.TierPrices.Count == 1).ShouldBeTrue();
            fromDb.TierPrices.First().Quantity.ShouldEqual(1);
        }
    }
}
