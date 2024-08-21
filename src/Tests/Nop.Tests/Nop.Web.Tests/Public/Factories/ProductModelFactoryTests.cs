using FluentAssertions;
using Nop.Core.Caching;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Data;
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
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class ProductModelFactoryTests : WebTest
{
    private IProductModelFactory _productModelFactory;
    private IProductService _productService;
    private IUrlRecordService _urlRecordService;

    private ProductModelFactoryForTest _productModelFactoryForTest;

    [OneTimeSetUp]
    public void SetUp()
    {
        _productModelFactory = GetService<IProductModelFactory>();
        _productService = GetService<IProductService>();
        _urlRecordService = GetService<IUrlRecordService>();
        _productModelFactoryForTest = GetService<ProductModelFactoryForTest>();
    }

    [Test]
    public async Task CanPreparePriceModel()
    {
        foreach (var product in await _productService.SearchProductsAsync())
        {
            PropertiesShouldEqual(await _productModelFactoryForTest.NewPrepareProductPriceModelAsync(product, true), await _productModelFactoryForTest.PrepareProductOverviewPriceModelAsync(product), "CustomProperties");
            PropertiesShouldEqual(await _productModelFactoryForTest.NewPrepareProductPriceModelAsync(product, true, true), await _productModelFactoryForTest.PrepareProductOverviewPriceModelAsync(product, true), "CustomProperties");
            PropertiesShouldEqual(await _productModelFactoryForTest.NewPrepareProductPriceModelAsync(product), await _productModelFactoryForTest.PrepareProductPriceModelAsync(product), "CustomProperties");
        }
    }

    [Test]
    public async Task CanPrepareProductTemplateViewPath()
    {
        var productTemplateRepository = GetService<IRepository<ProductTemplate>>();
        var productTemplateSimple = productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Simple product") ?? throw new Exception("Simple product template could not be loaded");
        var productTemplateGrouped = productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Grouped product (with variants)") ?? throw new Exception("Grouped product template could not be loaded");

        var modelSimple = await _productModelFactory.PrepareProductTemplateViewPathAsync(new Product
        {
            ProductTemplateId = productTemplateSimple.Id
        });

        var modelGrouped = await _productModelFactory.PrepareProductTemplateViewPathAsync(new Product
        {
            ProductTemplateId = productTemplateGrouped.Id
        });

        modelSimple.Should().NotBe(modelGrouped);

        modelSimple.Should().Be(productTemplateSimple.ViewPath);
        modelGrouped.Should().Be(productTemplateGrouped.ViewPath);
    }

    [Test]
    public async Task CanPrepareProductOverviewModels()
    {
        var product = await _productService.GetProductByIdAsync(1);
        var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(new[] { product })).FirstOrDefault();

        PropertiesShouldEqual(product, model);
    }

    [Test]
    public async Task CanPrepareProductDetailsModel()
    {
        var product = await _productService.GetProductByIdAsync(1);
        var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(new[] { product })).FirstOrDefault();

        PropertiesShouldEqual(product, model);
    }

    [Test]
    public async Task CanPrepareProductReviewsModel()
    {
        var pId = (await _productService.GetProductReviewByIdAsync(1)).ProductId;
        var product = await _productService.GetProductByIdAsync(pId);
        var model = await _productModelFactory.PrepareProductReviewsModelAsync(product);

        model.ProductId.Should().Be(product.Id);

        model.Items.Any().Should().BeTrue();
    }

    [Test]
    public async Task CanPrepareCustomerProductReviewsModel()
    {
        var model = await _productModelFactory.PrepareCustomerProductReviewsModelAsync(null);
        var review = model.ProductReviews.FirstOrDefault();

        review.Should().NotBeNull();

        var product = await _productService.GetProductByIdAsync(review.ProductId);

        review.ProductName.Should().Be(product.Name);
        review.ProductSeName.Should().Be(await _urlRecordService.GetSeNameAsync(product));
    }

    [Test]
    public async Task CanPrepareProductEmailAFriendModel()
    {
        var product = await _productService.GetProductByIdAsync(1);
        var model = await _productModelFactory.PrepareProductEmailAFriendModelAsync(new ProductEmailAFriendModel(), product, false);

        model.ProductId.Should().Be(product.Id);
        model.ProductName.Should().Be(product.Name);
        model.ProductSeName.Should().Be(await GetService<IUrlRecordService>().GetSeNameAsync(product));
        model.YourEmailAddress.Should().Be(NopTestsDefaults.AdminEmail);
    }

    [Test]
    public async Task CanPrepareProductSpecificationModel()
    {
        var product = await _productService.GetProductByIdAsync(1);
        var model = await _productModelFactory.PrepareProductSpecificationModelAsync(product);

        var group = model.Groups.FirstOrDefault();

        group.Should().NotBe(null);
    }

    #region Nested class

    public class ProductModelFactoryForTest : ProductModelFactory
    {
        public ProductModelFactoryForTest(CaptchaSettings captchaSettings, CatalogSettings catalogSettings, CustomerSettings customerSettings, ICategoryService categoryService, ICurrencyService currencyService, ICustomerService customerService, IDateRangeService dateRangeService, IDateTimeHelper dateTimeHelper, IDownloadService downloadService, IGenericAttributeService genericAttributeService, IJsonLdModelFactory jsonLdModelFactory, ILocalizationService localizationService, IManufacturerService manufacturerService, IPermissionService permissionService, IPictureService pictureService, IPriceCalculationService priceCalculationService, IPriceFormatter priceFormatter, IProductAttributeParser productAttributeParser, IProductAttributeService productAttributeService, IProductService productService, IProductTagService productTagService, IProductTemplateService productTemplateService, IReviewTypeService reviewTypeService, IShoppingCartService shoppingCartService, ISpecificationAttributeService specificationAttributeService, IStaticCacheManager staticCacheManager, IStoreContext storeContext, IStoreService storeService, IShoppingCartModelFactory shoppingCartModelFactory, ITaxService taxService, IUrlRecordService urlRecordService, IVendorService vendorService, IVideoService videoService, IWebHelper webHelper, IWorkContext workContext, MediaSettings mediaSettings, OrderSettings orderSettings, SeoSettings seoSettings, ShippingSettings shippingSettings, VendorSettings vendorSettings) : base(captchaSettings, catalogSettings, customerSettings, categoryService, currencyService, customerService, dateRangeService, dateTimeHelper, downloadService, genericAttributeService, jsonLdModelFactory, localizationService, manufacturerService, permissionService, pictureService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, productService, productTagService, productTemplateService, reviewTypeService, shoppingCartService, specificationAttributeService, staticCacheManager, storeContext, storeService, shoppingCartModelFactory, taxService, urlRecordService, vendorService, videoService, webHelper, workContext, mediaSettings, orderSettings, seoSettings, shippingSettings, vendorSettings)
        {
        }

        #region Methods

        /// <summary>
        /// Prepare the product overview price model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product overview price model
        /// </returns>
        public virtual async Task<ProductPriceModel> PrepareProductOverviewPriceModelAsync(Product product, bool forceRedirectionAfterAddingToCart = false)
        {
            ArgumentNullException.ThrowIfNull(product);

            async Task prepareSimpleProductOverviewPriceModel(ProductPriceModel priceModel)
            {
                //add to cart button
                priceModel.DisableBuyButton = product.DisableBuyButton ||
                                              !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART) ||
                                              !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES);

                //add to wishlist button
                priceModel.DisableWishlistButton = product.DisableWishlistButton ||
                                                   !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST) ||
                                                   !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES);
                //compare products
                priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;

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
                if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
                {
                    if (product.CustomerEntersPrice)
                    {
                        priceModel.CustomerEntersPrice = true;
                        return;
                    }
                    if (product.CallForPrice &&
                        //also check whether the current user is impersonated
                        (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
                            _workContext.OriginalCustomerIfImpersonated == null))
                    {
                        //call for price
                        priceModel.OldPrice = null;
                        priceModel.OldPriceValue = null;
                        priceModel.Price = await _localizationService.GetResourceAsync("Products.CallForPrice");
                        priceModel.PriceValue = null;
                    }
                    else
                    {
                        var store = await _storeContext.GetCurrentStoreAsync();
                        var customer = await _workContext.GetCurrentCustomerAsync();

                        //prices
                        var (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = (decimal.Zero, decimal.Zero);
                        var hasMultiplePrices = false;
                        if (_catalogSettings.DisplayFromPrices)
                        {
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
                                    (var priceWithoutDiscount, var priceWithDiscount, _, _) = await _priceCalculationService
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
                                        (var priceWithoutDiscount, var priceWithDiscount, _, _) = await _priceCalculationService
                                            .GetFinalPriceAsync(product, customer, store, combination.OverriddenPrice.Value, decimal.Zero, true, 1, null, null);
                                        prices.Add((priceWithoutDiscount, priceWithDiscount));
                                    }
                                    else
                                    {
                                        var additionalCharge = decimal.Zero;
                                        var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
                                        foreach (var attributeValue in attributeValues)
                                        {
                                            additionalCharge += await _priceCalculationService.
                                                GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer, store);
                                        }
                                        if (additionalCharge != decimal.Zero)
                                        {
                                            (var priceWithoutDiscount, var priceWithDiscount, _, _) = await _priceCalculationService
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
                        }
                        else
                            (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store);

                        var (tierPriceMinPossiblePriceWithoutDiscount, tierPriceMinPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store, quantity: int.MaxValue);

                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                        minPossiblePriceWithoutDiscount = Math.Min(minPossiblePriceWithoutDiscount, tierPriceMinPossiblePriceWithoutDiscount);
                        minPossiblePriceWithDiscount = Math.Min(minPossiblePriceWithDiscount, tierPriceMinPossiblePriceWithDiscount);

                        var (oldPriceBase, _) = await _taxService.GetProductPriceAsync(product, product.OldPrice);
                        var (finalPriceWithoutDiscountBase, _) = await _taxService.GetProductPriceAsync(product, minPossiblePriceWithoutDiscount);
                        var (finalPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, minPossiblePriceWithDiscount);
                        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
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
                            priceModel.OldPrice = await _priceFormatter.FormatPriceAsync(strikeThroughPrice);
                            priceModel.OldPriceValue = strikeThroughPrice;
                        }
                        else
                        {
                            priceModel.OldPrice = null;
                            priceModel.OldPriceValue = null;
                        }

                        //do we have tier prices configured?
                        var tierPrices = await _productService.GetTierPricesAsync(product, customer, store);

                        //When there is just one tier price (with  qty 1), there are no actual savings in the list.
                        var hasTierPrices = tierPrices.Any() && !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);

                        var price = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                        priceModel.Price = hasTierPrices || hasMultiplePrices
                            ? string.Format(await _localizationService.GetResourceAsync("Products.PriceRangeFrom"), price)
                            : price;
                        priceModel.PriceValue = finalPriceWithDiscount;

                        if (product.IsRental)
                        {
                            //rental product
                            priceModel.OldPrice = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceModel.OldPrice);
                            priceModel.RentalPrice = priceModel.Price = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceModel.Price);
                            priceModel.RentalPriceValue = finalPriceWithDiscount;
                        }

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this product
                        //we also ensure this it's not free shipping
                        priceModel.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductBoxes && product.IsShipEnabled && !product.IsFreeShipping;

                        //PAngV default baseprice (used in Germany)
                        priceModel.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(product, finalPriceWithDiscount);
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

            async Task prepareGroupedProductOverviewPriceModel(ProductPriceModel priceModel)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id,
                    store.Id);

                //add to cart button (ignore "DisableBuyButton" property for grouped products)
                priceModel.DisableBuyButton =
                    !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART) ||
                    !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES);

                //add to wishlist button (ignore "DisableWishlistButton" property for grouped products)
                priceModel.DisableWishlistButton =
                    !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST) ||
                    !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES);

                //compare products
                priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;
                if (!associatedProducts.Any())
                    return;

                //we have at least one associated product
                if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
                {
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
                    {
                        priceModel.CustomerEntersPrice = minPriceProduct?.CustomerEntersPrice ?? product.CustomerEntersPrice;
                        return;
                    }

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
                else
                {
                    //hide prices
                    priceModel.OldPrice = null;
                    priceModel.OldPriceValue = null;
                    priceModel.Price = null;
                    priceModel.PriceValue = null;
                }
            }

            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();

            var priceModel = new ProductPriceModel
            {
                ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart,
                CurrencyCode = currentCurrency.CurrencyCode,
                ProductId = product.Id
            };

            switch (product.ProductType)
            {
                case ProductType.GroupedProduct:
                    //grouped product
                    await prepareGroupedProductOverviewPriceModel(priceModel);

                    break;
                case ProductType.SimpleProduct:
                default:
                    //simple product
                    await prepareSimpleProductOverviewPriceModel(priceModel);

                    break;
            }

            return priceModel;
        }

        /// <summary>
        /// Prepare the product price model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product price model
        /// </returns>
        public virtual async Task<ProductPriceModel> PrepareProductPriceModelAsync(Product product)
        {
            ArgumentNullException.ThrowIfNull(product);
            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();

            var model = new ProductPriceModel
            {
                ProductId = product.Id,
                //currency code
                CurrencyCode = currentCurrency.CurrencyCode
            };

            if (await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.DISPLAY_PRICES))
            {
                model.HidePrices = false;
                if (product.CustomerEntersPrice)
                    model.CustomerEntersPrice = true;
                else
                {
                    if (product.CallForPrice &&
                        //also check whether the current user is impersonated
                        (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
                    {
                        model.CallForPrice = true;
                    }
                    else
                    {
                        var customer = await _workContext.GetCurrentCustomerAsync();
                        var store = await _storeContext.GetCurrentStoreAsync();

                        var (oldPriceBase, _) = await _taxService.GetProductPriceAsync(product, product.OldPrice);

                        var (finalPriceWithoutDiscountBase, _) = await _taxService.GetProductPriceAsync(product, (await _priceCalculationService.GetFinalPriceAsync(product, customer, store, includeDiscounts: false)).finalPrice);
                        var (finalPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, (await _priceCalculationService.GetFinalPriceAsync(product, customer, store)).finalPrice);

                        var oldPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(oldPriceBase, currentCurrency);
                        var finalPriceWithoutDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithoutDiscountBase, currentCurrency);
                        var finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, currentCurrency);

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                        {
                            model.OldPrice = await _priceFormatter.FormatPriceAsync(oldPrice);
                            model.OldPriceValue = oldPrice;
                        }

                        model.Price = await _priceFormatter.FormatPriceAsync(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                        {
                            model.PriceWithDiscount = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                            model.PriceWithDiscountValue = finalPriceWithDiscount;
                        }

                        model.PriceValue = finalPriceWithDiscount;

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this product
                        //we also ensure this it's not free shipping
                        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductDetailsPage
                                                       && product.IsShipEnabled &&
                                                       !product.IsFreeShipping;

                        //PAngV baseprice (used in Germany)
                        model.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(product, finalPriceWithDiscountBase);
                        model.BasePricePAngVValue = finalPriceWithDiscountBase;
                        
                        //rental
                        if (product.IsRental)
                        {
                            model.IsRental = true;
                            var priceStr = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                            model.Price = model.RentalPrice = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceStr);
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

        public async Task<ProductPriceModel> NewPrepareProductPriceModelAsync(Product product,
            bool addPriceRangeFrom = false, bool forceRedirectionAfterAddingToCart = false)
        {
            return await PrepareProductPriceModelAsync(product, addPriceRangeFrom, forceRedirectionAfterAddingToCart);
        }

        #endregion
    }

    #endregion
}