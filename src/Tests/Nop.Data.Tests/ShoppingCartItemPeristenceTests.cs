using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Customers;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Tax;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class ShoppingCartItemPeristenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_shoppingCartItem()
        {
            var sci = new ShoppingCartItem
            {
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                AttributesXml = "AttributesXml 1",
                CustomerEnteredPrice = 1.1M,
                Quantity= 2,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
                Customer = GetTestCustomer(),
                ProductVariant = GetTestProductVariant()
            };

            var fromDb = SaveAndLoadEntity(sci);
            fromDb.ShouldNotBeNull();

            fromDb.ShoppingCartType.ShouldEqual(ShoppingCartType.ShoppingCart);
            fromDb.AttributesXml.ShouldEqual("AttributesXml 1");
            fromDb.CustomerEnteredPrice.ShouldEqual(1.1M);
            fromDb.Quantity.ShouldEqual(2);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));

            fromDb.Customer.ShouldNotBeNull();

            fromDb.ProductVariant.ShouldNotBeNull();

        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = "admin@yourStore.com",
                Username = "admin@yourStore.com",
                AdminComment = "some comment here",
                TaxDisplayType = TaxDisplayType.IncludingTax,
                IsTaxExempt = true,
                VatNumber = "123456",
                VatNumberStatus = VatNumberStatus.Valid,
                DiscountCouponCode = "coupon1",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
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
    }
}
