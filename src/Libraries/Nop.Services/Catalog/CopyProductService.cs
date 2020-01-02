using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
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

        private readonly ICategoryService _categoryService;
        private readonly IDownloadService _downloadService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public CopyProductService(ICategoryService categoryService,
            IDownloadService downloadService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IManufacturerService manufacturerService,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            ISpecificationAttributeService specificationAttributeService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService)
        {
            _categoryService = categoryService;
            _downloadService = downloadService;
            _languageService = languageService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _manufacturerService = manufacturerService;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _specificationAttributeService = specificationAttributeService;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Copy discount mappings
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productCopy">New product</param>
        protected virtual void CopyDiscountsMapping(Product product, Product productCopy)
        {
            foreach (var discountMapping in product.DiscountProductMappings)
            {
                productCopy.DiscountProductMappings.Add(new DiscountProductMapping { Discount = discountMapping.Discount });
                _productService.UpdateProduct(productCopy);
            }
        }

        /// <summary>
        /// Copy associated products
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="isPublished">A value indicating whether they should be published</param>
        /// <param name="copyImages">A value indicating whether to copy images</param>
        /// <param name="copyAssociatedProducts">A value indicating whether to copy associated products</param>
        /// <param name="productCopy">New product</param>
        protected virtual void CopyAssociatedProducts(Product product, bool isPublished, bool copyImages, bool copyAssociatedProducts, Product productCopy)
        {
            if (!copyAssociatedProducts)
                return;

            var associatedProducts = _productService.GetAssociatedProducts(product.Id, showHidden: true);
            foreach (var associatedProduct in associatedProducts)
            {
                var associatedProductCopy = CopyProduct(associatedProduct,
                    string.Format(NopCatalogDefaults.ProductCopyNameTemplate, associatedProduct.Name),
                    isPublished, copyImages, false);
                associatedProductCopy.ParentGroupedProductId = productCopy.Id;
                _productService.UpdateProduct(productCopy);
            }
        }

        /// <summary>
        /// Copy tier prices
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productCopy">New product</param>
        protected virtual void CopyTierPrices(Product product, Product productCopy)
        {
            foreach (var tierPrice in product.TierPrices)
            {
                _productService.InsertTierPrice(new TierPrice
                {
                    ProductId = productCopy.Id,
                    StoreId = tierPrice.StoreId,
                    CustomerRoleId = tierPrice.CustomerRoleId,
                    Quantity = tierPrice.Quantity,
                    Price = tierPrice.Price,
                    StartDateTimeUtc = tierPrice.StartDateTimeUtc,
                    EndDateTimeUtc = tierPrice.EndDateTimeUtc
                });
            }
        }

        /// <summary>
        /// Copy attributes mapping
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productCopy">New product</param>
        /// <param name="originalNewPictureIdentifiers">Identifiers of pictures</param>
        protected virtual void CopyAttributesMapping(Product product, Product productCopy, Dictionary<int, int> originalNewPictureIdentifiers)
        {
            var associatedAttributes = new Dictionary<int, int>();
            var associatedAttributeValues = new Dictionary<int, int>();

            //attribute mapping with condition attributes
            var oldCopyWithConditionAttributes = new List<ProductAttributeMapping>();

            //all product attribute mapping copies
            var productAttributeMappingCopies = new Dictionary<int, ProductAttributeMapping>();

            var languages = _languageService.GetAllLanguages(true);

            foreach (var productAttributeMapping in _productAttributeService.GetProductAttributeMappingsByProductId(product.Id))
            {
                var productAttributeMappingCopy = new ProductAttributeMapping
                {
                    ProductId = productCopy.Id,
                    ProductAttributeId = productAttributeMapping.ProductAttributeId,
                    TextPrompt = productAttributeMapping.TextPrompt,
                    IsRequired = productAttributeMapping.IsRequired,
                    AttributeControlTypeId = productAttributeMapping.AttributeControlTypeId,
                    DisplayOrder = productAttributeMapping.DisplayOrder,
                    ValidationMinLength = productAttributeMapping.ValidationMinLength,
                    ValidationMaxLength = productAttributeMapping.ValidationMaxLength,
                    ValidationFileAllowedExtensions = productAttributeMapping.ValidationFileAllowedExtensions,
                    ValidationFileMaximumSize = productAttributeMapping.ValidationFileMaximumSize,
                    DefaultValue = productAttributeMapping.DefaultValue
                };
                _productAttributeService.InsertProductAttributeMapping(productAttributeMappingCopy);
                //localization
                foreach (var lang in languages)
                {
                    var textPrompt = _localizationService.GetLocalized(productAttributeMapping, x => x.TextPrompt, lang.Id, false, false);
                    if (!string.IsNullOrEmpty(textPrompt))
                        _localizedEntityService.SaveLocalizedValue(productAttributeMappingCopy, x => x.TextPrompt, textPrompt,
                            lang.Id);
                }

                productAttributeMappingCopies.Add(productAttributeMappingCopy.Id, productAttributeMappingCopy);

                if (!string.IsNullOrEmpty(productAttributeMapping.ConditionAttributeXml))
                {
                    oldCopyWithConditionAttributes.Add(productAttributeMapping);
                }

                //save associated value (used for combinations copying)
                associatedAttributes.Add(productAttributeMapping.Id, productAttributeMappingCopy.Id);

                // product attribute values
                var productAttributeValues = _productAttributeService.GetProductAttributeValues(productAttributeMapping.Id);
                foreach (var productAttributeValue in productAttributeValues)
                {
                    var attributeValuePictureId = 0;
                    if (originalNewPictureIdentifiers.ContainsKey(productAttributeValue.PictureId))
                    {
                        attributeValuePictureId = originalNewPictureIdentifiers[productAttributeValue.PictureId];
                    }

                    var attributeValueCopy = new ProductAttributeValue
                    {
                        ProductAttributeMappingId = productAttributeMappingCopy.Id,
                        AttributeValueTypeId = productAttributeValue.AttributeValueTypeId,
                        AssociatedProductId = productAttributeValue.AssociatedProductId,
                        Name = productAttributeValue.Name,
                        ColorSquaresRgb = productAttributeValue.ColorSquaresRgb,
                        PriceAdjustment = productAttributeValue.PriceAdjustment,
                        PriceAdjustmentUsePercentage = productAttributeValue.PriceAdjustmentUsePercentage,
                        WeightAdjustment = productAttributeValue.WeightAdjustment,
                        Cost = productAttributeValue.Cost,
                        CustomerEntersQty = productAttributeValue.CustomerEntersQty,
                        Quantity = productAttributeValue.Quantity,
                        IsPreSelected = productAttributeValue.IsPreSelected,
                        DisplayOrder = productAttributeValue.DisplayOrder,
                        PictureId = attributeValuePictureId,
                    };
                    //picture associated to "iamge square" attribute type (if exists)
                    if (productAttributeValue.ImageSquaresPictureId > 0)
                    {
                        var origImageSquaresPicture =
                            _pictureService.GetPictureById(productAttributeValue.ImageSquaresPictureId);
                        if (origImageSquaresPicture != null)
                        {
                            //copy the picture
                            var imageSquaresPictureCopy = _pictureService.InsertPicture(
                                _pictureService.LoadPictureBinary(origImageSquaresPicture),
                                origImageSquaresPicture.MimeType,
                                origImageSquaresPicture.SeoFilename,
                                origImageSquaresPicture.AltAttribute,
                                origImageSquaresPicture.TitleAttribute);
                            attributeValueCopy.ImageSquaresPictureId = imageSquaresPictureCopy.Id;
                        }
                    }

                    _productAttributeService.InsertProductAttributeValue(attributeValueCopy);

                    //save associated value (used for combinations copying)
                    associatedAttributeValues.Add(productAttributeValue.Id, attributeValueCopy.Id);

                    //localization
                    foreach (var lang in languages)
                    {
                        var name = _localizationService.GetLocalized(productAttributeValue, x => x.Name, lang.Id, false, false);
                        if (!string.IsNullOrEmpty(name))
                            _localizedEntityService.SaveLocalizedValue(attributeValueCopy, x => x.Name, name, lang.Id);
                    }
                }
            }

            //copy attribute conditions
            foreach (var productAttributeMapping in oldCopyWithConditionAttributes)
            {
                var oldConditionAttributeMapping = _productAttributeParser
                    .ParseProductAttributeMappings(productAttributeMapping.ConditionAttributeXml).FirstOrDefault();

                if (oldConditionAttributeMapping == null)
                    continue;

                var oldConditionValues =
                    _productAttributeParser.ParseProductAttributeValues(productAttributeMapping.ConditionAttributeXml,
                        oldConditionAttributeMapping.Id);

                if (!oldConditionValues.Any())
                    continue;

                var newAttributeMappingId = associatedAttributes[oldConditionAttributeMapping.Id];
                var newConditionAttributeMapping = productAttributeMappingCopies[newAttributeMappingId];

                var newConditionAttributeXml = string.Empty;

                foreach (var oldConditionValue in oldConditionValues)
                {
                    newConditionAttributeXml = _productAttributeParser.AddProductAttribute(newConditionAttributeXml,
                        newConditionAttributeMapping, associatedAttributeValues[oldConditionValue.Id].ToString());
                }

                var attributeMappingId = associatedAttributes[productAttributeMapping.Id];
                var conditionAttribute = productAttributeMappingCopies[attributeMappingId];
                conditionAttribute.ConditionAttributeXml = newConditionAttributeXml;

                _productAttributeService.UpdateProductAttributeMapping(conditionAttribute);
            }

            //attribute combinations
            foreach (var combination in _productAttributeService.GetAllProductAttributeCombinations(product.Id))
            {
                //generate new AttributesXml according to new value IDs
                var newAttributesXml = string.Empty;
                var parsedProductAttributes = _productAttributeParser.ParseProductAttributeMappings(combination.AttributesXml);
                foreach (var oldAttribute in parsedProductAttributes)
                {
                    if (!associatedAttributes.ContainsKey(oldAttribute.Id)) 
                        continue;

                    var newAttribute = _productAttributeService.GetProductAttributeMappingById(associatedAttributes[oldAttribute.Id]);

                    if (newAttribute == null) 
                        continue;

                    var oldAttributeValuesStr = _productAttributeParser.ParseValues(combination.AttributesXml, oldAttribute.Id);

                    foreach (var oldAttributeValueStr in oldAttributeValuesStr)
                    {
                        if (newAttribute.ShouldHaveValues())
                        {
                            //attribute values
                            var oldAttributeValue = int.Parse(oldAttributeValueStr);
                            if (!associatedAttributeValues.ContainsKey(oldAttributeValue)) 
                                continue;

                            var newAttributeValue = _productAttributeService.GetProductAttributeValueById(associatedAttributeValues[oldAttributeValue]);
                            
                            if (newAttributeValue != null)
                            {
                                newAttributesXml = _productAttributeParser.AddProductAttribute(newAttributesXml,
                                    newAttribute, newAttributeValue.Id.ToString());
                            }
                        }
                        else
                        {
                            //just a text
                            newAttributesXml = _productAttributeParser.AddProductAttribute(newAttributesXml,
                                newAttribute, oldAttributeValueStr);
                        }
                    }
                }

                //picture
                originalNewPictureIdentifiers.TryGetValue(combination.PictureId, out var combinationPictureId);

                var combinationCopy = new ProductAttributeCombination
                {
                    ProductId = productCopy.Id,
                    AttributesXml = newAttributesXml,
                    StockQuantity = combination.StockQuantity,
                    AllowOutOfStockOrders = combination.AllowOutOfStockOrders,
                    Sku = combination.Sku,
                    ManufacturerPartNumber = combination.ManufacturerPartNumber,
                    Gtin = combination.Gtin,
                    OverriddenPrice = combination.OverriddenPrice,
                    NotifyAdminForQuantityBelow = combination.NotifyAdminForQuantityBelow,
                    PictureId = combinationPictureId
                };
                _productAttributeService.InsertProductAttributeCombination(combinationCopy);

                //quantity change history
                _productService.AddStockQuantityHistoryEntry(productCopy, combination.StockQuantity,
                    combination.StockQuantity,
                    message: string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.CopyProduct"),
                        product.Id), combinationId: combination.Id);
            }
        }

        /// <summary>
        /// Copy product specifications
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productCopy">New product</param>
        protected virtual void CopyProductSpecifications(Product product, Product productCopy)
        {
            foreach (var productSpecificationAttribute in product.ProductSpecificationAttributes)
            {
                var psaCopy = new ProductSpecificationAttribute
                {
                    ProductId = productCopy.Id,
                    AttributeTypeId = productSpecificationAttribute.AttributeTypeId,
                    SpecificationAttributeOptionId = productSpecificationAttribute.SpecificationAttributeOptionId,
                    CustomValue = productSpecificationAttribute.CustomValue,
                    AllowFiltering = productSpecificationAttribute.AllowFiltering,
                    ShowOnProductPage = productSpecificationAttribute.ShowOnProductPage,
                    DisplayOrder = productSpecificationAttribute.DisplayOrder
                };
                _specificationAttributeService.InsertProductSpecificationAttribute(psaCopy);
            }
        }

        /// <summary>
        /// Copy crosssell mapping
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productCopy">New product</param>
        protected virtual void CopyCrossSellsMapping(Product product, Product productCopy)
        {
            foreach (var csProduct in _productService.GetCrossSellProductsByProductId1(product.Id, true))
            {
                _productService.InsertCrossSellProduct(
                    new CrossSellProduct
                    {
                        ProductId1 = productCopy.Id,
                        ProductId2 = csProduct.ProductId2
                    });
            }
        }

        /// <summary>
        /// Copy related products mapping
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productCopy">New product</param>
        protected virtual void CopyRelatedProductsMapping(Product product, Product productCopy)
        {
            foreach (var relatedProduct in _productService.GetRelatedProductsByProductId1(product.Id, true))
            {
                _productService.InsertRelatedProduct(
                    new RelatedProduct
                    {
                        ProductId1 = productCopy.Id,
                        ProductId2 = relatedProduct.ProductId2,
                        DisplayOrder = relatedProduct.DisplayOrder
                    });
            }
        }

        /// <summary>
        /// Copy manufacturer mapping
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productCopy">New product</param>
        protected virtual void CopyManufacturersMapping(Product product, Product productCopy)
        {
            foreach (var productManufacturers in product.ProductManufacturers)
            {
                var productManufacturerCopy = new ProductManufacturer
                {
                    ProductId = productCopy.Id,
                    ManufacturerId = productManufacturers.ManufacturerId,
                    IsFeaturedProduct = productManufacturers.IsFeaturedProduct,
                    DisplayOrder = productManufacturers.DisplayOrder
                };

                _manufacturerService.InsertProductManufacturer(productManufacturerCopy);
            }
        }

        /// <summary>
        /// Copy category mapping
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productCopy">New product</param>
        protected virtual void CopyCategoriesMapping(Product product, Product productCopy)
        {
            foreach (var productCategory in product.ProductCategories)
            {
                var productCategoryCopy = new ProductCategory
                {
                    ProductId = productCopy.Id,
                    CategoryId = productCategory.CategoryId,
                    IsFeaturedProduct = productCategory.IsFeaturedProduct,
                    DisplayOrder = productCategory.DisplayOrder
                };

                _categoryService.InsertProductCategory(productCategoryCopy);
            }
        }

        /// <summary>
        /// Copy warehouse mapping
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productCopy">New product</param>
        protected virtual void CopyWarehousesMapping(Product product, Product productCopy)
        {
            foreach (var pwi in product.ProductWarehouseInventory)
            {
                var pwiCopy = new ProductWarehouseInventory
                {
                    ProductId = productCopy.Id,
                    WarehouseId = pwi.WarehouseId,
                    StockQuantity = pwi.StockQuantity,
                    ReservedQuantity = 0
                };

                productCopy.ProductWarehouseInventory.Add(pwiCopy);

                //quantity change history
                var message = $"{_localizationService.GetResource("Admin.StockQuantityHistory.Messages.MultipleWarehouses")} {string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.CopyProduct"), product.Id)}";
                _productService.AddStockQuantityHistoryEntry(productCopy, pwi.StockQuantity, pwi.StockQuantity, pwi.WarehouseId, message);
            }

            _productService.UpdateProduct(productCopy);
        }

        /// <summary>
        /// Copy product pictures
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="newName">New product name</param>
        /// <param name="copyImages"></param>
        /// <param name="productCopy">New product</param>
        /// <returns>Identifiers of old and new pictures</returns>
        protected virtual Dictionary<int, int> CopyProductPictures(Product product, string newName, bool copyImages, Product productCopy)
        {
            //variable to store original and new picture identifiers
            var originalNewPictureIdentifiers = new Dictionary<int, int>();
            if (!copyImages) 
                return originalNewPictureIdentifiers;

            foreach (var productPicture in product.ProductPictures)
            {
                var picture = productPicture.Picture;
                var pictureCopy = _pictureService.InsertPicture(
                    _pictureService.LoadPictureBinary(picture),
                    picture.MimeType,
                    _pictureService.GetPictureSeName(newName),
                    picture.AltAttribute,
                    picture.TitleAttribute);
                _productService.InsertProductPicture(new ProductPicture
                {
                    ProductId = productCopy.Id,
                    PictureId = pictureCopy.Id,
                    DisplayOrder = productPicture.DisplayOrder
                });
                originalNewPictureIdentifiers.Add(picture.Id, pictureCopy.Id);
            }

            return originalNewPictureIdentifiers;
        }

        /// <summary>
        /// Copy localization data
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productCopy">New product</param>
        protected virtual void CopyLocalizationData(Product product, Product productCopy)
        {
            var languages = _languageService.GetAllLanguages(true);

            //localization
            foreach (var lang in languages)
            {
                var name = _localizationService.GetLocalized(product, x => x.Name, lang.Id, false, false);
                if (!string.IsNullOrEmpty(name))
                    _localizedEntityService.SaveLocalizedValue(productCopy, x => x.Name, name, lang.Id);

                var shortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription, lang.Id, false, false);
                if (!string.IsNullOrEmpty(shortDescription))
                    _localizedEntityService.SaveLocalizedValue(productCopy, x => x.ShortDescription, shortDescription, lang.Id);

                var fullDescription = _localizationService.GetLocalized(product, x => x.FullDescription, lang.Id, false, false);
                if (!string.IsNullOrEmpty(fullDescription))
                    _localizedEntityService.SaveLocalizedValue(productCopy, x => x.FullDescription, fullDescription, lang.Id);

                var metaKeywords = _localizationService.GetLocalized(product, x => x.MetaKeywords, lang.Id, false, false);
                if (!string.IsNullOrEmpty(metaKeywords))
                    _localizedEntityService.SaveLocalizedValue(productCopy, x => x.MetaKeywords, metaKeywords, lang.Id);

                var metaDescription = _localizationService.GetLocalized(product, x => x.MetaDescription, lang.Id, false, false);
                if (!string.IsNullOrEmpty(metaDescription))
                    _localizedEntityService.SaveLocalizedValue(productCopy, x => x.MetaDescription, metaDescription, lang.Id);

                var metaTitle = _localizationService.GetLocalized(product, x => x.MetaTitle, lang.Id, false, false);
                if (!string.IsNullOrEmpty(metaTitle))
                    _localizedEntityService.SaveLocalizedValue(productCopy, x => x.MetaTitle, metaTitle, lang.Id);

                //search engine name
                _urlRecordService.SaveSlug(productCopy, _urlRecordService.ValidateSeName(productCopy, string.Empty, name, false), lang.Id);
            }
        }

        /// <summary>
        /// Copy product
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="newName">New product name</param>
        /// <param name="isPublished">A value indicating whether a new product is published</param>
        /// <returns></returns>
        protected virtual Product CopyBaseProductData(Product product, string newName, bool isPublished)
        {
            //product download & sample download
            var downloadId = product.DownloadId;
            var sampleDownloadId = product.SampleDownloadId;
            if (product.IsDownload)
            {
                var download = _downloadService.GetDownloadById(product.DownloadId);
                if (download != null)
                {
                    var downloadCopy = new Download
                    {
                        DownloadGuid = Guid.NewGuid(),
                        UseDownloadUrl = download.UseDownloadUrl,
                        DownloadUrl = download.DownloadUrl,
                        DownloadBinary = download.DownloadBinary,
                        ContentType = download.ContentType,
                        Filename = download.Filename,
                        Extension = download.Extension,
                        IsNew = download.IsNew
                    };
                    _downloadService.InsertDownload(downloadCopy);
                    downloadId = downloadCopy.Id;
                }

                if (product.HasSampleDownload)
                {
                    var sampleDownload = _downloadService.GetDownloadById(product.SampleDownloadId);
                    if (sampleDownload != null)
                    {
                        var sampleDownloadCopy = new Download
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

            var newSku = !string.IsNullOrWhiteSpace(product.Sku)
                ? string.Format(_localizationService.GetResource("Admin.Catalog.Products.Copy.SKU.New"), product.Sku)
                : product.Sku;
            // product
            var productCopy = new Product
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
                ShowOnHomepage = product.ShowOnHomepage,
                MetaKeywords = product.MetaKeywords,
                MetaDescription = product.MetaDescription,
                MetaTitle = product.MetaTitle,
                AllowCustomerReviews = product.AllowCustomerReviews,
                LimitedToStores = product.LimitedToStores,
                Sku = newSku,
                ManufacturerPartNumber = product.ManufacturerPartNumber,
                Gtin = product.Gtin,
                IsGiftCard = product.IsGiftCard,
                GiftCardType = product.GiftCardType,
                OverriddenGiftCardAmount = product.OverriddenGiftCardAmount,
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
                IsRental = product.IsRental,
                RentalPriceLength = product.RentalPriceLength,
                RentalPricePeriod = product.RentalPricePeriod,
                IsShipEnabled = product.IsShipEnabled,
                IsFreeShipping = product.IsFreeShipping,
                ShipSeparately = product.ShipSeparately,
                AdditionalShippingCharge = product.AdditionalShippingCharge,
                DeliveryDateId = product.DeliveryDateId,
                IsTaxExempt = product.IsTaxExempt,
                TaxCategoryId = product.TaxCategoryId,
                IsTelecommunicationsOrBroadcastingOrElectronicServices =
                    product.IsTelecommunicationsOrBroadcastingOrElectronicServices,
                ManageInventoryMethod = product.ManageInventoryMethod,
                ProductAvailabilityRangeId = product.ProductAvailabilityRangeId,
                UseMultipleWarehouses = product.UseMultipleWarehouses,
                WarehouseId = product.WarehouseId,
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
                AllowAddingOnlyExistingAttributeCombinations = product.AllowAddingOnlyExistingAttributeCombinations,
                NotReturnable = product.NotReturnable,
                DisableBuyButton = product.DisableBuyButton,
                DisableWishlistButton = product.DisableWishlistButton,
                AvailableForPreOrder = product.AvailableForPreOrder,
                PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc,
                CallForPrice = product.CallForPrice,
                Price = product.Price,
                OldPrice = product.OldPrice,
                ProductCost = product.ProductCost,
                CustomerEntersPrice = product.CustomerEntersPrice,
                MinimumCustomerEnteredPrice = product.MinimumCustomerEnteredPrice,
                MaximumCustomerEnteredPrice = product.MaximumCustomerEnteredPrice,
                BasepriceEnabled = product.BasepriceEnabled,
                BasepriceAmount = product.BasepriceAmount,
                BasepriceUnitId = product.BasepriceUnitId,
                BasepriceBaseAmount = product.BasepriceBaseAmount,
                BasepriceBaseUnitId = product.BasepriceBaseUnitId,
                MarkAsNew = product.MarkAsNew,
                MarkAsNewStartDateTimeUtc = product.MarkAsNewStartDateTimeUtc,
                MarkAsNewEndDateTimeUtc = product.MarkAsNewEndDateTimeUtc,
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
            _urlRecordService.SaveSlug(productCopy, _urlRecordService.ValidateSeName(productCopy, string.Empty, productCopy.Name, true), 0);
            return productCopy;
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
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrEmpty(newName))
                throw new ArgumentException("Product name is required");

            var productCopy = CopyBaseProductData(product, newName, isPublished);

            //localization
            CopyLocalizationData(product, productCopy);

            //copy product tags
            foreach (var productProductTagMapping in product.ProductProductTagMappings)
            {
                productCopy.ProductProductTagMappings.Add(new ProductProductTagMapping { ProductTag = productProductTagMapping.ProductTag });
            }

            _productService.UpdateProduct(productCopy);

            //copy product pictures
            var originalNewPictureIdentifiers = CopyProductPictures(product, newName, copyImages, productCopy);

            //quantity change history
            _productService.AddStockQuantityHistoryEntry(productCopy, product.StockQuantity, product.StockQuantity, product.WarehouseId,
                string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.CopyProduct"), product.Id));

            //product specifications
            CopyProductSpecifications(product, productCopy);

            //product <-> warehouses mappings
            CopyWarehousesMapping(product, productCopy);
            //product <-> categories mappings
            CopyCategoriesMapping(product, productCopy);
            //product <-> manufacturers mappings
            CopyManufacturersMapping(product, productCopy);
            //product <-> related products mappings
            CopyRelatedProductsMapping(product, productCopy);
            //product <-> cross sells mappings
            CopyCrossSellsMapping(product, productCopy);
            //product <-> attributes mappings
            CopyAttributesMapping(product, productCopy, originalNewPictureIdentifiers);
            //product <-> discounts mapping
            CopyDiscountsMapping(product, productCopy);
            //store mapping
            var selectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(product);
            foreach (var id in selectedStoreIds)
            {
                _storeMappingService.InsertStoreMapping(productCopy, id);
            }

            //tier prices
            CopyTierPrices(product, productCopy);

            //update "HasTierPrices" and "HasDiscountsApplied" properties
            _productService.UpdateHasTierPricesProperty(productCopy);
            _productService.UpdateHasDiscountsApplied(productCopy);

            //associated products
            CopyAssociatedProducts(product, isPublished, copyImages, copyAssociatedProducts, productCopy);

            return productCopy;
        }

        #endregion
    }
}