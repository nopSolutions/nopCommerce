
using System;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;

using Nop.Services.Localization;
using Nop.Services.Media;

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

        #endregion

        #region Ctor

        public CopyProductService(IProductService productService,
            IProductAttributeService productAttributeService, ILanguageService languageService,
            ILocalizedEntityService localizedEntityService, IPictureService pictureService,
            ICategoryService categoryService, IManufacturerService manufacturerService,
            ISpecificationAttributeService specificationAttributeService, IDownloadService downloadService)
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
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a copy of product with all depended data
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="newName">The name of product duplicate</param>
        /// <param name="isPublished">A value indicating whether the product duplicate should be published</param>
        /// <param name="copyImages">A value indicating whether the product images should be copied</param>
        /// <returns>Product entity</returns>
        public Product CopyProduct(int productId, string newName, bool isPublished, bool copyImages)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id", "productId");

            Product productCopy = null;
            //uncomment this line to support transactions
            //using (var scope = new System.Transactions.TransactionScope())
            {
                // product
                productCopy = new Product()
                {
                    Name = newName,
                    ShortDescription = product.ShortDescription,
                    FullDescription = product.FullDescription,
                    AdminComment = product.AdminComment,
                    ShowOnHomePage = product.ShowOnHomePage,
                    MetaKeywords = product.MetaKeywords,
                    MetaDescription = product.MetaDescription,
                    MetaTitle = product.MetaTitle,
                    SeName = product.SeName,
                    AllowCustomerReviews = product.AllowCustomerReviews,
                    Published = isPublished,
                    Deleted = product.Deleted,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };
                _productService.InsertProduct(productCopy);

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

                    var seName = product.GetLocalized(x => x.SeName, lang.Id, false, false);
                    if (!String.IsNullOrEmpty(seName))
                        _localizedEntityService.SaveLocalizedValue(productCopy, x => x.SeName, seName, lang.Id);
                }

                // product pictures
                if (copyImages)
                {
                    foreach (var productPicture in product.ProductPictures)
                    {
                        var picture = productPicture.Picture;

                        var pictureCopy = _pictureService.InsertPicture(picture.PictureBinary, picture.MimeType, true);
                        _productService.InsertProductPicture(new ProductPicture()
                        {
                            ProductId = productCopy.Id,
                            PictureId = pictureCopy.Id,
                            DisplayOrder = productPicture.DisplayOrder
                        });
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
                        AllowFiltering = productSpecificationAttribute.AllowFiltering,
                        ShowOnProductPage = productSpecificationAttribute.ShowOnProductPage,
                        DisplayOrder = productSpecificationAttribute.DisplayOrder
                    };
                    _specificationAttributeService.InsertProductSpecificationAttribute(psaCopy);
                }

                // product variants
                var productVariants = product.ProductVariants;
                foreach (var productVariant in productVariants)
                {
                    // product variant picture
                    int pictureId = 0;
                    if (copyImages)
                    {
                        var picture = _pictureService.GetPictureById(productVariant.PictureId);
                        if (picture != null)
                        {
                            var pictureCopy = _pictureService.InsertPicture(picture.PictureBinary, picture.MimeType, true);
                            pictureId = pictureCopy.Id;
                        }
                    }

                    // product variant download & sample download
                    int downloadId = productVariant.DownloadId;
                    int sampleDownloadId = productVariant.SampleDownloadId;
                    if (productVariant.IsDownload)
                    {
                        var download = _downloadService.GetDownloadById(productVariant.DownloadId);
                        if (download != null)
                        {
                            var downloadCopy = new Download()
                            {
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

                        if (productVariant.HasSampleDownload)
                        {
                            var sampleDownload = _downloadService.GetDownloadById(productVariant.SampleDownloadId);
                            if (sampleDownload != null)
                            {
                                var sampleDownloadCopy = new Download()
                                {
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

                    // product variant
                    var productVariantCopy = new ProductVariant()
                    {
                        ProductId = productCopy.Id,
                        Name = productVariant.Name,
                        Sku = productVariant.Sku,
                        Description = productVariant.Description,
                        AdminComment = productVariant.AdminComment,
                        ManufacturerPartNumber = productVariant.ManufacturerPartNumber,
                        IsGiftCard = productVariant.IsGiftCard,
                        GiftCardType = productVariant.GiftCardType,
                        IsDownload = productVariant.IsDownload,
                        DownloadId = downloadId,
                        UnlimitedDownloads = productVariant.UnlimitedDownloads,
                        MaxNumberOfDownloads = productVariant.MaxNumberOfDownloads,
                        DownloadExpirationDays = productVariant.DownloadExpirationDays,
                        DownloadActivationType = productVariant.DownloadActivationType,
                        HasSampleDownload = productVariant.HasSampleDownload,
                        SampleDownloadId = sampleDownloadId,
                        HasUserAgreement = productVariant.HasUserAgreement,
                        UserAgreementText = productVariant.UserAgreementText,
                        IsRecurring = productVariant.IsRecurring,
                        RecurringCycleLength = productVariant.RecurringCycleLength,
                        RecurringCyclePeriod = productVariant.RecurringCyclePeriod,
                        RecurringTotalCycles = productVariant.RecurringTotalCycles,
                        IsShipEnabled = productVariant.IsShipEnabled,
                        IsFreeShipping = productVariant.IsFreeShipping,
                        AdditionalShippingCharge = productVariant.AdditionalShippingCharge,
                        IsTaxExempt = productVariant.IsTaxExempt,
                        TaxCategoryId = productVariant.TaxCategoryId,
                        ManageInventoryMethod = productVariant.ManageInventoryMethod,
                        StockQuantity = productVariant.StockQuantity,
                        DisplayStockAvailability = productVariant.DisplayStockAvailability,
                        DisplayStockQuantity = productVariant.DisplayStockQuantity,
                        MinStockQuantity = productVariant.MinStockQuantity,
                        LowStockActivityId = productVariant.LowStockActivityId,
                        NotifyAdminForQuantityBelow = productVariant.NotifyAdminForQuantityBelow,
                        BackorderMode = productVariant.BackorderMode,
                        OrderMinimumQuantity = productVariant.OrderMinimumQuantity,
                        OrderMaximumQuantity = productVariant.OrderMaximumQuantity,
                        DisableBuyButton = productVariant.DisableBuyButton,
                        CallForPrice = productVariant.CallForPrice,
                        Price = productVariant.Price,
                        OldPrice = productVariant.OldPrice,
                        ProductCost = productVariant.ProductCost,
                        CustomerEntersPrice = productVariant.CustomerEntersPrice,
                        MinimumCustomerEnteredPrice = productVariant.MinimumCustomerEnteredPrice,
                        MaximumCustomerEnteredPrice = productVariant.MaximumCustomerEnteredPrice,
                        Weight = productVariant.Weight,
                        Length = productVariant.Length,
                        Width = productVariant.Width,
                        Height = productVariant.Height,
                        PictureId = pictureId,
                        AvailableStartDateTimeUtc = productVariant.AvailableStartDateTimeUtc,
                        AvailableEndDateTimeUtc = productVariant.AvailableEndDateTimeUtc,
                        Published = productVariant.Published,
                        Deleted = productVariant.Deleted,
                        DisplayOrder = productVariant.DisplayOrder,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };

                    _productService.InsertProductVariant(productVariantCopy);

                    //localization
                    foreach (var lang in languages)
                    {
                        var name = productVariant.GetLocalized(x => x.Name, lang.Id, false, false);
                        if (!String.IsNullOrEmpty(name))
                            _localizedEntityService.SaveLocalizedValue(productVariantCopy, x => x.Name, name, lang.Id);

                        var description = productVariant.GetLocalized(x => x.Description, lang.Id, false, false);
                        if (!String.IsNullOrEmpty(description))
                            _localizedEntityService.SaveLocalizedValue(productVariantCopy, x => x.Description, description, lang.Id);
                    }

                    // product variant <-> attributes mappings
                    foreach (var productVariantAttribute in _productAttributeService.GetProductVariantAttributesByProductVariantId(productVariant.Id))
                    {
                        var productVariantAttributeCopy = new ProductVariantAttribute()
                        {
                            ProductVariantId = productVariantCopy.Id,
                            ProductAttributeId = productVariantAttribute.ProductAttributeId,
                            TextPrompt = productVariantAttribute.TextPrompt,
                            IsRequired = productVariantAttribute.IsRequired,
                            AttributeControlTypeId = productVariantAttribute.AttributeControlTypeId,
                            DisplayOrder = productVariantAttribute.DisplayOrder
                        };
                        _productAttributeService.InsertProductVariantAttribute(productVariantAttributeCopy);

                        // product variant attribute values
                        var productVariantAttributeValues = _productAttributeService.GetProductVariantAttributeValues(productVariantAttribute.Id);
                        foreach (var productVariantAttributeValue in productVariantAttributeValues)
                        {
                            var pvavCopy = new ProductVariantAttributeValue()
                            {
                                ProductVariantAttributeId = productVariantAttributeCopy.Id,
                                Name = productVariantAttributeValue.Name,
                                PriceAdjustment = productVariantAttributeValue.PriceAdjustment,
                                WeightAdjustment = productVariantAttributeValue.WeightAdjustment,
                                IsPreSelected = productVariantAttributeValue.IsPreSelected,
                                DisplayOrder = productVariantAttributeValue.DisplayOrder
                            };
                            _productAttributeService.InsertProductVariantAttributeValue(pvavCopy);

                            //localization
                            foreach (var lang in languages)
                            {
                                var name = productVariantAttributeValue.GetLocalized(x => x.Name, lang.Id, false, false);
                                if (!String.IsNullOrEmpty(name))
                                    _localizedEntityService.SaveLocalizedValue(pvavCopy, x => x.Name, name, lang.Id);
                            }
                        }
                    }
                    foreach (var combination in _productAttributeService.GetAllProductVariantAttributeCombinations(productVariant.Id))
                    {
                        var combinationCopy = new ProductVariantAttributeCombination()
                        {
                            ProductVariantId = productVariantCopy.Id,
                            AttributesXml = combination.AttributesXml,
                            StockQuantity = combination.StockQuantity,
                            AllowOutOfStockOrders = combination.AllowOutOfStockOrders
                        };
                        _productAttributeService.InsertProductVariantAttributeCombination(combinationCopy);
                    }

                    // product variant tier prices
                    foreach (var tierPrice in productVariant.TierPrices)
                    {
                        _productService.InsertTierPrice(
                            new TierPrice()
                            {
                                ProductVariantId = productVariantCopy.Id,
                                CustomerRoleId = tierPrice.CustomerRoleId,
                                Quantity = tierPrice.Quantity,
                                Price = tierPrice.Price
                            });
                    }

                    // product variant <-> discounts mapping
                    foreach (var discount in productVariant.AppliedDiscounts)
                    {
                        productVariantCopy.AppliedDiscounts.Add(discount);
                        _productService.UpdateProductVariant(productVariantCopy);
                    }
                }

                //uncomment this line to support transactions
                //scope.Complete();
            }

            return productCopy;
        }

        #endregion
    }
}
