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
    public class ProductVariantAttributePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productVariantAttribute()
        {
            var pva = new ProductVariantAttribute
                      {
                          TextPrompt = "TextPrompt 1",
                          IsRequired = true,
                          AttributeControlType = AttributeControlTypeEnum.DropdownList,
                          DisplayOrder = 1,
                          ProductVariant = new ProductVariant()
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
                                                         }
                                           },
                          ProductAttribute = new ProductAttribute()
                          {
                              Name = "Name 1",
                              Description = "Description 1",
                          }
                      };

            var fromDb = SaveAndLoadEntity(pva);
            fromDb.ShouldNotBeNull();
            fromDb.TextPrompt.ShouldEqual("TextPrompt 1");
            fromDb.IsRequired.ShouldEqual(true);
            fromDb.AttributeControlType.ShouldEqual(AttributeControlTypeEnum.DropdownList);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.ProductVariant.ShouldNotBeNull();
            fromDb.ProductVariant.Name.ShouldEqual("Product variant name 1");

            fromDb.ProductAttribute.ShouldNotBeNull();
            fromDb.ProductAttribute.Name.ShouldEqual("Name 1");
        }
    }
}
