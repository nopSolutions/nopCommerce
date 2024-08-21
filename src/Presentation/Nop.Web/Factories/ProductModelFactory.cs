using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the product model factory
/// </summary>
public partial class ProductModelFactory : IProductModelFactory
{
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly ICategoryService _categoryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateRangeService _dateRangeService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IDownloadService _downloadService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IJsonLdModelFactory _jsonLdModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IProductTagService _productTagService;
    protected readonly IProductTemplateService _productTemplateService;
    protected readonly IReviewTypeService _reviewTypeService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly ISpecificationAttributeService _specificationAttributeService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly ITaxService _taxService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly IVideoService _videoService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly OrderSettings _orderSettings;
    protected readonly SeoSettings _seoSettings;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly VendorSettings _vendorSettings;
    private static readonly char[] _separator = [','];

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
        IJsonLdModelFactory jsonLdModelFactory,
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
        IShoppingCartService shoppingCartService,
        ISpecificationAttributeService specificationAttributeService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreService storeService,
        IShoppingCartModelFactory shoppingCartModelFactory,
        ITaxService taxService,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        IVideoService videoService,
        IWebHelper webHelper,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        OrderSettings orderSettings,
        SeoSettings seoSettings,
        ShippingSettings shippingSettings,
        VendorSettings vendorSettings)
    {
        _captchaSettings = captchaSettings;
        _catalogSettings = catalogSettings;
        _customerSettings = customerSettings;
        _categoryService = categoryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _dateRangeService = dateRangeService;
        _dateTimeHelper = dateTimeHelper;
        _downloadService = downloadService;
        _genericAttributeService = genericAttributeService;
        _jsonLdModelFactory = jsonLdModelFactory;
        _localizationService = localizationService;
        _manufacturerService = manufacturerService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _priceCalculationService = priceCalculationService;
        _priceFormatter = priceFormatter;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _productTagService = productTagService;
        _productTemplateService = productTemplateService;
        _reviewTypeService = reviewTypeService;
        _shoppingCartService = shoppingCartService;
        _specificationAttributeService = specificationAttributeService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeService = storeService;
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _taxService = taxService;
        _urlRecordService = urlRecordService;
        _vendorService = vendorService;
        _webHelper = webHelper;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _orderSettings = orderSettings;
        _seoSettings = seoSettings;
        _shippingSettings = shippingSettings;
        _vendorSettings = vendorSettings;
        _videoService = videoService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare the grouped product overview price model
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="priceModel">Price model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareGroupedProductPriceModelAsync(Product product, ProductPriceModel priceModel)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id,
            store.Id);

        if (!associatedProducts.Any())
            return;

        //we have at least one associated product
        //find a minimum possible price
        decimal? minPossiblePrice = null;
        Product minPriceProduct = null;
        var customer = await _workContext.GetCurrentCustomerAsync();
        foreach (var associatedProduct in associatedProducts)
        {
            var (_, tmpMinPossiblePrice, _, _) = await _priceCalculationService.GetFinalPriceAsync(associatedProduct, customer, store);

            //calculate price for the maximum quantity if we have tier prices, and choose minimal
            tmpMinPossiblePrice = Math.Min(tmpMinPossiblePrice,
                (await _priceCalculationService.GetFinalPriceAsync(associatedProduct, customer, store, quantity: int.MaxValue)).finalPrice);

            if (minPossiblePrice.HasValue && tmpMinPossiblePrice >= minPossiblePrice.Value)
                continue;

            minPriceProduct = associatedProduct;
            minPossiblePrice = tmpMinPossiblePrice;
        }

        if (minPriceProduct == null || minPriceProduct.CustomerEntersPrice)
            return;

        if (minPriceProduct.CallForPrice &&
            //also check whether the current user is impersonated
            (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
             _workContext.OriginalCustomerIfImpersonated == null))
        {
            priceModel.OldPrice = null;
            priceModel.OldPriceValue = null;
            priceModel.Price = await _localizationService.GetResourceAsync("Products.CallForPrice");
            priceModel.PriceValue = null;
        }
        else
        {
            //calculate prices
            var (finalPriceBase, _) = await _taxService.GetProductPriceAsync(minPriceProduct, minPossiblePrice.Value);
            var finalPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceBase, await _workContext.GetWorkingCurrencyAsync());

            priceModel.OldPrice = null;
            priceModel.OldPriceValue = null;
            priceModel.Price = string.Format(await _localizationService.GetResourceAsync("Products.PriceRangeFrom"), await _priceFormatter.FormatPriceAsync(finalPrice));
            priceModel.PriceValue = finalPrice;

            //PAngV default baseprice (used in Germany)
            priceModel.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(product, finalPriceBase);
            priceModel.BasePricePAngVValue = finalPriceBase;
        }
    }

    /// <summary>
    /// Gets the minimum possible product price
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="customer">Customer</param>
    /// <param name="store">Store</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the minimum possible product price
    /// </returns>
    protected async Task<(bool hasMultiplePrices, decimal minPossiblePriceWithoutDiscount, decimal minPossiblePriceWithDiscount)> GetFromPrice(Product product, Customer customer, Store store)
    {
        var (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = (decimal.Zero, decimal.Zero);
        var hasMultiplePrices = false;

        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
        var cacheKey = _staticCacheManager
            .PrepareKeyForDefaultCache(NopCatalogDefaults.ProductMultiplePriceCacheKey, product, customerRoleIds, store);
        if (!_catalogSettings.CacheProductPrices || product.IsRental)
            cacheKey.CacheTime = 0;

        var cachedPrice = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var prices = new List<(decimal PriceWithoutDiscount, decimal PriceWithDiscount)>();

            // price when there are no required attributes
            var attributesMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            if (!attributesMappings.Any(am => !am.IsNonCombinable() && am.IsRequired))
            {
                var (priceWithoutDiscount, priceWithDiscount, _, _) = await _priceCalculationService
                    .GetFinalPriceAsync(product, customer, store);
                prices.Add((priceWithoutDiscount, priceWithDiscount));
            }

            var allAttributesXml = await _productAttributeParser.GenerateAllCombinationsAsync(product, true);
            foreach (var attributesXml in allAttributesXml)
            {
                var warnings = new List<string>();
                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(customer,
                    ShoppingCartType.ShoppingCart, product, 1, attributesXml, true, true, true));
                if (warnings.Any())
                    continue;

                //get price with additional charge
                var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
                if (combination?.OverriddenPrice.HasValue ?? false)
                {
                    var (priceWithoutDiscount, priceWithDiscount, _, _) = await _priceCalculationService
                        .GetFinalPriceAsync(product, customer, store, combination.OverriddenPrice.Value, decimal.Zero, true, 1, null, null);
                    prices.Add((priceWithoutDiscount, priceWithDiscount));
                }
                else
                {
                    var additionalCharge = decimal.Zero;
                    var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
                    foreach (var attributeValue in attributeValues)
                        additionalCharge += await _priceCalculationService.
                            GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer, store);
                    if (additionalCharge != decimal.Zero)
                    {
                        var (priceWithoutDiscount, priceWithDiscount, _, _) = await _priceCalculationService
                            .GetFinalPriceAsync(product, customer, store, additionalCharge);
                        prices.Add((priceWithoutDiscount, priceWithDiscount));
                    }
                }
            }

            if (prices.Distinct().Count() > 1)
            {
                (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = prices.OrderBy(p => p.PriceWithDiscount).First();
                return new
                {
                    PriceWithoutDiscount = minPossiblePriceWithoutDiscount,
                    PriceWithDiscount = minPossiblePriceWithDiscount
                };
            }

            // show default price when required attributes available but no values added
            (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store);

            //don't cache (return null) if there are no multiple prices
            return null;
        });

        if (cachedPrice is not null)
        {
            hasMultiplePrices = true;
            (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = (cachedPrice.PriceWithoutDiscount, cachedPrice.PriceWithDiscount);
        }

        return (hasMultiplePrices, minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount);
    }

    /// <summary>
    /// Prepare the product price model
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="addPriceRangeFrom">Indicates whether to add a price range information</param>
    /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product price model
    /// </returns>
    protected virtual async Task<ProductPriceModel> PrepareProductPriceModelAsync(Product product, bool addPriceRangeFrom = false, bool forceRedirectionAfterAddingToCart = false)
    {
        ArgumentNullException.ThrowIfNull(product);

        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
        
        var model = new ProductPriceModel
        {
            ProductId = product.Id,
            //add to cart button (ignore "DisableBuyButton" property for grouped products)
            DisableBuyButton = (product.ProductType != ProductType.GroupedProduct && product.DisableBuyButton) ||
                !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART) ||
                !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES),
            //add to wishlist button (ignore "DisableWishlistButton" property for grouped products)
            DisableWishlistButton = (product.ProductType != ProductType.GroupedProduct && product.DisableWishlistButton) ||
                !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST) ||
                !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES),
            //compare products
            DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled,
            //currency code
            CurrencyCode = currentCurrency.CurrencyCode,
            ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
        };

        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
        {
            model.OldPrice = null;
            model.OldPriceValue = null;
            model.Price = null;
            model.PriceValue = null;
            model.HidePrices = true;

            return model;
        }

        if (addPriceRangeFrom && product.ProductType == ProductType.GroupedProduct)
        {
            await PrepareGroupedProductPriceModelAsync(product, model);

            return model;
        }

        //pre-order
        if (product.AvailableForPreOrder)
        {
            model.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                product.PreOrderAvailabilityStartDateTimeUtc.Value >=
                DateTime.UtcNow;

            model.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
        }

        if (product.CustomerEntersPrice)
        {
            model.CustomerEntersPrice = true;

            return model;
        }

        if (product.CallForPrice &&
            //also check whether the current user is impersonated
            (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
                _workContext.OriginalCustomerIfImpersonated == null))
        {
            model.CallForPrice = true;

            //call for price
            model.OldPrice = null;
            model.OldPriceValue = null;
            model.Price = await _localizationService.GetResourceAsync("Products.CallForPrice");
            model.PriceValue = null;

            return model;
        }

        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();

        //prices
        decimal minPossiblePriceWithoutDiscount;
        decimal minPossiblePriceWithDiscount;
        var hasMultiplePrices = false;

        if (addPriceRangeFrom && _catalogSettings.DisplayFromPrices)
            (hasMultiplePrices, minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = await GetFromPrice(product, customer, store);
        else
            (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store);

        var priceModifiersExist = false;

        if (addPriceRangeFrom)
        {
            //calculate price for the maximum quantity if we have tier prices, and choose minimal
            var (minPriceWithoutDiscount, minPriceWithDiscount, _, _)
                = await _priceCalculationService.GetFinalPriceAsync(product, customer, store, quantity: int.MaxValue);

            priceModifiersExist = minPriceWithoutDiscount < minPossiblePriceWithDiscount;

            if (priceModifiersExist)
            {
                minPossiblePriceWithoutDiscount = minPriceWithoutDiscount;
                minPossiblePriceWithDiscount = minPriceWithDiscount;
            }
        }

        var (oldPriceBase, _) = await _taxService.GetProductPriceAsync(product, product.OldPrice);
        var (finalPriceWithoutDiscountBase, _) = await _taxService.GetProductPriceAsync(product, minPossiblePriceWithoutDiscount);
        var (finalPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, minPossiblePriceWithDiscount);
        var oldPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(oldPriceBase, currentCurrency);
        var finalPriceWithoutDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithoutDiscountBase, currentCurrency);
        var finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, currentCurrency);

        var strikeThroughPrice = decimal.Zero;

        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
            strikeThroughPrice = oldPrice;

        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
            strikeThroughPrice = finalPriceWithoutDiscount;

        if (strikeThroughPrice > decimal.Zero)
        {
            model.OldPrice = await _priceFormatter.FormatPriceAsync(strikeThroughPrice);
            model.OldPriceValue = strikeThroughPrice;
        }
        else
        {
            model.OldPrice = null;
            model.OldPriceValue = null;
        }

        model.Price = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);

        if (addPriceRangeFrom)
        {
            if (hasMultiplePrices)
                model.Price = string.Format(await _localizationService.GetResourceAsync("Products.PriceRangeFrom"), model.Price);
            else if (priceModifiersExist)
            {
                //do we have tier prices configured?
                var tierPrices = await _productService.GetTierPricesAsync(product, customer, store);

                //when there is just one tier price (with  qty 1), there are no actual savings in the list.
                var hasTierPrices = tierPrices.Any() && !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);

                if (hasTierPrices)
                    model.Price = string.Format(await _localizationService.GetResourceAsync("Products.PriceRangeFrom"), model.Price);
            }
        }

        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
        {
            model.PriceWithDiscount = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
            model.PriceWithDiscountValue = finalPriceWithDiscount;
        }

        model.PriceValue = finalPriceWithDiscount;

        //property for German market
        //we display tax/shipping info only with "shipping enabled" for this product
        //we also ensure this it's not free shipping
        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductBoxes
            && product.IsShipEnabled
            && !product.IsFreeShipping;

        if (product.IsRental)
        {
            model.IsRental = true;
            model.OldPrice = await _priceFormatter.FormatRentalProductPeriodAsync(product, model.OldPrice);
            model.Price = await _priceFormatter.FormatRentalProductPeriodAsync(product, model.Price);
            var priceStr = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
            model.RentalPrice = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceStr);
            model.RentalPriceValue = finalPriceWithDiscount;
        }

        //PAngV default base price (used in Germany)
        model.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(product, finalPriceWithDiscountBase);
        model.BasePricePAngVValue = finalPriceWithDiscountBase;

        return model;
    }

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
        ArgumentNullException.ThrowIfNull(product);

        var productSpecificationAttributes = await _specificationAttributeService.GetProductSpecificationAttributesAsync(
            product.Id, specificationAttributeGroupId: group?.Id, showOnProductPage: true);

        var result = new List<ProductSpecificationAttributeModel>();

        foreach (var psa in productSpecificationAttributes)
        {
            var option = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(psa.SpecificationAttributeOptionId);

            var model = result.FirstOrDefault(model => model.Id == option.SpecificationAttributeId);
            if (model == null)
            {
                var attribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(option.SpecificationAttributeId);
                model = new ProductSpecificationAttributeModel
                {
                    Id = attribute.Id,
                    Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name)
                };
                result.Add(model);
            }

            var value = new ProductSpecificationAttributeValueModel
            {
                AttributeTypeId = psa.AttributeTypeId,
                ColorSquaresRgb = option.ColorSquaresRgb,
                ValueRaw = psa.AttributeType switch
                {
                    SpecificationAttributeType.Option => WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(option, x => x.Name)),
                    SpecificationAttributeType.CustomText => WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(psa, x => x.CustomValue)),
                    SpecificationAttributeType.CustomHtmlText => await _localizationService.GetLocalizedAsync(psa, x => x.CustomValue),
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
        var currentStore = await _storeContext.GetCurrentStoreAsync();

        if (_catalogSettings.ShowProductReviewsPerStore)
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductReviewsModelKey, product, currentStore);

            productReview = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var productReviews = await _productService.GetAllProductReviewsAsync(productId: product.Id, approved: true, storeId: currentStore.Id);

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
            productReview.CanCurrentCustomerLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewProduct || !await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync());
            productReview.CanAddNewReview = await _productService.CanAddReviewAsync(product.Id, _catalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0);
        }

        return productReview;
    }
    
    /// <summary>
    /// Prepare the product overview picture model
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="productThumbPictureSize">Product thumb picture size (longest side); pass null to use the default value of media settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains picture models
    /// </returns>
    protected virtual async Task<IList<PictureModel>> PrepareProductOverviewPicturesModelAsync(Product product, int? productThumbPictureSize = null)
    {
        ArgumentNullException.ThrowIfNull(product);

        var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
        //If a size has been set in the view, we use it in priority
        var pictureSize = productThumbPictureSize ?? _mediaSettings.ProductThumbPictureSize;

        //prepare picture model
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductOverviewPicturesModelKey,
            product, pictureSize, true, _catalogSettings.DisplayAllPicturesOnCatalogPages, await _workContext.GetWorkingLanguageAsync(),
            _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());

        var cachedPictures = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            async Task<PictureModel> preparePictureModelAsync(Picture picture)
            {
                //we have to keep the url generation order "full size -> preview" because picture can be updated twice
                //this section of code requires detailed analysis in the future
                (var fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                (var imageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                return new PictureModel
                {
                    ImageUrl = imageUrl,
                    FullSizeImageUrl = fullSizeImageUrl,
                    //"title" attribute
                    Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                        ? picture.TitleAttribute
                        : string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat"),
                            productName),
                    //"alt" attribute
                    AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                        ? picture.AltAttribute
                        : string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat"),
                            productName)
                };
            }

            //all pictures
            var pictures = (await _pictureService
                    .GetPicturesByProductIdAsync(product.Id, _catalogSettings.DisplayAllPicturesOnCatalogPages ? 0 : 1))
                .DefaultIfEmpty(null);
            var pictureModels = await pictures
                .SelectAwait(async picture => await preparePictureModelAsync(picture))
                .ToListAsync();
            return pictureModels;
        });

        return cachedPictures;
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
        ArgumentNullException.ThrowIfNull(product);

        var breadcrumbModel = new ProductDetailsModel.ProductBreadcrumbModel
        {
            Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
            ProductId = product.Id,
            ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
            ProductSeName = await _urlRecordService.GetSeNameAsync(product)
        };
        var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);
        if (!productCategories.Any())
            return breadcrumbModel;

        var category = await _categoryService.GetCategoryByIdAsync(productCategories[0].CategoryId);
        if (category == null)
            return breadcrumbModel;

        foreach (var catBr in await _categoryService.GetCategoryBreadCrumbAsync(category))
        {
            breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
            {
                Id = catBr.Id,
                Name = await _localizationService.GetLocalizedAsync(catBr, x => x.Name),
                SeName = await _urlRecordService.GetSeNameAsync(catBr),
                IncludeInTopMenu = catBr.IncludeInTopMenu
            });
        }

        if (_seoSettings.MicrodataEnabled)
        {
            var jsonLdModel = await _jsonLdModelFactory.PrepareJsonLdProductBreadcrumbAsync(breadcrumbModel);
            breadcrumbModel.JsonLd = JsonConvert
                .SerializeObject(jsonLdModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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
        ArgumentNullException.ThrowIfNull(product);

        var store = await _storeContext.GetCurrentStoreAsync();
        var productsTags = await _productTagService.GetAllProductTagsByProductIdAsync(product.Id);

        var model = await productsTags
            //filter by store
            .WhereAwait(async x => await _productTagService.GetProductCountByProductTagIdAsync(x.Id, store.Id) > 0)
            .SelectAwait(async x => new ProductTagModel
            {
                Id = x.Id,
                Name = await _localizationService.GetLocalizedAsync(x, y => y.Name),
                SeName = await _urlRecordService.GetSeNameAsync(x),
                ProductCount = await _productTagService.GetProductCountByProductTagIdAsync(x.Id, store.Id)
            }).ToListAsync();

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
        ArgumentNullException.ThrowIfNull(product);

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
        var allowedQuantities = _productService.ParseAllowedQuantities(product);
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
            model.MinimumQuantityNotification = string.Format(await _localizationService.GetResourceAsync("Products.MinimumQuantityNotification"), product.OrderMinimumQuantity);
        }

        //'add to cart', 'add to wishlist' buttons
        model.DisableBuyButton = product.DisableBuyButton || !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART);
        model.DisableWishlistButton = product.DisableWishlistButton || !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST);
        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
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
                _catalogSettings.DisplayDatePreOrderAvailability)
            {
                model.PreOrderAvailabilityStartDateTimeUserTime =
                    (await _dateTimeHelper.ConvertToUserTimeAsync(model.PreOrderAvailabilityStartDateTimeUtc.Value)).ToString("D");
            }
        }
        //rental
        model.IsRental = product.IsRental;

        //customer entered price
        model.CustomerEntersPrice = product.CustomerEntersPrice;
        if (!model.CustomerEntersPrice)
            return model;

        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
        var minimumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MinimumCustomerEnteredPrice, currentCurrency);
        var maximumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MaximumCustomerEnteredPrice, currentCurrency);

        model.CustomerEnteredPrice = updatecartitem != null ? updatecartitem.CustomerEnteredPrice : minimumCustomerEnteredPrice;
        model.CustomerEnteredPriceRange = string.Format(await _localizationService.GetResourceAsync("Products.EnterProductPrice.Range"),
            await _priceFormatter.FormatPriceAsync(minimumCustomerEnteredPrice, false, false),
            await _priceFormatter.FormatPriceAsync(maximumCustomerEnteredPrice, false, false));

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
        ArgumentNullException.ThrowIfNull(product);

        var model = new List<ProductDetailsModel.ProductAttributeModel>();
        var store = updatecartitem != null ? await _storeService.GetStoreByIdAsync(updatecartitem.StoreId) : await _storeContext.GetCurrentStoreAsync();

        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
        foreach (var attribute in productAttributeMapping)
        {
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId);

            var attributeModel = new ProductDetailsModel.ProductAttributeModel
            {
                Id = attribute.Id,
                ProductId = product.Id,
                ProductAttributeId = attribute.ProductAttributeId,
                Name = await _localizationService.GetLocalizedAsync(productAttribute, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(productAttribute, x => x.Description),
                TextPrompt = await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType,
                DefaultValue = updatecartitem != null ? null : await _localizationService.GetLocalizedAsync(attribute, x => x.DefaultValue),
                HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml)
            };
            if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
            {
                attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                    .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }

            if (attribute.ShouldHaveValues())
            {
                //values
                var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                foreach (var attributeValue in attributeValues)
                {
                    var valueModel = new ProductDetailsModel.ProductAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                        ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                        IsPreSelected = attributeValue.IsPreSelected,
                        CustomerEntersQty = attributeValue.CustomerEntersQty,
                        Quantity = attributeValue.Quantity
                    };
                    attributeModel.Values.Add(valueModel);

                    //display price if allowed
                    if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
                    {
                        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
                        var customer = updatecartitem?.CustomerId is null ? currentCustomer : await _customerService.GetCustomerByIdAsync(updatecartitem.CustomerId);

                        var attributeValuePriceAdjustment = await _priceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer, store, quantity: updatecartitem?.Quantity ?? 1);
                        var (priceAdjustmentBase, _) = await _taxService.GetProductPriceAsync(product, attributeValuePriceAdjustment);
                        var priceAdjustment = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase, await _workContext.GetWorkingCurrencyAsync());

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
                                valueModel.PriceAdjustment = "+" + await _priceFormatter.FormatPriceAsync(priceAdjustment, false, false);
                            else if (priceAdjustmentBase < decimal.Zero)
                                valueModel.PriceAdjustment = "-" + await _priceFormatter.FormatPriceAsync(-priceAdjustment, false, false);
                        }

                        valueModel.PriceAdjustmentValue = priceAdjustment;
                    }

                    //"image square" picture (with with "image squares" attribute type only)
                    if (attributeValue.ImageSquaresPictureId > 0)
                    {
                        var productAttributeImageSquarePictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductAttributeImageSquarePictureModelKey
                            , attributeValue.ImageSquaresPictureId,
                            _webHelper.IsCurrentConnectionSecured(),
                            await _storeContext.GetCurrentStoreAsync());
                        valueModel.ImageSquaresPictureModel = await _staticCacheManager.GetAsync(productAttributeImageSquarePictureCacheKey, async () =>
                        {
                            var imageSquaresPicture = await _pictureService.GetPictureByIdAsync(attributeValue.ImageSquaresPictureId);

                            (var fullSizeImageUrl, imageSquaresPicture) = await _pictureService.GetPictureUrlAsync(imageSquaresPicture);
                            (var imageUrl, imageSquaresPicture) = await _pictureService.GetPictureUrlAsync(imageSquaresPicture, _mediaSettings.ImageSquarePictureSize);

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
                    valueModel.PictureId = (await _productAttributeService.GetProductAttributeValuePicturesAsync(attributeValue.Id)).FirstOrDefault()?.PictureId ?? 0;
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
                            var selectedValues = await _productAttributeParser.ParseProductAttributeValuesAsync(updatecartitem.AttributesXml);
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
                            foreach (var attributeValue in (await _productAttributeParser.ParseProductAttributeValuesAsync(updatecartitem.AttributesXml))
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
                            var enteredText = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                            if (enteredText.Any())
                                attributeModel.DefaultValue = enteredText[0];
                        }
                    }

                        break;
                    case AttributeControlType.Datepicker:
                    {
                        //keep in mind my that the code below works only in the current culture
                        var selectedDateStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
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
                            var downloadGuidStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id).FirstOrDefault();
                            _ = Guid.TryParse(downloadGuidStr, out var downloadGuid);
                            var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
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
        ArgumentNullException.ThrowIfNull(product);

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var model = await (await _productService.GetTierPricesAsync(product, customer, store))
            .SelectAwait(async tierPrice =>
            {
                var priceBase = (await _taxService.GetProductPriceAsync(product, (await _priceCalculationService.GetFinalPriceAsync(product,
                    customer, store, decimal.Zero, _catalogSettings.DisplayTierPricesWithDiscounts,
                    tierPrice.Quantity)).finalPrice)).price;

                var price = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceBase, await _workContext.GetWorkingCurrencyAsync());

                return new ProductDetailsModel.TierPriceModel
                {
                    Quantity = tierPrice.Quantity,
                    Price = await _priceFormatter.FormatPriceAsync(price, false, false),
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
        ArgumentNullException.ThrowIfNull(product);

        var model = await (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id))
            .SelectAwait(async pm =>
            {
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(pm.ManufacturerId);
                var modelMan = new ManufacturerBriefInfoModel
                {
                    Id = manufacturer.Id,
                    Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(manufacturer)
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
    protected virtual async Task<(PictureModel pictureModel, IList<PictureModel> allPictureModels, IList<VideoModel> allVideoModels)> PrepareProductDetailsPictureModelAsync(Product product, bool isAssociatedProduct)
    {
        ArgumentNullException.ThrowIfNull(product);

        //default picture size
        var defaultPictureSize = isAssociatedProduct ?
            _mediaSettings.AssociatedProductPictureSize :
            _mediaSettings.ProductDetailsPictureSize;

        //prepare picture models
        var productPicturesCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductDetailsPicturesModelKey
            , product, defaultPictureSize, isAssociatedProduct,
            await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());
        var cachedPictures = await _staticCacheManager.GetAsync(productPicturesCacheKey, async () =>
        {
            var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);

            var pictures = await _pictureService.GetPicturesByProductIdAsync(product.Id);
            var defaultPicture = pictures.FirstOrDefault();

            (var fullSizeImageUrl, defaultPicture) = await _pictureService.GetPictureUrlAsync(defaultPicture, 0, !isAssociatedProduct);
            (var imageUrl, defaultPicture) = await _pictureService.GetPictureUrlAsync(defaultPicture, defaultPictureSize, !isAssociatedProduct);

            var defaultPictureModel = new PictureModel
            {
                ImageUrl = imageUrl,
                FullSizeImageUrl = fullSizeImageUrl,
                //"title" attribute
                Title = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.TitleAttribute)) ?
                    defaultPicture.TitleAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName),
                //"alt" attribute
                AlternateText = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.AltAttribute)) ?
                    defaultPicture.AltAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName)
            };

            //all pictures
            var pictureModels = new List<PictureModel>();
            for (var i = 0; i < pictures.Count; i++)
            {
                var picture = pictures[i];

                (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                (imageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, defaultPictureSize, !isAssociatedProduct);
                (var thumbImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage);

                var pictureModel = new PictureModel
                {
                    Id = picture.Id,
                    ImageUrl = imageUrl,
                    ThumbImageUrl = thumbImageUrl,
                    FullSizeImageUrl = fullSizeImageUrl,
                    Title = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName),
                    AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName),
                };
                //"title" attribute
                pictureModel.Title = !string.IsNullOrEmpty(picture.TitleAttribute) ?
                    picture.TitleAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), productName);
                //"alt" attribute
                pictureModel.AlternateText = !string.IsNullOrEmpty(picture.AltAttribute) ?
                    picture.AltAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), productName);

                pictureModels.Add(pictureModel);
            }

            return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
        });

        var allPictureModels = cachedPictures.PictureModels;

        //all videos
        var allvideoModels = new List<VideoModel>();
        var videos = await _videoService.GetVideosByProductIdAsync(product.Id);
        foreach (var video in videos)
        {
            var videoModel = new VideoModel
            {
                VideoUrl = video.VideoUrl,
                Allow = _mediaSettings.VideoIframeAllow,
                Width = _mediaSettings.VideoIframeWidth,
                Height = _mediaSettings.VideoIframeHeight
            };

            allvideoModels.Add(videoModel);
        }
        return (cachedPictures.DefaultPictureModel, allPictureModels, allvideoModels);
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
        ArgumentNullException.ThrowIfNull(product);

        var template = (await _productTemplateService.GetProductTemplateByIdAsync(product.ProductTemplateId) ??
                        (await _productTemplateService.GetAllProductTemplatesAsync()).FirstOrDefault()) ?? throw new Exception("No default template could be loaded");

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
        ArgumentNullException.ThrowIfNull(products);

        var models = new List<ProductOverviewModel>();
        foreach (var product in products)
        {
            var model = new ProductOverviewModel
            {
                Id = product.Id,
                Name = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription),
                FullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription),
                SeName = await _urlRecordService.GetSeNameAsync(product),
                Sku = product.Sku,
                ProductType = product.ProductType,
                MarkAsNew = product.MarkAsNew &&
                            (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                            (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow)
            };

            //price
            if (preparePriceModel)
            {
                model.ProductPrice = await PrepareProductPriceModelAsync(product, true, forceRedirectionAfterAddingToCart);
            }

            //picture
            if (preparePictureModel)
            {
                model.PictureModels = await PrepareProductOverviewPicturesModelAsync(product, productThumbPictureSize);
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
        ArgumentNullException.ThrowIfNull(product);

        var result = new List<ProductCombinationModel>();

        var combinations = await _productAttributeService
            .GetAllProductAttributeCombinationsAsync(product.Id);
        if (combinations?.Any() == true)
        {
            foreach (var combination in combinations)
            {
                var combinationModel = new ProductCombinationModel
                {
                    InStock = combination.StockQuantity > 0 || combination.AllowOutOfStockOrders
                };

                var mappings = await _productAttributeParser
                    .ParseProductAttributeMappingsAsync(combination.AttributesXml);
                if (mappings == null || !mappings.Any())
                    continue;

                foreach (var mapping in mappings)
                {
                    var attributeModel = new ProductAttributeModel
                    {
                        Id = mapping.Id
                    };

                    var values = await _productAttributeParser
                        .ParseProductAttributeValuesAsync(combination.AttributesXml, mapping.Id);
                    if (values == null || !values.Any())
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
        ArgumentNullException.ThrowIfNull(product);

        //standard properties
        var model = new ProductDetailsModel
        {
            Id = product.Id,
            Name = await _localizationService.GetLocalizedAsync(product, x => x.Name),
            ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription),
            FullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription),
            MetaKeywords = await _localizationService.GetLocalizedAsync(product, x => x.MetaKeywords),
            MetaDescription = await _localizationService.GetLocalizedAsync(product, x => x.MetaDescription),
            MetaTitle = await _localizationService.GetLocalizedAsync(product, x => x.MetaTitle),
            SeName = await _urlRecordService.GetSeNameAsync(product),
            ProductType = product.ProductType,
            ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage,
            Sku = product.Sku,
            ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber,
            FreeShippingNotificationEnabled = _catalogSettings.ShowFreeShippingNotification,
            ManufacturerPartNumber = product.ManufacturerPartNumber,
            ShowGtin = _catalogSettings.ShowGtin,
            Gtin = product.Gtin,
            ManageInventoryMethod = product.ManageInventoryMethod,
            StockAvailability = await _productService.FormatStockMessageAsync(product, string.Empty),
            HasSampleDownload = product.IsDownload && product.HasSampleDownload,
            DisplayDiscontinuedMessage = !product.Published && _catalogSettings.DisplayDiscontinuedMessageForUnpublishedProducts,
            AvailableEndDate = product.AvailableEndDateTimeUtc,
            VisibleIndividually = product.VisibleIndividually,
            AllowAddingOnlyExistingAttributeCombinations = product.AllowAddingOnlyExistingAttributeCombinations,
            DisplayAttributeCombinationImagesOnly = product.DisplayAttributeCombinationImagesOnly
        };

        //automatically generate product description?
        if (_seoSettings.GenerateProductMetaDescription && string.IsNullOrEmpty(model.MetaDescription))
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
            var deliveryDate = await _dateRangeService.GetDeliveryDateByIdAsync(product.DeliveryDateId);
            if (deliveryDate != null)
            {
                model.DeliveryDate = await _localizationService.GetLocalizedAsync(deliveryDate, dd => dd.Name);
            }
        }

        var store = await _storeContext.GetCurrentStoreAsync();
        //email a friend
        model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
        //compare products
        model.CompareProductsEnabled = _catalogSettings.CompareProductsEnabled;
        //store name
        model.CurrentStoreName = await _localizationService.GetLocalizedAsync(store, x => x.Name);

        //vendor details
        if (_vendorSettings.ShowVendorOnProductDetailsPage)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(product.VendorId);
            if (vendor != null && !vendor.Deleted && vendor.Active)
            {
                model.ShowVendor = true;

                model.VendorModel = new VendorBriefInfoModel
                {
                    Id = vendor.Id,
                    Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(vendor),
                };
            }
        }

        //page sharing
        if (_catalogSettings.ShowShareButton && !string.IsNullOrEmpty(_catalogSettings.PageShareCode))
        {
            var shareCode = _catalogSettings.PageShareCode;
            if (_webHelper.IsCurrentConnectionSecured())
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
                                || await _productService.GetTotalStockQuantityAsync(product) > 0;
                model.DisplayBackInStockSubscription = !model.InStock && product.AllowBackInStockSubscriptions;
                break;

            case ManageInventoryMethod.ManageStockByAttributes:
                model.InStock = (await _productAttributeService
                                    .GetAllProductAttributeCombinationsAsync(product.Id))
                                ?.Any(c => c.StockQuantity > 0 || c.AllowOutOfStockOrders)
                                ?? false;
                break;
        }

        //breadcrumb
        //do not prepare this model for the associated products. anyway it's not used
        if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedProduct)
        {
            model.Breadcrumb = await PrepareProductBreadcrumbModelAsync(product);
        }

        //product tags
        //do not prepare this model for the associated products. anyway it's not used
        if (!isAssociatedProduct)
        {
            model.ProductTags = await PrepareProductTagModelsAsync(product);
        }

        //pictures and videos
        model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
        IList<PictureModel> allPictureModels;
        IList<VideoModel> allVideoModels;
        (model.DefaultPictureModel, allPictureModels, allVideoModels) = await PrepareProductDetailsPictureModelAsync(product, isAssociatedProduct);
        model.PictureModels = allPictureModels;
        model.VideoModels = allVideoModels;

        //price
        model.ProductPrice = await PrepareProductPriceModelAsync(product);

        //'Add to cart' model
        model.AddToCart = await PrepareProductAddToCartModelAsync(product, updatecartitem);
        var customer = await _workContext.GetCurrentCustomerAsync();
        //gift card
        if (product.IsGiftCard)
        {
            model.GiftCard.IsGiftCard = true;
            model.GiftCard.GiftCardType = product.GiftCardType;

            if (updatecartitem == null)
            {
                model.GiftCard.SenderName = await _customerService.GetCustomerFullNameAsync(customer);
                model.GiftCard.SenderEmail = customer.Email;
            }
            else
            {
                _productAttributeParser.GetGiftCardAttribute(updatecartitem.AttributesXml,
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

        model.ProductReviews = await PrepareProductReviewsModelAsync(product);

        if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
            model.TierPrices = await PrepareProductTierPriceModelsAsync(product);

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
        if (_shippingSettings.EstimateShippingProductPageEnabled && !model.IsFreeShipping)
        {
            var wrappedProduct = new ShoppingCartItem
            {
                StoreId = store.Id,
                ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart,
                CustomerId = customer.Id,
                ProductId = product.Id,
                CreatedOnUtc = DateTime.UtcNow
            };

            var estimateShippingModel = await _shoppingCartModelFactory.PrepareEstimateShippingModelAsync(new[] { wrappedProduct });

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
                var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id, store.Id);
                foreach (var associatedProduct in associatedProducts)
                    model.AssociatedProducts.Add(await PrepareProductDetailsModelAsync(associatedProduct, null, true));
            }
            model.InStock = model.AssociatedProducts.Any(associatedProduct => associatedProduct.InStock);
        }
        if (_seoSettings.MicrodataEnabled)
        {
            var jsonLdModel = await _jsonLdModelFactory.PrepareJsonLdProductAsync(model);
            model.JsonLd = JsonConvert.SerializeObject(jsonLdModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        return model;
    }

    /// <summary>
    /// Prepare the product reviews model
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product reviews model
    /// </returns>
    public virtual async Task<ProductReviewsModel> PrepareProductReviewsModelAsync(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        var model = new ProductReviewsModel
        {
            ProductId = product.Id
        };

        var currentStore = await _storeContext.GetCurrentStoreAsync();

        var productReviews = await _productService.GetAllProductReviewsAsync(
            approved: true,
            productId: product.Id,
            storeId: _catalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0);

        //get all review types
        foreach (var reviewType in await _reviewTypeService.GetAllReviewTypesAsync())
        {
            model.ReviewTypeList.Add(new ReviewTypeModel
            {
                Id = reviewType.Id,
                Name = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Name),
                Description = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Description),
                VisibleToAllCustomers = reviewType.VisibleToAllCustomers,
                DisplayOrder = reviewType.DisplayOrder,
                IsRequired = reviewType.IsRequired,
            });
        }

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();

        //filling data from db
        foreach (var pr in productReviews)
        {
            var customer = await _customerService.GetCustomerByIdAsync(pr.CustomerId);

            var productReviewModel = new ProductReviewModel
            {
                Id = pr.Id,
                CustomerId = pr.CustomerId,
                CustomerName = await _customerService.FormatUsernameAsync(customer),
                AllowViewingProfiles = _customerSettings.AllowViewingProfiles && customer != null && !await _customerService.IsGuestAsync(customer),
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
                WrittenOnStr = (await _dateTimeHelper.ConvertToUserTimeAsync(pr.CreatedOnUtc, DateTimeKind.Utc)).ToString("g"),
            };

            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                productReviewModel.CustomerAvatarUrl = await _pictureService.GetPictureUrlAsync(
                    await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                    _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
            }

            foreach (var q in await _reviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewIdAsync(pr.Id))
            {
                var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(q.ReviewTypeId);

                productReviewModel.AdditionalProductReviewList.Add(new ProductReviewReviewTypeMappingModel
                {
                    ReviewTypeId = q.ReviewTypeId,
                    ProductReviewId = pr.Id,
                    Rating = q.Rating,
                    Name = await _localizationService.GetLocalizedAsync(reviewType, x => x.Name),
                    VisibleToAllCustomers = reviewType.VisibleToAllCustomers || currentCustomer.Id == pr.CustomerId,
                });
            }

            model.Items.Add(productReviewModel);
        }

        foreach (var rt in model.ReviewTypeList)
        {
            if (model.ReviewTypeList.Count <= model.AddAdditionalProductReviewList.Count)
                continue;
            var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(rt.Id);
            var reviewTypeMappingModel = new AddProductReviewReviewTypeMappingModel
            {
                ReviewTypeId = rt.Id,
                Name = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Name),
                Description = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Description),
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

        model.AddProductReview.CanCurrentCustomerLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewProduct || !await _customerService.IsGuestAsync(currentCustomer);
        model.AddProductReview.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage;
        model.AddProductReview.CanAddNewReview = await _productService.CanAddReviewAsync(product.Id, _catalogSettings.ShowProductReviewsPerStore ? currentStore.Id : 0);

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
        var pageSize = _catalogSettings.ProductReviewsPageSizeOnAccountPage;
        var pageIndex = 0;

        if (page > 0)
        {
            pageIndex = page.Value - 1;
        }

        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();

        var list = await _productService.GetAllProductReviewsAsync(
            customerId: customer.Id,
            approved: null,
            storeId: _catalogSettings.ShowProductReviewsPerStore ? store.Id : 0,
            pageIndex: pageIndex,
            pageSize: pageSize);

        var productReviews = new List<CustomerProductReviewModel>();

        foreach (var review in list)
        {
            var product = await _productService.GetProductByIdAsync(review.ProductId);

            var productReviewModel = new CustomerProductReviewModel
            {
                Title = review.Title,
                ProductId = product.Id,
                ProductName = await _localizationService.GetLocalizedAsync(product, p => p.Name),
                ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                Rating = review.Rating,
                ReviewText = review.ReviewText,
                ReplyText = review.ReplyText,
                WrittenOnStr = (await _dateTimeHelper.ConvertToUserTimeAsync(review.CreatedOnUtc, DateTimeKind.Utc)).ToString("g")
            };

            if (_catalogSettings.ProductReviewsMustBeApproved)
            {
                productReviewModel.ApprovalStatus = review.IsApproved
                    ? await _localizationService.GetResourceAsync("Account.CustomerProductReviews.ApprovalStatus.Approved")
                    : await _localizationService.GetResourceAsync("Account.CustomerProductReviews.ApprovalStatus.Pending");
            }

            foreach (var q in await _reviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewIdAsync(review.Id))
            {
                var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(q.ReviewTypeId);

                productReviewModel.AdditionalProductReviewList.Add(new ProductReviewReviewTypeMappingModel
                {
                    ReviewTypeId = q.ReviewTypeId,
                    ProductReviewId = review.Id,
                    Rating = q.Rating,
                    Name = await _localizationService.GetLocalizedAsync(reviewType, x => x.Name),
                });
            }

            productReviews.Add(productReviewModel);
        }

        var pagerModel = new PagerModel(_localizationService)
        {
            PageSize = list.PageSize,
            TotalRecords = list.TotalCount,
            PageIndex = list.PageIndex,
            ShowTotalSummary = false,
            RouteActionName = "CustomerProductReviewsPaged",
            UseRouteLinks = true,
            RouteValues = new CustomerProductReviewsModel.CustomerProductReviewsRouteValues { PageNumber = pageIndex }
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
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(product);

        model.ProductId = product.Id;
        model.ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
        model.ProductSeName = await _urlRecordService.GetSeNameAsync(product);
        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailProductToFriendPage;
        if (!excludeProperties)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
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
        ArgumentNullException.ThrowIfNull(product);

        var model = new ProductSpecificationModel();

        // Add non-grouped attributes first
        model.Groups.Add(new ProductSpecificationAttributeGroupModel
        {
            Attributes = await PrepareProductSpecificationAttributeModelAsync(product, null)
        });

        // Add grouped attributes
        var groups = await _specificationAttributeService.GetProductSpecificationAttributeGroupsAsync(product.Id);
        foreach (var group in groups)
        {
            model.Groups.Add(new ProductSpecificationAttributeGroupModel
            {
                Id = group.Id,
                Name = await _localizationService.GetLocalizedAsync(group, x => x.Name),
                Attributes = await PrepareProductSpecificationAttributeModelAsync(product, group)
            });
        }

        return model;
    }

    #endregion
}