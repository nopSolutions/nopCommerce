using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping.Date;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the product model factory
    /// </summary>
    public partial class ProductModelFactory : IProductModelFactory
    {
        #region Fields

        protected CaptchaSettings CaptchaSettings { get; }
        protected CatalogSettings CatalogSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected ICategoryService CategoryService { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateRangeService DateRangeService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IDownloadService DownloadService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IManufacturerService ManufacturerService { get; }
        protected IPermissionService PermissionService { get; }
        protected IPictureService PictureService { get; }
        protected IPriceCalculationService PriceCalculationService { get; }
        protected IPriceFormatter PriceFormatter { get; }
        protected IProductAttributeParser ProductAttributeParser { get; }
        protected IProductAttributeService ProductAttributeService { get; }
        protected IProductService ProductService { get; }
        protected IProductTagService ProductTagService { get; }
        protected IProductTemplateService ProductTemplateService { get; }
        protected IReviewTypeService ReviewTypeService { get; }
        protected ISpecificationAttributeService SpecificationAttributeService { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IStoreContext StoreContext { get; }
        protected IShoppingCartModelFactory ShoppingCartModelFactory { get; }
        protected ITaxService TaxService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IVendorService VendorService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }
        protected MediaSettings MediaSettings { get; }
        protected OrderSettings OrderSettings { get; }
        protected SeoSettings SeoSettings { get; }
        protected ShippingSettings ShippingSettings { get; }
        protected VendorSettings VendorSettings { get; }

        #endregion

        #region Ctor

        public ProductModelFactory(CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CustomerSettings customerSettings,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IProductTagService productTagService,
            IProductTemplateService productTemplateService,
            IReviewTypeService reviewTypeService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IShoppingCartModelFactory shoppingCartModelFactory,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            SeoSettings seoSettings,
            ShippingSettings shippingSettings,
            VendorSettings vendorSettings)
        {
            CaptchaSettings = captchaSettings;
            CatalogSettings = catalogSettings;
            CustomerSettings = customerSettings;
            CategoryService = categoryService;
            CurrencyService = currencyService;
            CustomerService = customerService;
            DateRangeService = dateRangeService;
            DateTimeHelper = dateTimeHelper;
            DownloadService = downloadService;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            ManufacturerService = manufacturerService;
            PermissionService = permissionService;
            PictureService = pictureService;
            PriceCalculationService = priceCalculationService;
            PriceFormatter = priceFormatter;
            ProductAttributeParser = productAttributeParser;
            ProductAttributeService = productAttributeService;
            ProductService = productService;
            ProductTagService = productTagService;
            ProductTemplateService = productTemplateService;
            ReviewTypeService = reviewTypeService;
            SpecificationAttributeService = specificationAttributeService;
            StaticCacheManager = staticCacheManager;
            StoreContext = storeContext;
            ShoppingCartModelFactory = shoppingCartModelFactory;
            TaxService = taxService;
            UrlRecordService = urlRecordService;
            VendorService = vendorService;
            WebHelper = webHelper;
            WorkContext = workContext;
            MediaSettings = mediaSettings;
            OrderSettings = orderSettings;
            SeoSettings = seoSettings;
            ShippingSettings = shippingSettings;
            VendorSettings = vendorSettings;

        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the product specification models
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="group">Specification attribute group</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of product specification model
        /// </returns>
        protected virtual async Task<IList<ProductSpecificationAttributeModel>> PrepareProductSpecificationAttributeModelAsync(Product product, SpecificationAttributeGroup group)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var productSpecificationAttributes = await SpecificationAttributeService.GetProductSpecificationAttributesAsync(
                    product.Id, specificationAttributeGroupId: group?.Id, showOnProductPage: true);

            var result = new List<ProductSpecificationAttributeModel>();

            foreach (var psa in productSpecificationAttributes)
            {
                var option = await SpecificationAttributeService.GetSpecificationAttributeOptionByIdAsync(psa.SpecificationAttributeOptionId);

                var model = result.FirstOrDefault(model => model.Id == option.SpecificationAttributeId);
                if (model == null)
                {
                    var attribute = await SpecificationAttributeService.GetSpecificationAttributeByIdAsync(option.SpecificationAttributeId);
                    model = new ProductSpecificationAttributeModel
                    {
                        Id = attribute.Id,
                        Name = await LocalizationService.GetLocalizedAsync(attribute, x => x.Name)
                    };
                    result.Add(model);
                }

                var value = new ProductSpecificationAttributeValueModel
                {
                    AttributeTypeId = psa.AttributeTypeId,
                    ColorSquaresRgb = option.ColorSquaresRgb,
                    ValueRaw = psa.AttributeType switch
                    {
                        SpecificationAttributeType.Option => WebUtility.HtmlEncode(await LocalizationService.GetLocalizedAsync(option, x => x.Name)),
                        SpecificationAttributeType.CustomText => WebUtility.HtmlEncode(await LocalizationService.GetLocalizedAsync(psa, x => x.CustomValue)),
                        SpecificationAttributeType.CustomHtmlText => await LocalizationService.GetLocalizedAsync(psa, x => x.CustomValue),
                        SpecificationAttributeType.Hyperlink => $"<a href='{psa.CustomValue}' target='_blank'>{psa.CustomValue}</a>",
                        _ => null
                    }
                };

                model.Values.Add(value);
            }

            return result;
        }

        /// <summary>
        /// Prepare the product review overview model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product review overview model
        /// </returns>
        protected virtual async Task<ProductReviewOverviewModel> PrepareProductReviewOverviewModelAsync(Product product)
        {
            ProductReviewOverviewModel productReview;
            var currentStore = await StoreContext.GetCurrentStoreAsync();

            if (CatalogSettings.ShowProductReviewsPerStore)
            {
                var cacheKey = StaticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductReviewsModelKey, product, currentStore);

                productReview = await StaticCacheManager.GetAsync(cacheKey, async () =>
                {
                    var productReviews = await ProductService.GetAllProductReviewsAsync(productId: product.Id, approved: true, storeId: currentStore.Id);

                    return new ProductReviewOverviewModel
                    {
                        RatingSum = productReviews.Sum(pr => pr.Rating),
                        TotalReviews = productReviews.Count
                    };
                });
            }
            else
            {
                productReview = new ProductReviewOverviewModel
                {
                    RatingSum = product.ApprovedRatingSum,
                    TotalReviews = product.ApprovedTotalReviews
                };
            }

            if (productReview != null)
            {
                productReview.ProductId = product.Id;
                productReview.AllowCustomerReviews = product.AllowCustomerReviews;
                productReview.CanAddNewReview = await ProductService.CanAddReviewAsync(product.Id, CatalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0);
            }

            return productReview;
        }

        /// <summary>
        /// Prepare the product overview price model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product overview price model
        /// </returns>
        protected virtual async Task<ProductOverviewModel.ProductPriceModel> PrepareProductOverviewPriceModelAsync(Product product, bool forceRedirectionAfterAddingToCart = false)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var priceModel = new ProductOverviewModel.ProductPriceModel
            {
                ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
            };

            switch (product.ProductType)
            {
                case ProductType.GroupedProduct:
                    //grouped product
                    await PrepareGroupedProductOverviewPriceModelAsync(product, priceModel);

                    break;
                case ProductType.SimpleProduct:
                default:
                    //simple product
                    await PrepareSimpleProductOverviewPriceModelAsync(product, priceModel);

                    break;
            }

            return priceModel;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareSimpleProductOverviewPriceModelAsync(Product product, ProductOverviewModel.ProductPriceModel priceModel)
        {
            //add to cart button
            priceModel.DisableBuyButton = product.DisableBuyButton ||
                                          !await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart) ||
                                          !await PermissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);

            //add to wishlist button
            priceModel.DisableWishlistButton = product.DisableWishlistButton ||
                                               !await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) ||
                                               !await PermissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);
            //compare products
            priceModel.DisableAddToCompareListButton = !CatalogSettings.CompareProductsEnabled;

            //rental
            priceModel.IsRental = product.IsRental;

            //pre-order
            if (product.AvailableForPreOrder)
            {
                priceModel.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                                                  product.PreOrderAvailabilityStartDateTimeUtc.Value >=
                                                  DateTime.UtcNow;
                priceModel.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
            }

            //prices
            if (await PermissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                if (product.CustomerEntersPrice)
                    return;

                if (product.CallForPrice &&
                    //also check whether the current user is impersonated
                    (!OrderSettings.AllowAdminsToBuyCallForPriceProducts ||
                     WorkContext.OriginalCustomerIfImpersonated == null))
                {
                    //call for price
                    priceModel.OldPrice = null;
                    priceModel.OldPriceValue = null;
                    priceModel.Price = await LocalizationService.GetResourceAsync("Products.CallForPrice");
                    priceModel.PriceValue = null;
                }
                else
                {
                    //prices
                    var customer = await WorkContext.GetCurrentCustomerAsync();
                    var (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await PriceCalculationService.GetFinalPriceAsync(product, customer);

                    if (product.HasTierPrices)
                    {
                        var (tierPriceMinPossiblePriceWithoutDiscount, tierPriceMinPossiblePriceWithDiscount, _, _) = await PriceCalculationService.GetFinalPriceAsync(product, customer, quantity: int.MaxValue);

                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                        minPossiblePriceWithoutDiscount = Math.Min(minPossiblePriceWithoutDiscount, tierPriceMinPossiblePriceWithoutDiscount);
                        minPossiblePriceWithDiscount = Math.Min(minPossiblePriceWithDiscount, tierPriceMinPossiblePriceWithDiscount);
                    }

                    var (oldPriceBase, _) = await TaxService.GetProductPriceAsync(product, product.OldPrice);
                    var (finalPriceWithoutDiscountBase, _) = await TaxService.GetProductPriceAsync(product, minPossiblePriceWithoutDiscount);
                    var (finalPriceWithDiscountBase, _) = await TaxService.GetProductPriceAsync(product, minPossiblePriceWithDiscount);
                    var currentCurrency = await WorkContext.GetWorkingCurrencyAsync();
                    var oldPrice = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(oldPriceBase, currentCurrency);
                    var finalPriceWithoutDiscount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithoutDiscountBase, currentCurrency);
                    var finalPriceWithDiscount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, currentCurrency);

                    //do we have tier prices configured?
                    var tierPrices = new List<TierPrice>();
                    if (product.HasTierPrices)
                    {
                        var store = await StoreContext.GetCurrentStoreAsync();
                        tierPrices.AddRange(await ProductService.GetTierPricesAsync(product, customer, store.Id));
                    }
                    //When there is just one tier price (with  qty 1), there are no actual savings in the list.
                    var displayFromMessage = tierPrices.Any() && !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);
                    if (displayFromMessage)
                    {
                        priceModel.OldPrice = null;
                        priceModel.OldPriceValue = null;
                        priceModel.Price = string.Format(await LocalizationService.GetResourceAsync("Products.PriceRangeFrom"), await PriceFormatter.FormatPriceAsync(finalPriceWithDiscount));
                        priceModel.PriceValue = finalPriceWithDiscount;
                    }
                    else
                    {
                        var strikeThroughPrice = decimal.Zero;

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                            strikeThroughPrice = oldPrice;

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                            strikeThroughPrice = finalPriceWithoutDiscount;

                        if (strikeThroughPrice > decimal.Zero)
                        {
                            priceModel.OldPrice = await PriceFormatter.FormatPriceAsync(strikeThroughPrice);
                            priceModel.OldPriceValue = strikeThroughPrice;
                        }

                        priceModel.Price = await PriceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                        priceModel.PriceValue = finalPriceWithDiscount;
                    }

                    if (product.IsRental)
                    {
                        //rental product
                        priceModel.OldPrice = await PriceFormatter.FormatRentalProductPeriodAsync(product, priceModel.OldPrice);
                        priceModel.OldPriceValue = priceModel.OldPriceValue;
                        priceModel.Price = await PriceFormatter.FormatRentalProductPeriodAsync(product, priceModel.Price);
                        priceModel.PriceValue = priceModel.PriceValue;
                    }

                    //property for German market
                    //we display tax/shipping info only with "shipping enabled" for this product
                    //we also ensure this it's not free shipping
                    priceModel.DisplayTaxShippingInfo = CatalogSettings.DisplayTaxShippingInfoProductBoxes && product.IsShipEnabled && !product.IsFreeShipping;

                    //PAngV default baseprice (used in Germany)
                    priceModel.BasePricePAngV = await PriceFormatter.FormatBasePriceAsync(product, finalPriceWithDiscount);
                    priceModel.BasePricePAngVValue = finalPriceWithDiscount;
                }
            }
            else
            {
                //hide prices
                priceModel.OldPrice = null;
                priceModel.OldPriceValue = null;
                priceModel.Price = null;
                priceModel.PriceValue = null;
            }
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareGroupedProductOverviewPriceModelAsync(Product product, ProductOverviewModel.ProductPriceModel priceModel)
        {
            var store = await StoreContext.GetCurrentStoreAsync();
            var associatedProducts = await ProductService.GetAssociatedProductsAsync(product.Id,
                store.Id);

            //add to cart button (ignore "DisableBuyButton" property for grouped products)
            priceModel.DisableBuyButton =
                !await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart) ||
                !await PermissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);

            //add to wishlist button (ignore "DisableWishlistButton" property for grouped products)
            priceModel.DisableWishlistButton =
                !await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) ||
                !await PermissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);

            //compare products
            priceModel.DisableAddToCompareListButton = !CatalogSettings.CompareProductsEnabled;
            if (!associatedProducts.Any())
                return;

            //we have at least one associated product
            if (await PermissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                //find a minimum possible price
                decimal? minPossiblePrice = null;
                Product minPriceProduct = null;
                var customer = await WorkContext.GetCurrentCustomerAsync();
                foreach (var associatedProduct in associatedProducts)
                {
                    var (_, tmpMinPossiblePrice, _, _) = await PriceCalculationService.GetFinalPriceAsync(associatedProduct, customer);

                    if (associatedProduct.HasTierPrices)
                    {
                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                        tmpMinPossiblePrice = Math.Min(tmpMinPossiblePrice,
                            (await PriceCalculationService.GetFinalPriceAsync(associatedProduct, customer, quantity: int.MaxValue)).priceWithoutDiscounts);
                    }

                    if (minPossiblePrice.HasValue && tmpMinPossiblePrice >= minPossiblePrice.Value)
                        continue;
                    minPriceProduct = associatedProduct;
                    minPossiblePrice = tmpMinPossiblePrice;
                }

                if (minPriceProduct == null || minPriceProduct.CustomerEntersPrice)
                    return;

                if (minPriceProduct.CallForPrice &&
                    //also check whether the current user is impersonated
                    (!OrderSettings.AllowAdminsToBuyCallForPriceProducts ||
                     WorkContext.OriginalCustomerIfImpersonated == null))
                {
                    priceModel.OldPrice = null;
                    priceModel.OldPriceValue = null;
                    priceModel.Price = await LocalizationService.GetResourceAsync("Products.CallForPrice");
                    priceModel.PriceValue = null;
                }
                else
                {
                    //calculate prices
                    var (finalPriceBase, _) = await TaxService.GetProductPriceAsync(minPriceProduct, minPossiblePrice.Value);
                    var finalPrice = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceBase, await WorkContext.GetWorkingCurrencyAsync());

                    priceModel.OldPrice = null;
                    priceModel.OldPriceValue = null;
                    priceModel.Price = string.Format(await LocalizationService.GetResourceAsync("Products.PriceRangeFrom"), await PriceFormatter.FormatPriceAsync(finalPrice));
                    priceModel.PriceValue = finalPrice;

                    //PAngV default baseprice (used in Germany)
                    priceModel.BasePricePAngV = await PriceFormatter.FormatBasePriceAsync(product, finalPriceBase);
                    priceModel.BasePricePAngVValue = finalPriceBase;
                }
            }
            else
            {
                //hide prices
                priceModel.OldPrice = null;
                priceModel.OldPriceValue = null;
                priceModel.Price = null;
                priceModel.PriceValue = null;
            }
        }

        /// <summary>
        /// Prepare the product overview picture model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productThumbPictureSize">Product thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture model
        /// </returns>
        protected virtual async Task<PictureModel> PrepareProductOverviewPictureModelAsync(Product product, int? productThumbPictureSize = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var productName = await LocalizationService.GetLocalizedAsync(product, x => x.Name);
            //If a size has been set in the view, we use it in priority
            var pictureSize = productThumbPictureSize ?? MediaSettings.ProductThumbPictureSize;

            //prepare picture model
            var cacheKey = StaticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductDefaultPictureModelKey,
                product, pictureSize, true, await WorkContext.GetWorkingLanguageAsync(), WebHelper.IsCurrentConnectionSecured(),
                await StoreContext.GetCurrentStoreAsync());

            var defaultPictureModel = await StaticCacheManager.GetAsync(cacheKey, async () =>
            {
                var picture = (await PictureService.GetPicturesByProductIdAsync(product.Id, 1)).FirstOrDefault();
                string fullSizeImageUrl, imageUrl;
                (imageUrl, picture) = await PictureService.GetPictureUrlAsync(picture, pictureSize);
                (fullSizeImageUrl, picture) = await PictureService.GetPictureUrlAsync(picture);

                var pictureModel = new PictureModel
                {
                    ImageUrl = imageUrl,
                    FullSizeImageUrl = fullSizeImageUrl,
                    //"title" attribute
                    Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                        ? picture.TitleAttribute
                        : string.Format(await LocalizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat"),
                            productName),
                    //"alt" attribute
                    AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                        ? picture.AltAttribute
                        : string.Format(await LocalizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat"),
                            productName)
                };

                return pictureModel;
            });

            return defaultPictureModel;
        }

        /// <summary>
        /// Prepare the product breadcrumb model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product breadcrumb model
        /// </returns>
        protected virtual async Task<ProductDetailsModel.ProductBreadcrumbModel> PrepareProductBreadcrumbModelAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var breadcrumbModel = new ProductDetailsModel.ProductBreadcrumbModel
            {
                Enabled = CatalogSettings.CategoryBreadcrumbEnabled,
                ProductId = product.Id,
                ProductName = await LocalizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await UrlRecordService.GetSeNameAsync(product)
            };
            var productCategories = await CategoryService.GetProductCategoriesByProductIdAsync(product.Id);
            if (!productCategories.Any())
                return breadcrumbModel;

            var category = await CategoryService.GetCategoryByIdAsync(productCategories[0].CategoryId);
            if (category == null)
                return breadcrumbModel;

            foreach (var catBr in await CategoryService.GetCategoryBreadCrumbAsync(category))
            {
                breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                {
                    Id = catBr.Id,
                    Name = await LocalizationService.GetLocalizedAsync(catBr, x => x.Name),
                    SeName = await UrlRecordService.GetSeNameAsync(catBr),
                    IncludeInTopMenu = catBr.IncludeInTopMenu
                });
            }

            return breadcrumbModel;
        }

        /// <summary>
        /// Prepare the product tag models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of product tag model
        /// </returns>
        protected virtual async Task<IList<ProductTagModel>> PrepareProductTagModelsAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var store = await StoreContext.GetCurrentStoreAsync();
            var productsTags = await ProductTagService.GetAllProductTagsByProductIdAsync(product.Id);

            var model = await productsTags
                    //filter by store
                    .WhereAwait(async x => await ProductTagService.GetProductCountByProductTagIdAsync(x.Id, store.Id) > 0)
                    .SelectAwait(async x => new ProductTagModel
                    {
                        Id = x.Id,
                        Name = await LocalizationService.GetLocalizedAsync(x, y => y.Name),
                        SeName = await UrlRecordService.GetSeNameAsync(x),
                        ProductCount = await ProductTagService.GetProductCountByProductTagIdAsync(x.Id, store.Id)
                    }).ToListAsync();

            return model;
        }

        /// <summary>
        /// Prepare the product price model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product price model
        /// </returns>
        protected virtual async Task<ProductDetailsModel.ProductPriceModel> PrepareProductPriceModelAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new ProductDetailsModel.ProductPriceModel
            {
                ProductId = product.Id
            };

            if (await PermissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                model.HidePrices = false;
                if (product.CustomerEntersPrice)
                {
                    model.CustomerEntersPrice = true;
                }
                else
                {
                    if (product.CallForPrice &&
                        //also check whether the current user is impersonated
                        (!OrderSettings.AllowAdminsToBuyCallForPriceProducts || WorkContext.OriginalCustomerIfImpersonated == null))
                    {
                        model.CallForPrice = true;
                    }
                    else
                    {
                        var customer = await WorkContext.GetCurrentCustomerAsync();
                        var (oldPriceBase, _) = await TaxService.GetProductPriceAsync(product, product.OldPrice);
                        var (finalPriceWithoutDiscountBase, _) = await TaxService.GetProductPriceAsync(product, (await PriceCalculationService.GetFinalPriceAsync(product, customer, includeDiscounts: false)).finalPrice);
                        var (finalPriceWithDiscountBase, _) = await TaxService.GetProductPriceAsync(product, (await PriceCalculationService.GetFinalPriceAsync(product, customer)).finalPrice);
                        var currentCurrency = await WorkContext.GetWorkingCurrencyAsync();
                        var oldPrice = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(oldPriceBase, currentCurrency);
                        var finalPriceWithoutDiscount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithoutDiscountBase, currentCurrency);
                        var finalPriceWithDiscount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, currentCurrency);

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                        {
                            model.OldPrice = await PriceFormatter.FormatPriceAsync(oldPrice);
                            model.OldPriceValue = oldPrice;
                        }

                        model.Price = await PriceFormatter.FormatPriceAsync(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                        {
                            model.PriceWithDiscount = await PriceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                            model.PriceWithDiscountValue = finalPriceWithDiscount;
                        }

                        model.PriceValue = finalPriceWithDiscount;

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this product
                        //we also ensure this it's not free shipping
                        model.DisplayTaxShippingInfo = CatalogSettings.DisplayTaxShippingInfoProductDetailsPage
                            && product.IsShipEnabled &&
                            !product.IsFreeShipping;

                        //PAngV baseprice (used in Germany)
                        model.BasePricePAngV = await PriceFormatter.FormatBasePriceAsync(product, finalPriceWithDiscountBase);
                        model.BasePricePAngVValue = finalPriceWithDiscountBase;
                        //currency code
                        model.CurrencyCode = (await WorkContext.GetWorkingCurrencyAsync()).CurrencyCode;

                        //rental
                        if (product.IsRental)
                        {
                            model.IsRental = true;
                            var priceStr = await PriceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                            model.RentalPrice = await PriceFormatter.FormatRentalProductPeriodAsync(product, priceStr);
                            model.RentalPriceValue = finalPriceWithDiscount;
                        }
                    }
                }
            }
            else
            {
                model.HidePrices = true;
                model.OldPrice = null;
                model.OldPriceValue = null;
                model.Price = null;
            }

            return model;
        }

        /// <summary>
        /// Prepare the product add to cart model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product add to cart model
        /// </returns>
        protected virtual async Task<ProductDetailsModel.AddToCartModel> PrepareProductAddToCartModelAsync(Product product, ShoppingCartItem updatecartitem)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new ProductDetailsModel.AddToCartModel
            {
                ProductId = product.Id
            };

            if (updatecartitem != null)
            {
                model.UpdatedShoppingCartItemId = updatecartitem.Id;
                model.UpdateShoppingCartItemType = updatecartitem.ShoppingCartType;
            }

            //quantity
            model.EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : product.OrderMinimumQuantity;
            //allowed quantities
            var allowedQuantities = ProductService.ParseAllowedQuantities(product);
            foreach (var qty in allowedQuantities)
            {
                model.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = updatecartitem != null && updatecartitem.Quantity == qty
                });
            }
            //minimum quantity notification
            if (product.OrderMinimumQuantity > 1)
            {
                model.MinimumQuantityNotification = string.Format(await LocalizationService.GetResourceAsync("Products.MinimumQuantityNotification"), product.OrderMinimumQuantity);
            }

            //'add to cart', 'add to wishlist' buttons
            model.DisableBuyButton = product.DisableBuyButton || !await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart);
            model.DisableWishlistButton = product.DisableWishlistButton || !await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist);
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                model.DisableBuyButton = true;
                model.DisableWishlistButton = true;
            }
            //pre-order
            if (product.AvailableForPreOrder)
            {
                model.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                    product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                model.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;

                if (model.AvailableForPreOrder &&
                    model.PreOrderAvailabilityStartDateTimeUtc.HasValue &&
                    CatalogSettings.DisplayDatePreOrderAvailability)
                {
                    model.PreOrderAvailabilityStartDateTimeUserTime =
                        (await DateTimeHelper.ConvertToUserTimeAsync(model.PreOrderAvailabilityStartDateTimeUtc.Value)).ToString("D");
                }
            }
            //rental
            model.IsRental = product.IsRental;

            //customer entered price
            model.CustomerEntersPrice = product.CustomerEntersPrice;
            if (!model.CustomerEntersPrice)
                return model;

            var currentCurrency = await WorkContext.GetWorkingCurrencyAsync();
            var minimumCustomerEnteredPrice = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MinimumCustomerEnteredPrice, currentCurrency);
            var maximumCustomerEnteredPrice = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MaximumCustomerEnteredPrice, currentCurrency);

            model.CustomerEnteredPrice = updatecartitem != null ? updatecartitem.CustomerEnteredPrice : minimumCustomerEnteredPrice;
            model.CustomerEnteredPriceRange = string.Format(await LocalizationService.GetResourceAsync("Products.EnterProductPrice.Range"),
                await PriceFormatter.FormatPriceAsync(minimumCustomerEnteredPrice, false, false),
                await PriceFormatter.FormatPriceAsync(maximumCustomerEnteredPrice, false, false));

            return model;
        }

        /// <summary>
        /// Prepare the product attribute models
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of product attribute model
        /// </returns>
        protected virtual async Task<IList<ProductDetailsModel.ProductAttributeModel>> PrepareProductAttributeModelsAsync(Product product, ShoppingCartItem updatecartitem)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new List<ProductDetailsModel.ProductAttributeModel>();

            var productAttributeMapping = await ProductAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            foreach (var attribute in productAttributeMapping)
            {
                var productAttrubute = await ProductAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId);

                var attributeModel = new ProductDetailsModel.ProductAttributeModel
                {
                    Id = attribute.Id,
                    ProductId = product.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = await LocalizationService.GetLocalizedAsync(productAttrubute, x => x.Name),
                    Description = await LocalizationService.GetLocalizedAsync(productAttrubute, x => x.Description),
                    TextPrompt = await LocalizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = updatecartitem != null ? null : await LocalizationService.GetLocalizedAsync(attribute, x => x.DefaultValue),
                    HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml)
                };
                if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await ProductAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new ProductDetailsModel.ProductAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = await LocalizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                            IsPreSelected = attributeValue.IsPreSelected,
                            CustomerEntersQty = attributeValue.CustomerEntersQty,
                            Quantity = attributeValue.Quantity
                        };
                        attributeModel.Values.Add(valueModel);

                        //display price if allowed
                        if (await PermissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
                        {
                            var customer = updatecartitem?.CustomerId is null ? await WorkContext.GetCurrentCustomerAsync() : await CustomerService.GetCustomerByIdAsync(updatecartitem.CustomerId);

                            var attributeValuePriceAdjustment = await PriceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer);
                            var (priceAdjustmentBase, _) = await TaxService.GetProductPriceAsync(product, attributeValuePriceAdjustment);
                            var priceAdjustment = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase, await WorkContext.GetWorkingCurrencyAsync());

                            if (attributeValue.PriceAdjustmentUsePercentage)
                            {
                                var priceAdjustmentStr = attributeValue.PriceAdjustment.ToString("G29");
                                if (attributeValue.PriceAdjustment > decimal.Zero)
                                    valueModel.PriceAdjustment = "+";
                                valueModel.PriceAdjustment += priceAdjustmentStr + "%";
                            }
                            else
                            {
                                if (priceAdjustmentBase > decimal.Zero)
                                    valueModel.PriceAdjustment = "+" + await PriceFormatter.FormatPriceAsync(priceAdjustment, false, false);
                                else if (priceAdjustmentBase < decimal.Zero)
                                    valueModel.PriceAdjustment = "-" + await PriceFormatter.FormatPriceAsync(-priceAdjustment, false, false);
                            }

                            valueModel.PriceAdjustmentValue = priceAdjustment;
                        }

                        //"image square" picture (with with "image squares" attribute type only)
                        if (attributeValue.ImageSquaresPictureId > 0)
                        {
                            var productAttributeImageSquarePictureCacheKey = StaticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductAttributeImageSquarePictureModelKey
                                , attributeValue.ImageSquaresPictureId,
                                    WebHelper.IsCurrentConnectionSecured(),
                                    await StoreContext.GetCurrentStoreAsync());
                            valueModel.ImageSquaresPictureModel = await StaticCacheManager.GetAsync(productAttributeImageSquarePictureCacheKey, async () =>
                            {
                                var imageSquaresPicture = await PictureService.GetPictureByIdAsync(attributeValue.ImageSquaresPictureId);
                                string fullSizeImageUrl, imageUrl;
                                (imageUrl, imageSquaresPicture) = await PictureService.GetPictureUrlAsync(imageSquaresPicture, MediaSettings.ImageSquarePictureSize);
                                (fullSizeImageUrl, imageSquaresPicture) = await PictureService.GetPictureUrlAsync(imageSquaresPicture);

                                if (imageSquaresPicture != null)
                                {
                                    return new PictureModel
                                    {
                                        FullSizeImageUrl = fullSizeImageUrl,
                                        ImageUrl = imageUrl
                                    };
                                }

                                return new PictureModel();
                            });
                        }

                        //picture of a product attribute value
                        valueModel.PictureId = attributeValue.PictureId;
                    }
                }

                //set already selected attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = await ProductAttributeParser.ParseProductAttributeValuesAsync(updatecartitem.AttributesXml);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                            {
                                                item.IsPreSelected = true;

                                                //set customer entered quantity
                                                if (attributeValue.CustomerEntersQty)
                                                    item.Quantity = attributeValue.Quantity;
                                            }
                                }
                            }

                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                            {
                                //values are already pre-set

                                //set customer entered quantity
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    foreach (var attributeValue in (await ProductAttributeParser.ParseProductAttributeValuesAsync(updatecartitem.AttributesXml))
                                        .Where(value => value.CustomerEntersQty))
                                    {
                                        var item = attributeModel.Values.FirstOrDefault(value => value.Id == attributeValue.Id);
                                        if (item != null)
                                            item.Quantity = attributeValue.Quantity;
                                    }
                                }
                            }

                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var enteredText = ProductAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                    if (enteredText.Any())
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }

                            break;
                        case AttributeControlType.Datepicker:
                            {
                                //keep in mind my that the code below works only in the current culture
                                var selectedDateStr = ProductAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                if (selectedDateStr.Any())
                                {
                                    if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture, DateTimeStyles.None, out var selectedDate))
                                    {
                                        //successfully parsed
                                        attributeModel.SelectedDay = selectedDate.Day;
                                        attributeModel.SelectedMonth = selectedDate.Month;
                                        attributeModel.SelectedYear = selectedDate.Year;
                                    }
                                }
                            }

                            break;
                        case AttributeControlType.FileUpload:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var downloadGuidStr = ProductAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id).FirstOrDefault();
                                    _ = Guid.TryParse(downloadGuidStr, out var downloadGuid);
                                    var download = await DownloadService.GetDownloadByGuidAsync(downloadGuid);
                                    if (download != null)
                                        attributeModel.DefaultValue = download.DownloadGuid.ToString();
                                }
                            }

                            break;
                        default:
                            break;
                    }
                }

                model.Add(attributeModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the product tier price models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of tier price model
        /// </returns>
        protected virtual async Task<IList<ProductDetailsModel.TierPriceModel>> PrepareProductTierPriceModelsAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var model = await (await ProductService.GetTierPricesAsync(product, customer, store.Id))
                .SelectAwait(async tierPrice =>
                {
                    var priceBase = (await TaxService.GetProductPriceAsync(product, (await PriceCalculationService.GetFinalPriceAsync(product,
                        customer, decimal.Zero, CatalogSettings.DisplayTierPricesWithDiscounts,
                        tierPrice.Quantity)).priceWithoutDiscounts)).price;

                    var price = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(priceBase, await WorkContext.GetWorkingCurrencyAsync());

                    return new ProductDetailsModel.TierPriceModel
                    {
                        Quantity = tierPrice.Quantity,
                        Price = await PriceFormatter.FormatPriceAsync(price, false, false),
                        PriceValue = price
                    };
                }).ToListAsync();

            return model;
        }

        /// <summary>
        /// Prepare the product manufacturer models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of manufacturer brief info model
        /// </returns>
        protected virtual async Task<IList<ManufacturerBriefInfoModel>> PrepareProductManufacturerModelsAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = await (await ManufacturerService.GetProductManufacturersByProductIdAsync(product.Id))
                .SelectAwait(async pm =>
                {
                    var manufacturer = await ManufacturerService.GetManufacturerByIdAsync(pm.ManufacturerId);
                    var modelMan = new ManufacturerBriefInfoModel
                    {
                        Id = manufacturer.Id,
                        Name = await LocalizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                        SeName = await UrlRecordService.GetSeNameAsync(manufacturer)
                    };

                    return modelMan;
                }).ToListAsync();

            return model;
        }

        /// <summary>
        /// Prepare the product details picture model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="isAssociatedProduct">Whether the product is associated</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture model for the default picture; All picture models
        /// </returns>
        protected virtual async Task<(PictureModel pictureModel, IList<PictureModel> allPictureModels)> PrepareProductDetailsPictureModelAsync(Product product, bool isAssociatedProduct)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //default picture size
            var defaultPictureSize = isAssociatedProduct ?
                MediaSettings.AssociatedProductPictureSize :
                MediaSettings.ProductDetailsPictureSize;

            //prepare picture models
            var productPicturesCacheKey = StaticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductDetailsPicturesModelKey
                , product, defaultPictureSize, isAssociatedProduct,
                await WorkContext.GetWorkingLanguageAsync(), WebHelper.IsCurrentConnectionSecured(), await StoreContext.GetCurrentStoreAsync());
            var cachedPictures = await StaticCacheManager.GetAsync(productPicturesCacheKey, async () =>
            {
                var productName = await LocalizationService.GetLocalizedAsync(product, x => x.Name);

                var pictures = await PictureService.GetPicturesByProductIdAsync(product.Id);
                var defaultPicture = pictures.FirstOrDefault();

                string fullSizeImageUrl, imageUrl, thumbImageUrl;
                (imageUrl, defaultPicture) = await PictureService.GetPictureUrlAsync(defaultPicture, defaultPictureSize, !isAssociatedProduct);
                (fullSizeImageUrl, defaultPicture) = await PictureService.GetPictureUrlAsync(defaultPicture, 0, !isAssociatedProduct);

                var defaultPictureModel = new PictureModel
                {
                    ImageUrl = imageUrl,
                    FullSizeImageUrl = fullSizeImageUrl
                };
                //"title" attribute
                defaultPictureModel.Title = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.TitleAttribute)) ?
                    defaultPicture.TitleAttribute :
                    string.Format(await LocalizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName);
                //"alt" attribute
                defaultPictureModel.AlternateText = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.AltAttribute)) ?
                    defaultPicture.AltAttribute :
                    string.Format(await LocalizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName);

                //all pictures
                var pictureModels = new List<PictureModel>();
                for (var i = 0; i < pictures.Count; i++)
                {
                    var picture = pictures[i];

                    (imageUrl, picture) = await PictureService.GetPictureUrlAsync(picture, defaultPictureSize, !isAssociatedProduct);
                    (fullSizeImageUrl, picture) = await PictureService.GetPictureUrlAsync(picture);
                    (thumbImageUrl, picture) = await PictureService.GetPictureUrlAsync(picture, MediaSettings.ProductThumbPictureSizeOnProductDetailsPage);

                    var pictureModel = new PictureModel
                    {
                        ImageUrl = imageUrl,
                        ThumbImageUrl = thumbImageUrl,
                        FullSizeImageUrl = fullSizeImageUrl,
                        Title = string.Format(await LocalizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName),
                        AlternateText = string.Format(await LocalizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName),
                    };
                    //"title" attribute
                    pictureModel.Title = !string.IsNullOrEmpty(picture.TitleAttribute) ?
                        picture.TitleAttribute :
                        string.Format(await LocalizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName);
                    //"alt" attribute
                    pictureModel.AlternateText = !string.IsNullOrEmpty(picture.AltAttribute) ?
                        picture.AltAttribute :
                        string.Format(await LocalizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName);

                    pictureModels.Add(pictureModel);
                }

                return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
            });

            var allPictureModels = cachedPictures.PictureModels;
            return (cachedPictures.DefaultPictureModel, allPictureModels);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the product template view path
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view path
        /// </returns>
        public virtual async Task<string> PrepareProductTemplateViewPathAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var template = await ProductTemplateService.GetProductTemplateByIdAsync(product.ProductTemplateId) ??
                           (await ProductTemplateService.GetAllProductTemplatesAsync()).FirstOrDefault();

            if (template == null)
                throw new Exception("No default template could be loaded");

            return template.ViewPath;
        }

        /// <summary>
        /// Prepare the product overview models
        /// </summary>
        /// <param name="products">Collection of products</param>
        /// <param name="preparePriceModel">Whether to prepare the price model</param>
        /// <param name="preparePictureModel">Whether to prepare the picture model</param>
        /// <param name="productThumbPictureSize">Product thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <param name="prepareSpecificationAttributes">Whether to prepare the specification attribute models</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of product overview model
        /// </returns>
        public virtual async Task<IEnumerable<ProductOverviewModel>> PrepareProductOverviewModelsAsync(IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));

            var models = new List<ProductOverviewModel>();
            foreach (var product in products)
            {
                var model = new ProductOverviewModel
                {
                    Id = product.Id,
                    Name = await LocalizationService.GetLocalizedAsync(product, x => x.Name),
                    ShortDescription = await LocalizationService.GetLocalizedAsync(product, x => x.ShortDescription),
                    FullDescription = await LocalizationService.GetLocalizedAsync(product, x => x.FullDescription),
                    SeName = await UrlRecordService.GetSeNameAsync(product),
                    Sku = product.Sku,
                    ProductType = product.ProductType,
                    MarkAsNew = product.MarkAsNew &&
                        (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                        (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow)
                };

                //price
                if (preparePriceModel)
                {
                    model.ProductPrice = await PrepareProductOverviewPriceModelAsync(product, forceRedirectionAfterAddingToCart);
                }

                //picture
                if (preparePictureModel)
                {
                    model.DefaultPictureModel = await PrepareProductOverviewPictureModelAsync(product, productThumbPictureSize);
                }

                //specs
                if (prepareSpecificationAttributes)
                {
                    model.ProductSpecificationModel = await PrepareProductSpecificationModelAsync(product);
                }

                //reviews
                model.ReviewOverviewModel = await PrepareProductReviewOverviewModelAsync(product);

                models.Add(model);
            }

            return models;
        }

        /// <summary>
        /// Prepare the product combination models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product combination models
        /// </returns>
        public virtual async Task<IList<ProductCombinationModel>> PrepareProductCombinationModelsAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var result = new List<ProductCombinationModel>();

            var combinations = await ProductAttributeService
                .GetAllProductAttributeCombinationsAsync(product.Id);
            if (combinations?.Any() == true)
            {
                foreach (var combination in combinations)
                {
                    var combinationModel = new ProductCombinationModel
                    {
                        InStock = combination.StockQuantity > 0 || combination.AllowOutOfStockOrders
                    };

                    var mappings = await ProductAttributeParser
                        .ParseProductAttributeMappingsAsync(combination.AttributesXml);
                    if (mappings == null || mappings.Count == 0)
                        continue;

                    foreach (var mapping in mappings)
                    {
                        var attributeModel = new ProductAttributeModel
                        {
                            Id = mapping.Id
                        };

                        var values = await ProductAttributeParser
                            .ParseProductAttributeValuesAsync(combination.AttributesXml, mapping.Id);
                        if (values == null || values.Count == 0)
                            continue;

                        foreach (var value in values)
                            attributeModel.ValueIds.Add(value.Id);

                        combinationModel.Attributes.Add(attributeModel);
                    }

                    result.Add(combinationModel);
                }
            }

            return result;
        }

        /// <summary>
        /// Prepare the product details model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <param name="isAssociatedProduct">Whether the product is associated</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product details model
        /// </returns>
        public virtual async Task<ProductDetailsModel> PrepareProductDetailsModelAsync(Product product,
            ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //standard properties
            var model = new ProductDetailsModel
            {
                Id = product.Id,
                Name = await LocalizationService.GetLocalizedAsync(product, x => x.Name),
                ShortDescription = await LocalizationService.GetLocalizedAsync(product, x => x.ShortDescription),
                FullDescription = await LocalizationService.GetLocalizedAsync(product, x => x.FullDescription),
                MetaKeywords = await LocalizationService.GetLocalizedAsync(product, x => x.MetaKeywords),
                MetaDescription = await LocalizationService.GetLocalizedAsync(product, x => x.MetaDescription),
                MetaTitle = await LocalizationService.GetLocalizedAsync(product, x => x.MetaTitle),
                SeName = await UrlRecordService.GetSeNameAsync(product),
                ProductType = product.ProductType,
                ShowSku = CatalogSettings.ShowSkuOnProductDetailsPage,
                Sku = product.Sku,
                ShowManufacturerPartNumber = CatalogSettings.ShowManufacturerPartNumber,
                FreeShippingNotificationEnabled = CatalogSettings.ShowFreeShippingNotification,
                ManufacturerPartNumber = product.ManufacturerPartNumber,
                ShowGtin = CatalogSettings.ShowGtin,
                Gtin = product.Gtin,
                ManageInventoryMethod = product.ManageInventoryMethod,
                StockAvailability = await ProductService.FormatStockMessageAsync(product, string.Empty),
                HasSampleDownload = product.IsDownload && product.HasSampleDownload,
                DisplayDiscontinuedMessage = !product.Published && CatalogSettings.DisplayDiscontinuedMessageForUnpublishedProducts,
                AvailableEndDate = product.AvailableEndDateTimeUtc,
                VisibleIndividually = product.VisibleIndividually,
                AllowAddingOnlyExistingAttributeCombinations = product.AllowAddingOnlyExistingAttributeCombinations
            };

            //automatically generate product description?
            if (SeoSettings.GenerateProductMetaDescription && string.IsNullOrEmpty(model.MetaDescription))
            {
                //based on short description
                model.MetaDescription = model.ShortDescription;
            }

            //shipping info
            model.IsShipEnabled = product.IsShipEnabled;
            if (product.IsShipEnabled)
            {
                model.IsFreeShipping = product.IsFreeShipping;
                //delivery date
                var deliveryDate = await DateRangeService.GetDeliveryDateByIdAsync(product.DeliveryDateId);
                if (deliveryDate != null)
                {
                    model.DeliveryDate = await LocalizationService.GetLocalizedAsync(deliveryDate, dd => dd.Name);
                }
            }

            var store = await StoreContext.GetCurrentStoreAsync();
            //email a friend
            model.EmailAFriendEnabled = CatalogSettings.EmailAFriendEnabled;
            //compare products
            model.CompareProductsEnabled = CatalogSettings.CompareProductsEnabled;
            //store name
            model.CurrentStoreName = await LocalizationService.GetLocalizedAsync(store, x => x.Name);

            //vendor details
            if (VendorSettings.ShowVendorOnProductDetailsPage)
            {
                var vendor = await VendorService.GetVendorByIdAsync(product.VendorId);
                if (vendor != null && !vendor.Deleted && vendor.Active)
                {
                    model.ShowVendor = true;

                    model.VendorModel = new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = await LocalizationService.GetLocalizedAsync(vendor, x => x.Name),
                        SeName = await UrlRecordService.GetSeNameAsync(vendor),
                    };
                }
            }

            //page sharing
            if (CatalogSettings.ShowShareButton && !string.IsNullOrEmpty(CatalogSettings.PageShareCode))
            {
                var shareCode = CatalogSettings.PageShareCode;
                if (WebHelper.IsCurrentConnectionSecured())
                {
                    //need to change the add this link to be https linked when the page is, so that the page doesn't ask about mixed mode when viewed in https...
                    shareCode = shareCode.Replace("http://", "https://");
                }

                model.PageShareCode = shareCode;
            }

            switch (product.ManageInventoryMethod)
            {
                case ManageInventoryMethod.DontManageStock:
                    model.InStock = true;
                    break;

                case ManageInventoryMethod.ManageStock:
                    model.InStock = product.BackorderMode != BackorderMode.NoBackorders
                        || await ProductService.GetTotalStockQuantityAsync(product) > 0;
                    model.DisplayBackInStockSubscription = !model.InStock && product.AllowBackInStockSubscriptions;
                    break;

                case ManageInventoryMethod.ManageStockByAttributes:
                    model.InStock = (await ProductAttributeService
                        .GetAllProductAttributeCombinationsAsync(product.Id))
                        ?.Any(c => c.StockQuantity > 0 || c.AllowOutOfStockOrders)
                        ?? false;
                    break;
            }

            //breadcrumb
            //do not prepare this model for the associated products. anyway it's not used
            if (CatalogSettings.CategoryBreadcrumbEnabled && !isAssociatedProduct)
            {
                model.Breadcrumb = await PrepareProductBreadcrumbModelAsync(product);
            }

            //product tags
            //do not prepare this model for the associated products. anyway it's not used
            if (!isAssociatedProduct)
            {
                model.ProductTags = await PrepareProductTagModelsAsync(product);
            }

            //pictures
            model.DefaultPictureZoomEnabled = MediaSettings.DefaultPictureZoomEnabled;
            IList<PictureModel> allPictureModels;
            (model.DefaultPictureModel, allPictureModels) = await PrepareProductDetailsPictureModelAsync(product, isAssociatedProduct);
            model.PictureModels = allPictureModels;

            //price
            model.ProductPrice = await PrepareProductPriceModelAsync(product);

            //'Add to cart' model
            model.AddToCart = await PrepareProductAddToCartModelAsync(product, updatecartitem);
            var customer = await WorkContext.GetCurrentCustomerAsync();
            //gift card
            if (product.IsGiftCard)
            {
                model.GiftCard.IsGiftCard = true;
                model.GiftCard.GiftCardType = product.GiftCardType;

                if (updatecartitem == null)
                {
                    model.GiftCard.SenderName = await CustomerService.GetCustomerFullNameAsync(customer);
                    model.GiftCard.SenderEmail = customer.Email;
                }
                else
                {
                    ProductAttributeParser.GetGiftCardAttribute(updatecartitem.AttributesXml,
                        out var giftCardRecipientName, out var giftCardRecipientEmail,
                        out var giftCardSenderName, out var giftCardSenderEmail, out var giftCardMessage);

                    model.GiftCard.RecipientName = giftCardRecipientName;
                    model.GiftCard.RecipientEmail = giftCardRecipientEmail;
                    model.GiftCard.SenderName = giftCardSenderName;
                    model.GiftCard.SenderEmail = giftCardSenderEmail;
                    model.GiftCard.Message = giftCardMessage;
                }
            }

            //product attributes
            model.ProductAttributes = await PrepareProductAttributeModelsAsync(product, updatecartitem);

            //product specifications
            //do not prepare this model for the associated products. anyway it's not used
            if (!isAssociatedProduct)
            {
                model.ProductSpecificationModel = await PrepareProductSpecificationModelAsync(product);
            }

            //product review overview
            model.ProductReviewOverview = await PrepareProductReviewOverviewModelAsync(product);

            //tier prices
            if (product.HasTierPrices && await PermissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                model.TierPrices = await PrepareProductTierPriceModelsAsync(product);
            }

            //manufacturers
            model.ProductManufacturers = await PrepareProductManufacturerModelsAsync(product);

            //rental products
            if (product.IsRental)
            {
                model.IsRental = true;
                //set already entered dates attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    model.RentalStartDate = updatecartitem.RentalStartDateUtc;
                    model.RentalEndDate = updatecartitem.RentalEndDateUtc;
                }
            }

            //estimate shipping
            if (ShippingSettings.EstimateShippingProductPageEnabled && !model.IsFreeShipping)
            {
                var wrappedProduct = new ShoppingCartItem
                {
                    StoreId = store.Id,
                    ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart,
                    CustomerId = customer.Id,
                    ProductId = product.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };

                var estimateShippingModel = await ShoppingCartModelFactory.PrepareEstimateShippingModelAsync(new[] { wrappedProduct });

                model.ProductEstimateShipping.ProductId = product.Id;
                model.ProductEstimateShipping.RequestDelay = estimateShippingModel.RequestDelay;
                model.ProductEstimateShipping.Enabled = estimateShippingModel.Enabled;
                model.ProductEstimateShipping.CountryId = estimateShippingModel.CountryId;
                model.ProductEstimateShipping.StateProvinceId = estimateShippingModel.StateProvinceId;
                model.ProductEstimateShipping.ZipPostalCode = estimateShippingModel.ZipPostalCode;
                model.ProductEstimateShipping.UseCity = estimateShippingModel.UseCity;
                model.ProductEstimateShipping.City = estimateShippingModel.City;
                model.ProductEstimateShipping.AvailableCountries = estimateShippingModel.AvailableCountries;
                model.ProductEstimateShipping.AvailableStates = estimateShippingModel.AvailableStates;
            }

            //associated products
            if (product.ProductType == ProductType.GroupedProduct)
            {
                //ensure no circular references
                if (!isAssociatedProduct)
                {
                    var associatedProducts = await ProductService.GetAssociatedProductsAsync(product.Id, store.Id);
                    foreach (var associatedProduct in associatedProducts)
                        model.AssociatedProducts.Add(await PrepareProductDetailsModelAsync(associatedProduct, null, true));
                }
                model.InStock = model.AssociatedProducts.Any(associatedProduct => associatedProduct.InStock);
            }

            return model;
        }

        /// <summary>
        /// Prepare the product reviews model
        /// </summary>
        /// <param name="model">Product reviews model</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product reviews model
        /// </returns>
        public virtual async Task<ProductReviewsModel> PrepareProductReviewsModelAsync(ProductReviewsModel model, Product product)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            model.ProductId = product.Id;
            model.ProductName = await LocalizationService.GetLocalizedAsync(product, x => x.Name);
            model.ProductSeName = await UrlRecordService.GetSeNameAsync(product);

            var currentStore = await StoreContext.GetCurrentStoreAsync();

            var productReviews = await ProductService.GetAllProductReviewsAsync(
                approved: true,
                productId: product.Id,
                storeId: CatalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0);

            //get all review types
            foreach (var reviewType in await ReviewTypeService.GetAllReviewTypesAsync())
            {
                model.ReviewTypeList.Add(new ReviewTypeModel
                {
                    Id = reviewType.Id,
                    Name = await LocalizationService.GetLocalizedAsync(reviewType, entity => entity.Name),
                    Description = await LocalizationService.GetLocalizedAsync(reviewType, entity => entity.Description),
                    VisibleToAllCustomers = reviewType.VisibleToAllCustomers,
                    DisplayOrder = reviewType.DisplayOrder,
                    IsRequired = reviewType.IsRequired,
                });
            }

            var currentCustomer = await WorkContext.GetCurrentCustomerAsync();

            //filling data from db
            foreach (var pr in productReviews)
            {
                var customer = await CustomerService.GetCustomerByIdAsync(pr.CustomerId);

                var productReviewModel = new ProductReviewModel
                {
                    Id = pr.Id,
                    CustomerId = pr.CustomerId,
                    CustomerName = await CustomerService.FormatUsernameAsync(customer),
                    AllowViewingProfiles = CustomerSettings.AllowViewingProfiles && customer != null && !await CustomerService.IsGuestAsync(customer),
                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    ReplyText = pr.ReplyText,
                    Rating = pr.Rating,
                    Helpfulness = new ProductReviewHelpfulnessModel
                    {
                        ProductReviewId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                    },
                    WrittenOnStr = (await DateTimeHelper.ConvertToUserTimeAsync(pr.CreatedOnUtc, DateTimeKind.Utc)).ToString("g"),
                };

                if (CustomerSettings.AllowCustomersToUploadAvatars)
                {
                    productReviewModel.CustomerAvatarUrl = await PictureService.GetPictureUrlAsync(
                        await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                        MediaSettings.AvatarPictureSize, CustomerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                }

                foreach (var q in await ReviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewIdAsync(pr.Id))
                {
                    var reviewType = await ReviewTypeService.GetReviewTypeByIdAsync(q.ReviewTypeId);

                    productReviewModel.AdditionalProductReviewList.Add(new ProductReviewReviewTypeMappingModel
                    {
                        ReviewTypeId = q.ReviewTypeId,
                        ProductReviewId = pr.Id,
                        Rating = q.Rating,
                        Name = await LocalizationService.GetLocalizedAsync(reviewType, x => x.Name),
                        VisibleToAllCustomers = reviewType.VisibleToAllCustomers || currentCustomer.Id == pr.CustomerId,
                    });
                }

                model.Items.Add(productReviewModel);
            }

            foreach (var rt in model.ReviewTypeList)
            {
                if (model.ReviewTypeList.Count <= model.AddAdditionalProductReviewList.Count)
                    continue;
                var reviewType = await ReviewTypeService.GetReviewTypeByIdAsync(rt.Id);
                var reviewTypeMappingModel = new AddProductReviewReviewTypeMappingModel
                {
                    ReviewTypeId = rt.Id,
                    Name = await LocalizationService.GetLocalizedAsync(reviewType, entity => entity.Name),
                    Description = await LocalizationService.GetLocalizedAsync(reviewType, entity => entity.Description),
                    DisplayOrder = rt.DisplayOrder,
                    IsRequired = rt.IsRequired,
                };

                model.AddAdditionalProductReviewList.Add(reviewTypeMappingModel);
            }

            //Average rating
            foreach (var rtm in model.ReviewTypeList)
            {
                var totalRating = 0;
                var totalCount = 0;
                foreach (var item in model.Items)
                {
                    foreach (var q in item.AdditionalProductReviewList.Where(w => w.ReviewTypeId == rtm.Id))
                    {
                        totalRating += q.Rating;
                        totalCount = ++totalCount;
                    }
                }

                rtm.AverageRating = (double)totalRating / (totalCount > 0 ? totalCount : 1);
            }

            model.AddProductReview.CanCurrentCustomerLeaveReview = CatalogSettings.AllowAnonymousUsersToReviewProduct || !await CustomerService.IsGuestAsync(currentCustomer);
            model.AddProductReview.DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnProductReviewPage;
            model.AddProductReview.CanAddNewReview = await ProductService.CanAddReviewAsync(product.Id, CatalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0);

            return model;
        }

        /// <summary>
        /// Prepare the customer product reviews model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer product reviews model
        /// </returns>
        public virtual async Task<CustomerProductReviewsModel> PrepareCustomerProductReviewsModelAsync(int? page)
        {
            var pageSize = CatalogSettings.ProductReviewsPageSizeOnAccountPage;
            var pageIndex = 0;

            if (page > 0)
            {
                pageIndex = page.Value - 1;
            }

            var store = await StoreContext.GetCurrentStoreAsync();
            var customer = await WorkContext.GetCurrentCustomerAsync();

            var list = await ProductService.GetAllProductReviewsAsync(
                customerId: customer.Id,
                approved: null,
                storeId: CatalogSettings.ShowProductReviewsPerStore ? store.Id : 0,
                pageIndex: pageIndex,
                pageSize: pageSize);

            var productReviews = new List<CustomerProductReviewModel>();

            foreach (var review in list)
            {
                var product = await ProductService.GetProductByIdAsync(review.ProductId);

                var productReviewModel = new CustomerProductReviewModel
                {
                    Title = review.Title,
                    ProductId = product.Id,
                    ProductName = await LocalizationService.GetLocalizedAsync(product, p => p.Name),
                    ProductSeName = await UrlRecordService.GetSeNameAsync(product),
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                    ReplyText = review.ReplyText,
                    WrittenOnStr = (await DateTimeHelper.ConvertToUserTimeAsync(review.CreatedOnUtc, DateTimeKind.Utc)).ToString("g")
                };

                if (CatalogSettings.ProductReviewsMustBeApproved)
                {
                    productReviewModel.ApprovalStatus = review.IsApproved
                        ? await LocalizationService.GetResourceAsync("Account.CustomerProductReviews.ApprovalStatus.Approved")
                        : await LocalizationService.GetResourceAsync("Account.CustomerProductReviews.ApprovalStatus.Pending");
                }

                foreach (var q in await ReviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewIdAsync(review.Id))
                {
                    var reviewType = await ReviewTypeService.GetReviewTypeByIdAsync(q.ReviewTypeId);

                    productReviewModel.AdditionalProductReviewList.Add(new ProductReviewReviewTypeMappingModel
                    {
                        ReviewTypeId = q.ReviewTypeId,
                        ProductReviewId = review.Id,
                        Rating = q.Rating,
                        Name = await LocalizationService.GetLocalizedAsync(reviewType, x => x.Name),
                    });
                }

                productReviews.Add(productReviewModel);
            }

            var pagerModel = new PagerModel(LocalizationService)
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerProductReviewsPaged",
                UseRouteLinks = true,
                RouteValues = new CustomerProductReviewsModel.CustomerProductReviewsRouteValues { pageNumber = pageIndex }
            };

            var model = new CustomerProductReviewsModel
            {
                ProductReviews = productReviews,
                PagerModel = pagerModel
            };

            return model;
        }

        /// <summary>
        /// Prepare the product email a friend model
        /// </summary>
        /// <param name="model">Product email a friend model</param>
        /// <param name="product">Product</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product email a friend model
        /// </returns>
        public virtual async Task<ProductEmailAFriendModel> PrepareProductEmailAFriendModelAsync(ProductEmailAFriendModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            model.ProductId = product.Id;
            model.ProductName = await LocalizationService.GetLocalizedAsync(product, x => x.Name);
            model.ProductSeName = await UrlRecordService.GetSeNameAsync(product);
            model.DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnEmailProductToFriendPage;
            if (!excludeProperties)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                model.YourEmailAddress = customer.Email;
            }

            return model;
        }

        /// <summary>
        /// Prepare the product specification model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product specification model
        /// </returns>
        public virtual async Task<ProductSpecificationModel> PrepareProductSpecificationModelAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new ProductSpecificationModel();

            // Add non-grouped attributes first
            model.Groups.Add(new ProductSpecificationAttributeGroupModel
            {
                Attributes = await PrepareProductSpecificationAttributeModelAsync(product, null)
            });

            // Add grouped attributes
            var groups = await SpecificationAttributeService.GetProductSpecificationAttributeGroupsAsync(product.Id);
            foreach (var group in groups)
            {
                model.Groups.Add(new ProductSpecificationAttributeGroupModel
                {
                    Id = group.Id,
                    Name = await LocalizationService.GetLocalizedAsync(group, x => x.Name),
                    Attributes = await PrepareProductSpecificationAttributeModelAsync(product, group)
                });
            }

            return model;
        }

        #endregion
    }
}
