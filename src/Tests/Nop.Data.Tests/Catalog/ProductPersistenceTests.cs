using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Shipping;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_product()
        {
            var product = new Product
            {
                ProductType = ProductType.GroupedProduct,
                ParentGroupedProductId = 2,
                VisibleIndividually = true,
                Name = "Name 1",
                ShortDescription = "ShortDescription 1",
                FullDescription = "FullDescription 1",
                AdminComment = "AdminComment 1",
                VendorId = 1,
                ProductTemplateId = 2,
                ShowOnHomePage = false,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                AllowCustomerReviews = true,
                ApprovedRatingSum = 2,
                NotApprovedRatingSum = 3,
                ApprovedTotalReviews = 4,
                NotApprovedTotalReviews = 5,
                SubjectToAcl = true,
                LimitedToStores = true,
                Sku = "sku 1",
                ManufacturerPartNumber = "manufacturerPartNumber",
                Gtin = "gtin 1",
                IsGiftCard = true,
                GiftCardTypeId = 1,
                OverriddenGiftCardAmount = 1,
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
                IsRental = true,
                RentalPriceLength = 9,
                RentalPricePeriodId = 10,
                IsShipEnabled = true,
                IsFreeShipping = true,
                ShipSeparately = true,
                AdditionalShippingCharge = 10.1M,
                DeliveryDateId = 5,
                IsTaxExempt = true,
                TaxCategoryId = 11,
                IsTelecommunicationsOrBroadcastingOrElectronicServices = true,
                ManageInventoryMethodId = 12,
                UseMultipleWarehouses = true,
                WarehouseId = 6,
                StockQuantity = 13,
                DisplayStockAvailability = true,
                DisplayStockQuantity = true,
                MinStockQuantity = 14,
                LowStockActivityId = 15,
                NotifyAdminForQuantityBelow = 16,
                BackorderModeId = 17,
                AllowBackInStockSubscriptions = true,
                OrderMinimumQuantity = 18,
                OrderMaximumQuantity = 19,
                AllowedQuantities = "1, 5,6,10",
                AllowAddingOnlyExistingAttributeCombinations = true,
                NotReturnable = true,
                DisableBuyButton = true,
                DisableWishlistButton = true,
                AvailableForPreOrder = true,
                PreOrderAvailabilityStartDateTimeUtc = new DateTime(2010, 01, 01),
                CallForPrice = true,
                Price = 21.1M,
                OldPrice = 22.1M,
                ProductCost = 23.1M,
                SpecialPrice = 32.1M,
                SpecialPriceStartDateTimeUtc = new DateTime(2010, 01, 05),
                SpecialPriceEndDateTimeUtc = new DateTime(2010, 01, 06),
                CustomerEntersPrice = true,
                MinimumCustomerEnteredPrice = 24.1M,
                MaximumCustomerEnteredPrice = 25.1M,
                BasepriceEnabled = true,
                BasepriceAmount = 33.1M,
                BasepriceUnitId = 4,
                BasepriceBaseAmount = 34.1M,
                BasepriceBaseUnitId = 5,
                MarkAsNew = true,
                MarkAsNewStartDateTimeUtc = new DateTime(2010, 01, 07),
                MarkAsNewEndDateTimeUtc = new DateTime(2010, 01, 08),
                HasTierPrices = true,
                HasDiscountsApplied = true,
                Weight = 26.1M,
                Length = 27.1M,
                Width = 28.1M,
                Height = 29.1M,
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 01),
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 02),
                RequireOtherProducts = true,
                RequiredProductIds = "1,2,3",
                AutomaticallyAddRequiredProducts = true,
                DisplayOrder = 30,
                Published = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
            
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.ProductType.ShouldEqual(ProductType.GroupedProduct);
            fromDb.ParentGroupedProductId.ShouldEqual(2);
            fromDb.VisibleIndividually.ShouldEqual(true);
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.ShortDescription.ShouldEqual("ShortDescription 1");
            fromDb.FullDescription.ShouldEqual("FullDescription 1");
            fromDb.AdminComment.ShouldEqual("AdminComment 1");
            fromDb.VendorId.ShouldEqual(1);
            fromDb.ProductTemplateId.ShouldEqual(2);
            fromDb.ShowOnHomePage.ShouldEqual(false);
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.AllowCustomerReviews.ShouldEqual(true);
            fromDb.ApprovedRatingSum.ShouldEqual(2);
            fromDb.NotApprovedRatingSum.ShouldEqual(3);
            fromDb.ApprovedTotalReviews.ShouldEqual(4);
            fromDb.NotApprovedTotalReviews.ShouldEqual(5);
            fromDb.SubjectToAcl.ShouldEqual(true);
            fromDb.LimitedToStores.ShouldEqual(true);
            fromDb.ShouldNotBeNull();
            fromDb.Sku.ShouldEqual("sku 1");
            fromDb.ManufacturerPartNumber.ShouldEqual("manufacturerPartNumber");
            fromDb.Gtin.ShouldEqual("gtin 1");
            fromDb.IsGiftCard.ShouldEqual(true);
            fromDb.GiftCardTypeId.ShouldEqual(1);
            fromDb.OverriddenGiftCardAmount.ShouldEqual(1);
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
            fromDb.IsRental.ShouldEqual(true);
            fromDb.RentalPriceLength.ShouldEqual(9);
            fromDb.RentalPricePeriodId.ShouldEqual(10);
            fromDb.IsShipEnabled.ShouldEqual(true);
            fromDb.IsFreeShipping.ShouldEqual(true);
            fromDb.ShipSeparately.ShouldEqual(true);
            fromDb.AdditionalShippingCharge.ShouldEqual(10.1M);
            fromDb.DeliveryDateId.ShouldEqual(5);
            fromDb.IsTaxExempt.ShouldEqual(true);
            fromDb.TaxCategoryId.ShouldEqual(11);
            fromDb.IsTelecommunicationsOrBroadcastingOrElectronicServices.ShouldEqual(true);
            fromDb.ManageInventoryMethodId.ShouldEqual(12);
            fromDb.UseMultipleWarehouses.ShouldEqual(true);
            fromDb.WarehouseId.ShouldEqual(6);
            fromDb.StockQuantity.ShouldEqual(13);
            fromDb.DisplayStockAvailability.ShouldEqual(true);
            fromDb.DisplayStockQuantity.ShouldEqual(true);
            fromDb.MinStockQuantity.ShouldEqual(14);
            fromDb.LowStockActivityId.ShouldEqual(15);
            fromDb.NotifyAdminForQuantityBelow.ShouldEqual(16);
            fromDb.BackorderModeId.ShouldEqual(17);
            fromDb.AllowBackInStockSubscriptions.ShouldEqual(true);
            fromDb.OrderMinimumQuantity.ShouldEqual(18);
            fromDb.OrderMaximumQuantity.ShouldEqual(19);
            fromDb.AllowedQuantities.ShouldEqual("1, 5,6,10");
            fromDb.AllowAddingOnlyExistingAttributeCombinations.ShouldEqual(true);
            fromDb.NotReturnable.ShouldEqual(true);
            fromDb.DisableBuyButton.ShouldEqual(true);
            fromDb.DisableWishlistButton.ShouldEqual(true);
            fromDb.AvailableForPreOrder.ShouldEqual(true);
            fromDb.PreOrderAvailabilityStartDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.CallForPrice.ShouldEqual(true);
            fromDb.Price.ShouldEqual(21.1M);
            fromDb.OldPrice.ShouldEqual(22.1M);
            fromDb.ProductCost.ShouldEqual(23.1M);
            fromDb.SpecialPrice.ShouldEqual(32.1M);
            fromDb.SpecialPriceStartDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 05));
            fromDb.SpecialPriceEndDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 06));
            fromDb.CustomerEntersPrice.ShouldEqual(true);
            fromDb.MinimumCustomerEnteredPrice.ShouldEqual(24.1M);
            fromDb.MaximumCustomerEnteredPrice.ShouldEqual(25.1M);
            fromDb.BasepriceEnabled.ShouldEqual(true);
            fromDb.BasepriceAmount.ShouldEqual(33.1M);
            fromDb.BasepriceUnitId.ShouldEqual(4);
            fromDb.BasepriceBaseAmount.ShouldEqual(34.1M);
            fromDb.BasepriceBaseUnitId.ShouldEqual(5);
            fromDb.MarkAsNew.ShouldEqual(true);
            fromDb.MarkAsNewStartDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 07));
            fromDb.MarkAsNewEndDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 08));
            fromDb.HasTierPrices.ShouldEqual(true);
            fromDb.HasDiscountsApplied.ShouldEqual(true);
            fromDb.Weight.ShouldEqual(26.1M);
            fromDb.Length.ShouldEqual(27.1M);
            fromDb.Width.ShouldEqual(28.1M);
            fromDb.Height.ShouldEqual(29.1M);
            fromDb.AvailableStartDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.AvailableEndDateTimeUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.RequireOtherProducts.ShouldEqual(true);
            fromDb.RequiredProductIds.ShouldEqual("1,2,3");
            fromDb.AutomaticallyAddRequiredProducts.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(30);
            fromDb.Published.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }

        [Test]
        public void Can_save_and_load_product_with_productCategories()
        {
            var product = new Product
                          {
                              Name = "Name 1",
                              Published = true,
                              Deleted = false,
                              CreatedOnUtc = new DateTime(2010, 01, 01),
                              UpdatedOnUtc = new DateTime(2010, 01, 02)
                          };
            product.ProductCategories.Add
                (
                    new ProductCategory
                    {
                        IsFeaturedProduct = true,
                        DisplayOrder = 1,
                        Category = new Category
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
                        }
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.ProductCategories.ShouldNotBeNull();
            (fromDb.ProductCategories.Count == 1).ShouldBeTrue();
            fromDb.ProductCategories.First().IsFeaturedProduct.ShouldEqual(true);

            fromDb.ProductCategories.First().Category.ShouldNotBeNull();
            fromDb.ProductCategories.First().Category.Name.ShouldEqual("Books");
        }

        [Test]
        public void Can_save_and_load_product_with_productManufacturers()
        {
            var product = new Product
                          {
                              Name = "Name 1",
                              Published = true,
                              Deleted = false,
                              CreatedOnUtc = new DateTime(2010, 01, 01),
                              UpdatedOnUtc = new DateTime(2010, 01, 02)
                          };
            product.ProductManufacturers.Add
                (
                    new ProductManufacturer
                    {
                        IsFeaturedProduct = true,
                        DisplayOrder = 1,
                        Manufacturer = new Manufacturer
                        {
                            Name = "Name",
                            Description = "Description 1",
                            MetaKeywords = "Meta keywords",
                            MetaDescription = "Meta description",
                            MetaTitle = "Meta title",
                            PictureId = 3,
                            PageSize = 4,
                            PriceRanges = "1-3;",
                            Published = true,
                            Deleted = false,
                            DisplayOrder = 5,
                            CreatedOnUtc =
                                new DateTime(2010, 01, 01),
                            UpdatedOnUtc =
                                new DateTime(2010, 01, 02),
                        }
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.ProductManufacturers.ShouldNotBeNull();
            (fromDb.ProductManufacturers.Count == 1).ShouldBeTrue();
            fromDb.ProductManufacturers.First().IsFeaturedProduct.ShouldEqual(true);

            fromDb.ProductManufacturers.First().Manufacturer.ShouldNotBeNull();
            fromDb.ProductManufacturers.First().Manufacturer.Name.ShouldEqual("Name");
        }

        [Test]
        public void Can_save_and_load_product_with_productPictures()
        {
            var product = new Product
                          {
                              Name = "Name 1",
                              Published = true,
                              Deleted = false,
                              CreatedOnUtc = new DateTime(2010, 01, 01),
                              UpdatedOnUtc = new DateTime(2010, 01, 02)
                          };
            product.ProductPictures.Add
                (
                    new ProductPicture
                    {
                        DisplayOrder = 1,
                        Picture = new Picture
                        {
                            PictureBinary = new byte[] { 1, 2, 3 },
                            MimeType = MimeTypes.ImagePJpeg,
                            IsNew = true
                        }
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.ProductPictures.ShouldNotBeNull();
            (fromDb.ProductPictures.Count == 1).ShouldBeTrue();
            fromDb.ProductPictures.First().DisplayOrder.ShouldEqual(1);

            fromDb.ProductPictures.First().Picture.ShouldNotBeNull();
            fromDb.ProductPictures.First().Picture.MimeType.ShouldEqual(MimeTypes.ImagePJpeg);
        }

        [Test]
        public void Can_save_and_load_product_with_productTags()
        {
            var product = new Product
            {
                Name = "Name 1",
                Published = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02)
            };
            product.ProductTags.Add
                (
                    new ProductTag
                    {
                        Name = "Tag name 1",
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");


            fromDb.ProductTags.ShouldNotBeNull();
            (fromDb.ProductTags.Count == 1).ShouldBeTrue();
            fromDb.ProductTags.First().Name.ShouldEqual("Tag name 1");
        }

        [Test]
        public void Can_save_and_load_product_with_tierPrices()
        {
            var product = new Product
            {
                Name = "Product name 1",
                CreatedOnUtc = new DateTime(2010, 01, 03),
                UpdatedOnUtc = new DateTime(2010, 01, 04),
            };
            product.TierPrices.Add
                (
                    new TierPrice
                    {
                        Quantity = 1,
                        Price = 2,
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Product name 1");

            fromDb.TierPrices.ShouldNotBeNull();
            (fromDb.TierPrices.Count == 1).ShouldBeTrue();
            fromDb.TierPrices.First().Quantity.ShouldEqual(1);
        }

        [Test]
        public void Can_save_and_load_product_with_productWarehouseInventory()
        {
            var product = new Product
            {
                Name = "Name 1",
                Published = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02)
            };
            product.ProductWarehouseInventory.Add
                (
                    new ProductWarehouseInventory
                    {
                        Warehouse = new Warehouse
                        {
                            Name = "Name 2",
                            AddressId = 1,
                        },
                        StockQuantity = 2,
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.ProductWarehouseInventory.ShouldNotBeNull();
            (fromDb.ProductWarehouseInventory.Count == 1).ShouldBeTrue();
            fromDb.ProductWarehouseInventory.First().StockQuantity.ShouldEqual(2);
        }

    }
}
