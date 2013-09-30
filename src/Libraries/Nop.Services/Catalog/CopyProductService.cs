using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Copy Product service
    /// </summary>
    public partial class CopyProductService : ICopyProductService
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductTagService _productTagService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        public CopyProductService(IProductService productService,
            IProductAttributeService productAttributeService, ILanguageService languageService,
            ILocalizedEntityService localizedEntityService, IPictureService pictureService,
            ICategoryService categoryService, IManufacturerService manufacturerService,
            ISpecificationAttributeService specificationAttributeService, IDownloadService downloadService,
            IProductAttributeParser productAttributeParser, IProductTagService productTagService,
            IUrlRecordService urlRecordService, IStoreMappingService storeMappingService)
        {
            this._productService = productService;
            this._productAttributeService = productAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._pictureService = pictureService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._specificationAttributeService = specificationAttributeService;
            this._downloadService = downloadService;
            this._productAttributeParser = productAttributeParser;
            this._productTagService = productTagService;
            this._urlRecordService = urlRecordService;
            this._storeMappingService = storeMappingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a copy of product with all depended data
        /// </summary>
        /// <param name="product">The product to copy</param>
        /// <param name="newName">The name of product duplicate</param>
        /// <param name="isPublished">A value indicating whether the product duplicate should be published</param>
        /// <param name="copyImages">A value indicating whether the product images should be copied</param>
        /// <param name="copyAssociatedProducts">A value indicating whether the copy associated products</param>
        /// <returns>Product copy</returns>
        public virtual Product CopyProduct(Product product, string newName,
            bool isPublished = true, bool copyImages = true, bool copyAssociatedProducts = true)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (String.IsNullOrEmpty(newName))
                throw new ArgumentException("Product name is required");


            Product productCopy = null;

            //product download & sample download
            int downloadId = product.DownloadId;
            int sampleDownloadId = product.SampleDownloadId;
            if (product.IsDownload)
            {
                var download = _downloadService.GetDownloadById(product.DownloadId);
                if (download != null)
                {
                    var downloadCopy = new Download()
                    {
                        DownloadGuid = Guid.NewGuid(),
                        UseDownloadUrl = download.UseDownloadUrl,
                        DownloadUrl = download.DownloadUrl,
                        DownloadBinary = download.DownloadBinary,
                        ContentType = download.ContentType,
                        Filename = download.Filename,
                        Extension = download.Extension,
                        IsNew = download.IsNew,
                    };
                    _downloadService.InsertDownload(downloadCopy);
                    downloadId = downloadCopy.Id;
                }

                if (product.HasSampleDownload)
                {
                    var sampleDownload = _downloadService.GetDownloadById(product.SampleDownloadId);
                    if (sampleDownload != null)
                    {
                        var sampleDownloadCopy = new Download()
                        {
                            DownloadGuid = Guid.NewGuid(),
                            UseDownloadUrl = sampleDownload.UseDownloadUrl,
                            DownloadUrl = sampleDownload.DownloadUrl,
                            DownloadBinary = sampleDownload.DownloadBinary,
                            ContentType = sampleDownload.ContentType,
                            Filename = sampleDownload.Filename,
                            Extension = sampleDownload.Extension,
                            IsNew = sampleDownload.IsNew
                        };
                        _downloadService.InsertDownload(sampleDownloadCopy);
                        sampleDownloadId = sampleDownloadCopy.Id;
                    }
                }
            }

            // product
            productCopy = new Product()
            {
                ProductTypeId = product.ProductTypeId,
                ParentGroupedProductId = product.ParentGroupedProductId,
                VisibleIndividually = product.VisibleIndividually,
                Name = newName,
                ShortDescription = product.ShortDescription,
                FullDescription = product.FullDescription,
                VendorId = product.VendorId,
                ProductTemplateId = product.ProductTemplateId,
                AdminComment = product.AdminComment,
                ShowOnHomePage = product.ShowOnHomePage,
                MetaKeywords = product.MetaKeywords,
                MetaDescription = product.MetaDescription,
                MetaTitle = product.MetaTitle,
                AllowCustomerReviews = product.AllowCustomerReviews,
                LimitedToStores = product.LimitedToStores,
                Sku = product.Sku,
                ManufacturerPartNumber = product.ManufacturerPartNumber,
                Gtin = product.Gtin,
                IsGiftCard = product.IsGiftCard,
                GiftCardType = product.GiftCardType,
                RequireOtherProducts = product.RequireOtherProducts,
                RequiredProductIds = product.RequiredProductIds,
                AutomaticallyAddRequiredProducts = product.AutomaticallyAddRequiredProducts,
                IsDownload = product.IsDownload,
                DownloadId = downloadId,
                UnlimitedDownloads = product.UnlimitedDownloads,
                MaxNumberOfDownloads = product.MaxNumberOfDownloads,
                DownloadExpirationDays = product.DownloadExpirationDays,
                DownloadActivationType = product.DownloadActivationType,
                HasSampleDownload = product.HasSampleDownload,
                SampleDownloadId = sampleDownloadId,
                HasUserAgreement = product.HasUserAgreement,
                UserAgreementText = product.UserAgreementText,
                IsRecurring = product.IsRecurring,
                RecurringCycleLength = product.RecurringCycleLength,
                RecurringCyclePeriod = product.RecurringCyclePeriod,
                RecurringTotalCycles = product.RecurringTotalCycles,
                IsShipEnabled = product.IsShipEnabled,
                IsFreeShipping = product.IsFreeShipping,
                AdditionalShippingCharge = product.AdditionalShippingCharge,
                DeliveryDateId = product.DeliveryDateId,
                WarehouseId = product.WarehouseId,
                IsTaxExempt = product.IsTaxExempt,
                TaxCategoryId = product.TaxCategoryId,
                ManageInventoryMethod = product.ManageInventoryMethod,
                StockQuantity = product.StockQuantity,
                DisplayStockAvailability = product.DisplayStockAvailability,
                DisplayStockQuantity = product.DisplayStockQuantity,
                MinStockQuantity = product.MinStockQuantity,
                LowStockActivityId = product.LowStockActivityId,
                NotifyAdminForQuantityBelow = product.NotifyAdminForQuantityBelow,
                BackorderMode = product.BackorderMode,
                AllowBackInStockSubscriptions = product.AllowBackInStockSubscriptions,
                OrderMinimumQuantity = product.OrderMinimumQuantity,
                OrderMaximumQuantity = product.OrderMaximumQuantity,
                AllowedQuantities = product.AllowedQuantities,
                DisableBuyButton = product.DisableBuyButton,
                DisableWishlistButton = product.DisableWishlistButton,
                AvailableForPreOrder = product.AvailableForPreOrder,
                PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc,
                CallForPrice = product.CallForPrice,
                Price = product.Price,
                OldPrice = product.OldPrice,
                ProductCost = product.ProductCost,
                SpecialPrice = product.SpecialPrice,
                SpecialPriceStartDateTimeUtc = product.SpecialPriceStartDateTimeUtc,
                SpecialPriceEndDateTimeUtc = product.SpecialPriceEndDateTimeUtc,
                CustomerEntersPrice = product.CustomerEntersPrice,
                MinimumCustomerEnteredPrice = product.MinimumCustomerEnteredPrice,
                MaximumCustomerEnteredPrice = product.MaximumCustomerEnteredPrice,
                Weight = product.Weight,
                Length = product.Length,
                Width = product.Width,
                Height = product.Height,
                AvailableStartDateTimeUtc = product.AvailableStartDateTimeUtc,
                AvailableEndDateTimeUtc = product.AvailableEndDateTimeUtc,
                DisplayOrder = product.DisplayOrder,
                Published = isPublished,
                Deleted = product.Deleted,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            //validate search engine name
            _productService.InsertProduct(productCopy);

            //search engine name
            _urlRecordService.SaveSlug(productCopy, productCopy.ValidateSeName("", productCopy.Name, true), 0);

            var languages = _languageService.GetAllLanguages(true);

            //localization
            foreach (var lang in languages)
            {
                var name = product.GetLocalized(x => x.Name, lang.Id, false, false);
                if (!String.IsNullOrEmpty(name))
                    _localizedEntityService.SaveLocalizedValue(productCopy, x => x.Name, name, lang.Id);

                var shortDescription = product.GetLocalized(x => x.ShortDescription, lang.Id, false, false);
                if (!String.IsNullOrEmpty(shortDescription))
                    _localizedEntityService.SaveLocalizedValue(productCopy, x => x.ShortDescription, shortDescription, lang.Id);

                var fullDescription = product.GetLocalized(x => x.FullDescription, lang.Id, false, false);
                if (!String.IsNullOrEmpty(fullDescription))
                    _localizedEntityService.SaveLocalizedValue(productCopy, x => x.FullDescription, fullDescription, lang.Id);

                var metaKeywords = product.GetLocalized(x => x.MetaKeywords, lang.Id, false, false);
                if (!String.IsNullOrEmpty(metaKeywords))
                    _localizedEntityService.SaveLocalizedValue(productCopy, x => x.MetaKeywords, metaKeywords, lang.Id);

                var metaDescription = product.GetLocalized(x => x.MetaDescription, lang.Id, false, false);
                if (!String.IsNullOrEmpty(metaDescription))
                    _localizedEntityService.SaveLocalizedValue(productCopy, x => x.MetaDescription, metaDescription, lang.Id);

                var metaTitle = product.GetLocalized(x => x.MetaTitle, lang.Id, false, false);
                if (!String.IsNullOrEmpty(metaTitle))
                    _localizedEntityService.SaveLocalizedValue(productCopy, x => x.MetaTitle, metaTitle, lang.Id);

                //search engine name
                _urlRecordService.SaveSlug(productCopy, productCopy.ValidateSeName("", name, false), lang.Id);
            }

            //product tags
            foreach (var productTag in product.ProductTags)
            {
                productCopy.ProductTags.Add(productTag);
            }
            _productService.UpdateProduct(product);

            // product pictures

            //variant to store original and new picture identifiers
            var originalNewPictureIdentifiers = new Dictionary<int, int>();
            if (copyImages)
            {
                foreach (var productPicture in product.ProductPictures)
                {
                    var picture = productPicture.Picture;
                    var pictureCopy = _pictureService.InsertPicture(
                        _pictureService.LoadPictureBinary(picture),
                        picture.MimeType,
                        _pictureService.GetPictureSeName(newName),
                        true);
                    _productService.InsertProductPicture(new ProductPicture()
                    {
                        ProductId = productCopy.Id,
                        PictureId = pictureCopy.Id,
                        DisplayOrder = productPicture.DisplayOrder
                    });
                    originalNewPictureIdentifiers.Add(picture.Id, pictureCopy.Id);
                }
            }

            // product <-> categories mappings
            foreach (var productCategory in product.ProductCategories)
            {
                var productCategoryCopy = new ProductCategory()
                {
                    ProductId = productCopy.Id,
                    CategoryId = productCategory.CategoryId,
                    IsFeaturedProduct = productCategory.IsFeaturedProduct,
                    DisplayOrder = productCategory.DisplayOrder
                };

                _categoryService.InsertProductCategory(productCategoryCopy);
            }

            // product <-> manufacturers mappings
            foreach (var productManufacturers in product.ProductManufacturers)
            {
                var productManufacturerCopy = new ProductManufacturer()
                {
                    ProductId = productCopy.Id,
                    ManufacturerId = productManufacturers.ManufacturerId,
                    IsFeaturedProduct = productManufacturers.IsFeaturedProduct,
                    DisplayOrder = productManufacturers.DisplayOrder
                };

                _manufacturerService.InsertProductManufacturer(productManufacturerCopy);
            }

            // product <-> releated products mappings
            foreach (var relatedProduct in _productService.GetRelatedProductsByProductId1(product.Id, true))
            {
                _productService.InsertRelatedProduct(
                    new RelatedProduct()
                    {
                        ProductId1 = productCopy.Id,
                        ProductId2 = relatedProduct.ProductId2,
                        DisplayOrder = relatedProduct.DisplayOrder
                    });
            }

            // product <-> cross sells mappings
            foreach (var csProduct in _productService.GetCrossSellProductsByProductId1(product.Id, true))
            {
                _productService.InsertCrossSellProduct(
                    new CrossSellProduct()
                    {
                        ProductId1 = productCopy.Id,
                        ProductId2 = csProduct.ProductId2,
                    });
            }

            // product specifications
            foreach (var productSpecificationAttribute in product.ProductSpecificationAttributes)
            {
                var psaCopy = new ProductSpecificationAttribute()
                {
                    ProductId = productCopy.Id,
                    SpecificationAttributeOptionId = productSpecificationAttribute.SpecificationAttributeOptionId,
                    CustomValue = productSpecificationAttribute.CustomValue,
                    AllowFiltering = productSpecificationAttribute.AllowFiltering,
                    ShowOnProductPage = productSpecificationAttribute.ShowOnProductPage,
                    DisplayOrder = productSpecificationAttribute.DisplayOrder
                };
                _specificationAttributeService.InsertProductSpecificationAttribute(psaCopy);
            }

            //store mapping
            var selectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(product);
            foreach (var id in selectedStoreIds)
            {
                _storeMappingService.InsertStoreMapping(productCopy, id);
            }


            // product <-> attributes mappings
            var associatedAttributes = new Dictionary<int, int>();
            var associatedAttributeValues = new Dictionary<int, int>();
            foreach (var productVariantAttribute in _productAttributeService.GetProductVariantAttributesByProductId(product.Id))
            {
                var productVariantAttributeCopy = new ProductVariantAttribute()
                {
                    ProductId = productCopy.Id,
                    ProductAttributeId = productVariantAttribute.ProductAttributeId,
                    TextPrompt = productVariantAttribute.TextPrompt,
                    IsRequired = productVariantAttribute.IsRequired,
                    AttributeControlTypeId = productVariantAttribute.AttributeControlTypeId,
                    DisplayOrder = productVariantAttribute.DisplayOrder
                };
                _productAttributeService.InsertProductVariantAttribute(productVariantAttributeCopy);
                //save associated value (used for combinations copying)
                associatedAttributes.Add(productVariantAttribute.Id, productVariantAttributeCopy.Id);

                // product variant attribute values
                var productVariantAttributeValues = _productAttributeService.GetProductVariantAttributeValues(productVariantAttribute.Id);
                foreach (var productVariantAttributeValue in productVariantAttributeValues)
                {
                    int pvavPictureId = 0;
                    if (originalNewPictureIdentifiers.ContainsKey(productVariantAttributeValue.PictureId))
                    {
                        pvavPictureId = originalNewPictureIdentifiers[productVariantAttributeValue.PictureId];
                    }
                    var pvavCopy = new ProductVariantAttributeValue()
                    {
                        ProductVariantAttributeId = productVariantAttributeCopy.Id,
                        AttributeValueTypeId = productVariantAttributeValue.AttributeValueTypeId,
                        AssociatedProductId = productVariantAttributeValue.AssociatedProductId,
                        Name = productVariantAttributeValue.Name,
                        ColorSquaresRgb = productVariantAttributeValue.ColorSquaresRgb,
                        PriceAdjustment = productVariantAttributeValue.PriceAdjustment,
                        WeightAdjustment = productVariantAttributeValue.WeightAdjustment,
                        Cost = productVariantAttributeValue.Cost,
                        Quantity = productVariantAttributeValue.Quantity,
                        IsPreSelected = productVariantAttributeValue.IsPreSelected,
                        DisplayOrder = productVariantAttributeValue.DisplayOrder,
                        PictureId = pvavPictureId,
                    };
                    _productAttributeService.InsertProductVariantAttributeValue(pvavCopy);

                    //save associated value (used for combinations copying)
                    associatedAttributeValues.Add(productVariantAttributeValue.Id, pvavCopy.Id);

                    //localization
                    foreach (var lang in languages)
                    {
                        var name = productVariantAttributeValue.GetLocalized(x => x.Name, lang.Id, false, false);
                        if (!String.IsNullOrEmpty(name))
                            _localizedEntityService.SaveLocalizedValue(pvavCopy, x => x.Name, name, lang.Id);
                    }
                }
            }
            //attribute combinations
            foreach (var combination in _productAttributeService.GetAllProductVariantAttributeCombinations(product.Id))
            {
                //generate new AttributesXml according to new value IDs
                string newAttributesXml = "";
                var parsedProductVariantAttributes = _productAttributeParser.ParseProductVariantAttributes(combination.AttributesXml);
                foreach (var oldPva in parsedProductVariantAttributes)
                {
                    if (associatedAttributes.ContainsKey(oldPva.Id))
                    {
                        int newPvaId = associatedAttributes[oldPva.Id];
                        var newPva = _productAttributeService.GetProductVariantAttributeById(newPvaId);
                        if (newPva != null)
                        {
                            var oldPvaValuesStr = _productAttributeParser.ParseValues(combination.AttributesXml, oldPva.Id);
                            foreach (var oldPvaValueStr in oldPvaValuesStr)
                            {
                                if (newPva.ShouldHaveValues())
                                {
                                    //attribute values
                                    int oldPvaValue = int.Parse(oldPvaValueStr);
                                    if (associatedAttributeValues.ContainsKey(oldPvaValue))
                                    {
                                        int newPvavId = associatedAttributeValues[oldPvaValue];
                                        var newPvav = _productAttributeService.GetProductVariantAttributeValueById(newPvavId);
                                        if (newPvav != null)
                                        {
                                            newAttributesXml = _productAttributeParser.AddProductAttribute(newAttributesXml,
                                                newPva, newPvav.Id.ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    //just a text
                                    newAttributesXml = _productAttributeParser.AddProductAttribute(newAttributesXml,
                                        newPva, oldPvaValueStr);
                                }
                            }
                        }
                    }
                }
                var combinationCopy = new ProductVariantAttributeCombination()
                {
                    ProductId = productCopy.Id,
                    AttributesXml = newAttributesXml,
                    StockQuantity = combination.StockQuantity,
                    AllowOutOfStockOrders = combination.AllowOutOfStockOrders,
                    Sku = combination.Sku,
                    ManufacturerPartNumber = combination.ManufacturerPartNumber,
                    Gtin = combination.Gtin,
                    OverriddenPrice = combination.OverriddenPrice
                };
                _productAttributeService.InsertProductVariantAttributeCombination(combinationCopy);
            }

            //tier prices
            foreach (var tierPrice in product.TierPrices)
            {
                _productService.InsertTierPrice(
                    new TierPrice()
                    {
                        ProductId = productCopy.Id,
                        StoreId = tierPrice.StoreId,
                        CustomerRoleId = tierPrice.CustomerRoleId,
                        Quantity = tierPrice.Quantity,
                        Price = tierPrice.Price
                    });
            }

            // product <-> discounts mapping
            foreach (var discount in product.AppliedDiscounts)
            {
                productCopy.AppliedDiscounts.Add(discount);
                _productService.UpdateProduct(productCopy);
            }


            //update "HasTierPrices" and "HasDiscountsApplied" properties
            _productService.UpdateHasTierPricesProperty(productCopy);
            _productService.UpdateHasDiscountsApplied(productCopy);


            //associated products
            if (copyAssociatedProducts)
            {
                var associatedProducts = _productService.SearchProducts(parentGroupedProductId: product.Id, showHidden: true);
                foreach (var associatedProduct in associatedProducts)
                {
                    var associatedProductCopy = CopyProduct(associatedProduct, string.Format("Copy of {0}", associatedProduct.Name),
                        isPublished, copyImages, false);
                    associatedProductCopy.ParentGroupedProductId = productCopy.Id;
                    _productService.UpdateProduct(productCopy);
                }
            }

            return productCopy;
        }

        #endregion
    }
}
