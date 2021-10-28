using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the shopping cart model factory
    /// </summary>
    public partial class ShoppingCartModelFactory : IShoppingCartModelFactory
    {
        #region Fields

        protected AddressSettings AddressSettings { get; }
        protected CaptchaSettings CaptchaSettings { get; }
        protected CatalogSettings CatalogSettings { get; }
        protected CommonSettings CommonSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected IAddressModelFactory AddressModelFactory { get; }
        protected ICheckoutAttributeFormatter CheckoutAttributeFormatter { get; }
        protected ICheckoutAttributeParser CheckoutAttributeParser { get; }
        protected ICheckoutAttributeService CheckoutAttributeService { get; }
        protected ICountryService CountryService { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IDiscountService DiscountService { get; }
        protected IDownloadService DownloadService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IGiftCardService GiftCardService { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IOrderProcessingService OrderProcessingService { get; }
        protected IOrderTotalCalculationService OrderTotalCalculationService { get; }
        protected IPaymentPluginManager PaymentPluginManager { get; }
        protected IPaymentService PaymentService { get; }
        protected IPermissionService PermissionService { get; }
        protected IPictureService PictureService { get; }
        protected IPriceFormatter PriceFormatter { get; }
        protected IProductAttributeFormatter ProductAttributeFormatter { get; }
        protected IProductService ProductService { get; }
        protected IShippingService ShippingService { get; }
        protected IShoppingCartService ShoppingCartService { get; }
        protected IStateProvinceService StateProvinceService { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected ITaxService TaxService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IVendorService VendorService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }
        protected MediaSettings MediaSettings { get; }
        protected OrderSettings OrderSettings { get; }
        protected RewardPointsSettings RewardPointsSettings { get; }
        protected ShippingSettings ShippingSettings { get; }
        protected ShoppingCartSettings ShoppingCartSettings { get; }
        protected TaxSettings TaxSettings { get; }
        protected VendorSettings VendorSettings { get; }

        #endregion

        #region Ctor

        public ShoppingCartModelFactory(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            IAddressModelFactory addressModelFactory,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings)
        {
            AddressSettings = addressSettings;
            CaptchaSettings = captchaSettings;
            CatalogSettings = catalogSettings;
            CommonSettings = commonSettings;
            CustomerSettings = customerSettings;
            AddressModelFactory = addressModelFactory;
            CheckoutAttributeFormatter = checkoutAttributeFormatter;
            CheckoutAttributeParser = checkoutAttributeParser;
            CheckoutAttributeService = checkoutAttributeService;
            CountryService = countryService;
            CurrencyService = currencyService;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            DiscountService = discountService;
            DownloadService = downloadService;
            GenericAttributeService = genericAttributeService;
            GiftCardService = giftCardService;
            HttpContextAccessor = httpContextAccessor;
            LocalizationService = localizationService;
            OrderProcessingService = orderProcessingService;
            OrderTotalCalculationService = orderTotalCalculationService;
            PaymentPluginManager = paymentPluginManager;
            PaymentService = paymentService;
            PermissionService = permissionService;
            PictureService = pictureService;
            PriceFormatter = priceFormatter;
            ProductAttributeFormatter = productAttributeFormatter;
            ProductService = productService;
            ShippingService = shippingService;
            ShoppingCartService = shoppingCartService;
            StateProvinceService = stateProvinceService;
            StaticCacheManager = staticCacheManager;
            StoreContext = storeContext;
            StoreMappingService = storeMappingService;
            TaxService = taxService;
            UrlRecordService = urlRecordService;
            VendorService = vendorService;
            WebHelper = webHelper;
            WorkContext = workContext;
            MediaSettings = mediaSettings;
            OrderSettings = orderSettings;
            RewardPointsSettings = rewardPointsSettings;
            ShippingSettings = shippingSettings;
            ShoppingCartSettings = shoppingCartSettings;
            TaxSettings = taxSettings;
            VendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the checkout attribute models
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the checkout attribute model
        /// </returns>
        protected virtual async Task<IList<ShoppingCartModel.CheckoutAttributeModel>> PrepareCheckoutAttributeModelsAsync(
            IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new List<ShoppingCartModel.CheckoutAttributeModel>();
            var store = await StoreContext.GetCurrentStoreAsync();
            var excludeShippableAttributes = !await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            var checkoutAttributes =
                await CheckoutAttributeService.GetAllCheckoutAttributesAsync(store.Id,
                    excludeShippableAttributes);
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new ShoppingCartModel.CheckoutAttributeModel
                {
                    Id = attribute.Id,
                    Name = await LocalizationService.GetLocalizedAsync(attribute, x => x.Name),
                    TextPrompt = await LocalizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = await LocalizationService.GetLocalizedAsync(attribute, x => x.DefaultValue)
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
                    var attributeValues = await CheckoutAttributeService.GetCheckoutAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new ShoppingCartModel.CheckoutAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = await LocalizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                        };
                        attributeModel.Values.Add(attributeValueModel);

                        //display price if allowed
                        if (await PermissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
                        {
                            var (priceAdjustmentBase, _) = await TaxService.GetCheckoutAttributePriceAsync(attribute, attributeValue);
                            var priceAdjustment =
                                await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase,
                                    await WorkContext.GetWorkingCurrencyAsync());
                            if (priceAdjustmentBase > decimal.Zero)
                                attributeValueModel.PriceAdjustment =
                                    "+" + await PriceFormatter.FormatPriceAsync(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                attributeValueModel.PriceAdjustment =
                                    "-" + await PriceFormatter.FormatPriceAsync(-priceAdjustment);
                        }
                    }
                }

                //set already selected attributes
                var selectedCheckoutAttributes = await GenericAttributeService.GetAttributeAsync<string>(
                    await WorkContext.GetCurrentCustomerAsync(),
                    NopCustomerDefaults.CheckoutAttributes, store.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    {
                        if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                        {
                            //clear default selection
                            foreach (var item in attributeModel.Values)
                                item.IsPreSelected = false;

                            //select new values
                            var selectedValues =
                                CheckoutAttributeParser.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                            foreach (var attributeValue in await selectedValues.SelectMany(x => x.values).ToListAsync())
                                foreach (var item in attributeModel.Values)
                                    if (attributeValue.Id == item.Id)
                                        item.IsPreSelected = true;
                        }
                    }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                    {
                        //do nothing
                        //values are already pre-set
                    }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                    {
                        if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                        {
                            var enteredText =
                                CheckoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                            if (enteredText.Any())
                                attributeModel.DefaultValue = enteredText[0];
                        }
                    }

                        break;
                    case AttributeControlType.Datepicker:
                    {
                        //keep in mind my that the code below works only in the current culture
                        var selectedDateStr =
                            CheckoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                        if (selectedDateStr.Any())
                        {
                            if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                DateTimeStyles.None, out var selectedDate))
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
                        if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                        {
                            var downloadGuidStr = CheckoutAttributeParser
                                .ParseValues(selectedCheckoutAttributes, attribute.Id).FirstOrDefault();
                            Guid.TryParse(downloadGuidStr, out var downloadGuid);
                            var download = await DownloadService.GetDownloadByGuidAsync(downloadGuid);
                            if (download != null)
                                attributeModel.DefaultValue = download.DownloadGuid.ToString();
                        }
                    }

                        break;
                    default:
                        break;
                }

                model.Add(attributeModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the shopping cart item model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="sci">Shopping cart item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart item model
        /// </returns>
        protected virtual async Task<ShoppingCartModel.ShoppingCartItemModel> PrepareShoppingCartItemModelAsync(IList<ShoppingCartItem> cart, ShoppingCartItem sci)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (sci == null)
                throw new ArgumentNullException(nameof(sci));

            var product = await ProductService.GetProductByIdAsync(sci.ProductId);

            var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
            {
                Id = sci.Id,
                Sku = await ProductService.FormatSkuAsync(product, sci.AttributesXml),
                VendorName = VendorSettings.ShowVendorOnOrderDetailsPage ? (await VendorService.GetVendorByProductIdAsync(product.Id))?.Name : string.Empty,
                ProductId = sci.ProductId,
                ProductName = await LocalizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await UrlRecordService.GetSeNameAsync(product),
                Quantity = sci.Quantity,
                AttributeInfo = await ProductAttributeFormatter.FormatAttributesAsync(product, sci.AttributesXml),
            };

            //allow editing?
            //1. setting enabled?
            //2. simple product?
            //3. has attribute or gift card?
            //4. visible individually?
            cartItemModel.AllowItemEditing = ShoppingCartSettings.AllowCartItemEditing &&
                                             product.ProductType == ProductType.SimpleProduct &&
                                             (!string.IsNullOrEmpty(cartItemModel.AttributeInfo) ||
                                              product.IsGiftCard) &&
                                             product.VisibleIndividually;

            //disable removal?
            //1. do other items require this one?
            cartItemModel.DisableRemoval = (await ShoppingCartService.GetProductsRequiringProductAsync(cart, product)).Any();

            //allowed quantities
            var allowedQuantities = ProductService.ParseAllowedQuantities(product);
            foreach (var qty in allowedQuantities)
            {
                cartItemModel.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = sci.Quantity == qty
                });
            }

            //recurring info
            if (product.IsRecurring)
                cartItemModel.RecurringInfo = string.Format(await LocalizationService.GetResourceAsync("ShoppingCart.RecurringPeriod"),
                        product.RecurringCycleLength, await LocalizationService.GetLocalizedEnumAsync(product.RecurringCyclePeriod));

            //rental info
            if (product.IsRental)
            {
                var rentalStartDate = sci.RentalStartDateUtc.HasValue
                    ? ProductService.FormatRentalDate(product, sci.RentalStartDateUtc.Value)
                    : string.Empty;
                var rentalEndDate = sci.RentalEndDateUtc.HasValue
                    ? ProductService.FormatRentalDate(product, sci.RentalEndDateUtc.Value)
                    : string.Empty;
                cartItemModel.RentalInfo =
                    string.Format(await LocalizationService.GetResourceAsync("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
            }

            //unit prices
            var currentCurrency = await WorkContext.GetWorkingCurrencyAsync();
            if (product.CallForPrice &&
                //also check whether the current user is impersonated
                (!OrderSettings.AllowAdminsToBuyCallForPriceProducts || WorkContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.UnitPrice = await LocalizationService.GetResourceAsync("Products.CallForPrice");
                cartItemModel.UnitPriceValue = 0;
            }
            else
            {
                var (shoppingCartUnitPriceWithDiscountBase, _) = await TaxService.GetProductPriceAsync(product, (await ShoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice);
                var shoppingCartUnitPriceWithDiscount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartUnitPriceWithDiscountBase, currentCurrency);
                cartItemModel.UnitPrice = await PriceFormatter.FormatPriceAsync(shoppingCartUnitPriceWithDiscount);
                cartItemModel.UnitPriceValue = shoppingCartUnitPriceWithDiscount;
            }
            //subtotal, discount
            if (product.CallForPrice &&
                //also check whether the current user is impersonated
                (!OrderSettings.AllowAdminsToBuyCallForPriceProducts || WorkContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.SubTotal = await LocalizationService.GetResourceAsync("Products.CallForPrice");
                cartItemModel.SubTotalValue = 0;
            }
            else
            {
                //sub total
                var (subTotal, shoppingCartItemDiscountBase, _, maximumDiscountQty) = await ShoppingCartService.GetSubTotalAsync(sci, true);
                var (shoppingCartItemSubTotalWithDiscountBase, _) = await TaxService.GetProductPriceAsync(product, subTotal);
                var shoppingCartItemSubTotalWithDiscount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemSubTotalWithDiscountBase, currentCurrency);
                cartItemModel.SubTotal = await PriceFormatter.FormatPriceAsync(shoppingCartItemSubTotalWithDiscount);
                cartItemModel.SubTotalValue = shoppingCartItemSubTotalWithDiscount;
                cartItemModel.MaximumDiscountedQty = maximumDiscountQty;

                //display an applied discount amount
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    (shoppingCartItemDiscountBase, _) = await TaxService.GetProductPriceAsync(product, shoppingCartItemDiscountBase);
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        var shoppingCartItemDiscount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemDiscountBase, currentCurrency);
                        cartItemModel.Discount = await PriceFormatter.FormatPriceAsync(shoppingCartItemDiscount);
                        cartItemModel.DiscountValue = shoppingCartItemDiscount;
                    }
                }
            }

            //picture
            if (ShoppingCartSettings.ShowProductImagesOnShoppingCart)
            {
                cartItemModel.Picture = await PrepareCartItemPictureModelAsync(sci,
                    MediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
            }

            //item warnings
            var itemWarnings = await ShoppingCartService.GetShoppingCartItemWarningsAsync(
                await WorkContext.GetCurrentCustomerAsync(),
                sci.ShoppingCartType,
                product,
                sci.StoreId,
                sci.AttributesXml,
                sci.CustomerEnteredPrice,
                sci.RentalStartDateUtc,
                sci.RentalEndDateUtc,
                sci.Quantity,
                false,
                sci.Id);
            foreach (var warning in itemWarnings)
                cartItemModel.Warnings.Add(warning);

            return cartItemModel;
        }

        /// <summary>
        /// Prepare the wishlist item model
        /// </summary>
        /// <param name="sci">Shopping cart item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart item model
        /// </returns>
        protected virtual async Task<WishlistModel.ShoppingCartItemModel> PrepareWishlistItemModelAsync(ShoppingCartItem sci)
        {
            if (sci == null)
                throw new ArgumentNullException(nameof(sci));

            var product = await ProductService.GetProductByIdAsync(sci.ProductId);

            var cartItemModel = new WishlistModel.ShoppingCartItemModel
            {
                Id = sci.Id,
                Sku = await ProductService.FormatSkuAsync(product, sci.AttributesXml),
                ProductId = product.Id,
                ProductName = await LocalizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await UrlRecordService.GetSeNameAsync(product),
                Quantity = sci.Quantity,
                AttributeInfo = await ProductAttributeFormatter.FormatAttributesAsync(product, sci.AttributesXml),
            };

            //allow editing?
            //1. setting enabled?
            //2. simple product?
            //3. has attribute or gift card?
            //4. visible individually?
            cartItemModel.AllowItemEditing = ShoppingCartSettings.AllowCartItemEditing &&
                                             product.ProductType == ProductType.SimpleProduct &&
                                             (!string.IsNullOrEmpty(cartItemModel.AttributeInfo) ||
                                              product.IsGiftCard) &&
                                             product.VisibleIndividually;

            //allowed quantities
            var allowedQuantities = ProductService.ParseAllowedQuantities(product);
            foreach (var qty in allowedQuantities)
            {
                cartItemModel.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = sci.Quantity == qty
                });
            }

            //recurring info
            if (product.IsRecurring)
                cartItemModel.RecurringInfo = string.Format(await LocalizationService.GetResourceAsync("ShoppingCart.RecurringPeriod"),
                        product.RecurringCycleLength, await LocalizationService.GetLocalizedEnumAsync(product.RecurringCyclePeriod));

            //rental info
            if (product.IsRental)
            {
                var rentalStartDate = sci.RentalStartDateUtc.HasValue
                    ? ProductService.FormatRentalDate(product, sci.RentalStartDateUtc.Value)
                    : string.Empty;
                var rentalEndDate = sci.RentalEndDateUtc.HasValue
                    ? ProductService.FormatRentalDate(product, sci.RentalEndDateUtc.Value)
                    : string.Empty;
                cartItemModel.RentalInfo =
                    string.Format(await LocalizationService.GetResourceAsync("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
            }

            //unit prices
            var currentCurrency = await WorkContext.GetWorkingCurrencyAsync();
            if (product.CallForPrice &&
                //also check whether the current user is impersonated
                (!OrderSettings.AllowAdminsToBuyCallForPriceProducts || WorkContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.UnitPrice = await LocalizationService.GetResourceAsync("Products.CallForPrice");
                cartItemModel.UnitPriceValue = 0;
            }
            else
            {
                var (shoppingCartUnitPriceWithDiscountBase, _) = await TaxService.GetProductPriceAsync(product, (await ShoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice);
                var shoppingCartUnitPriceWithDiscount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartUnitPriceWithDiscountBase, currentCurrency);
                cartItemModel.UnitPrice = await PriceFormatter.FormatPriceAsync(shoppingCartUnitPriceWithDiscount);
                cartItemModel.UnitPriceValue = shoppingCartUnitPriceWithDiscount;
            }
            //subtotal, discount
            if (product.CallForPrice &&
                //also check whether the current user is impersonated
                (!OrderSettings.AllowAdminsToBuyCallForPriceProducts || WorkContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.SubTotal = await LocalizationService.GetResourceAsync("Products.CallForPrice");
                cartItemModel.SubTotalValue = 0;
            }
            else
            {
                //sub total
                var (subTotal, shoppingCartItemDiscountBase, _, maximumDiscountQty) = await ShoppingCartService.GetSubTotalAsync(sci, true);
                var (shoppingCartItemSubTotalWithDiscountBase, _) = await TaxService.GetProductPriceAsync(product, subTotal);
                var shoppingCartItemSubTotalWithDiscount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemSubTotalWithDiscountBase, currentCurrency);
                cartItemModel.SubTotal = await PriceFormatter.FormatPriceAsync(shoppingCartItemSubTotalWithDiscount);
                cartItemModel.SubTotalValue = shoppingCartItemSubTotalWithDiscount;
                cartItemModel.MaximumDiscountedQty = maximumDiscountQty;

                //display an applied discount amount
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    (shoppingCartItemDiscountBase, _) = await TaxService.GetProductPriceAsync(product, shoppingCartItemDiscountBase);
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        var shoppingCartItemDiscount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemDiscountBase, currentCurrency);
                        cartItemModel.Discount = await PriceFormatter.FormatPriceAsync(shoppingCartItemDiscount);
                        cartItemModel.DiscountValue = shoppingCartItemDiscount;
                    }
                }
            }

            //picture
            if (ShoppingCartSettings.ShowProductImagesOnWishList)
            {
                cartItemModel.Picture = await PrepareCartItemPictureModelAsync(sci,
                    MediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
            }

            //item warnings
            var itemWarnings = await ShoppingCartService.GetShoppingCartItemWarningsAsync(
                await WorkContext.GetCurrentCustomerAsync(),
                sci.ShoppingCartType,
                product,
                sci.StoreId,
                sci.AttributesXml,
                sci.CustomerEnteredPrice,
                sci.RentalStartDateUtc,
                sci.RentalEndDateUtc,
                sci.Quantity,
                false,
                sci.Id);
            foreach (var warning in itemWarnings)
                cartItemModel.Warnings.Add(warning);

            return cartItemModel;
        }

        /// <summary>
        /// Prepare the order review data model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order review data model
        /// </returns>
        protected virtual async Task<ShoppingCartModel.OrderReviewDataModel> PrepareOrderReviewDataModelAsync(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new ShoppingCartModel.OrderReviewDataModel
            {
                Display = true
            };

            //billing info
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var billingAddress = await CustomerService.GetCustomerBillingAddressAsync(customer);
            if (billingAddress != null)
            {
                await AddressModelFactory.PrepareAddressModelAsync(model.BillingAddress,
                        address: billingAddress,
                        excludeProperties: false,
                        addressSettings: AddressSettings);
            }

            //shipping info
            if (await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            {
                model.IsShippable = true;

                var pickupPoint = await GenericAttributeService.GetAttributeAsync<PickupPoint>(customer,
                    NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);
                model.SelectedPickupInStore = ShippingSettings.AllowPickupInStore && pickupPoint != null;
                if (!model.SelectedPickupInStore)
                {
                    if (await CustomerService.GetCustomerShippingAddressAsync(customer) is Address address)
                    {
                        await AddressModelFactory.PrepareAddressModelAsync(model.ShippingAddress,
                            address: address,
                            excludeProperties: false,
                            addressSettings: AddressSettings);
                    }
                }
                else
                {
                    var country = await CountryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode);
                    var state = await StateProvinceService.GetStateProvinceByAbbreviationAsync(pickupPoint.StateAbbreviation, country?.Id);

                    model.PickupAddress = new AddressModel
                    {
                        Address1 = pickupPoint.Address,
                        City = pickupPoint.City,
                        County = pickupPoint.County,
                        CountryName = country?.Name ?? string.Empty,
                        StateProvinceName = state?.Name ?? string.Empty,
                        ZipPostalCode = pickupPoint.ZipPostalCode
                    };
                }

                //selected shipping method
                var shippingOption = await GenericAttributeService.GetAttributeAsync<ShippingOption>(customer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
                if (shippingOption != null)
                    model.ShippingMethod = shippingOption.Name;
            }

            //payment info
            var selectedPaymentMethodSystemName = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
            var paymentMethod = await PaymentPluginManager
                .LoadPluginBySystemNameAsync(selectedPaymentMethodSystemName, customer, store.Id);
            model.PaymentMethod = paymentMethod != null
                ? await LocalizationService.GetLocalizedFriendlyNameAsync(paymentMethod, (await WorkContext.GetWorkingLanguageAsync()).Id)
                : string.Empty;

            //custom values
            var processPaymentRequest = HttpContextAccessor.HttpContext?.Session?.Get<ProcessPaymentRequest>("OrderPaymentInfo");
            if (processPaymentRequest != null)
                model.CustomValues = processPaymentRequest.CustomValues;

            return model;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Prepare the estimate shipping model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="setEstimateShippingDefaultAddress">Whether to use customer default shipping address for estimating</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the estimate shipping model
        /// </returns>
        public virtual async Task<EstimateShippingModel> PrepareEstimateShippingModelAsync(IList<ShoppingCartItem> cart, bool setEstimateShippingDefaultAddress = true)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new EstimateShippingModel
            {
                RequestDelay = ShippingSettings.RequestDelay,
                UseCity = ShippingSettings.EstimateShippingCityNameEnabled,
                Enabled = cart.Any() && await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart)
            };
            if (model.Enabled)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var shippingAddress = await CustomerService.GetCustomerShippingAddressAsync(customer);
                if (shippingAddress == null)
                {
                    shippingAddress = await (await CustomerService.GetAddressesByCustomerIdAsync(customer.Id))
                    //enabled for the current store
                    .FirstOrDefaultAwaitAsync(async a => a.CountryId == null || await StoreMappingService.AuthorizeAsync(await CountryService.GetCountryByAddressAsync(a)));
                }

                //countries
                var defaultEstimateCountryId = (setEstimateShippingDefaultAddress && shippingAddress != null)
                    ? shippingAddress.CountryId
                    : model.CountryId;
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = await LocalizationService.GetResourceAsync("Address.SelectCountry"),
                    Value = "0"
                });

                var currentLanguage = await WorkContext.GetWorkingLanguageAsync();
                foreach (var c in await CountryService.GetAllCountriesForShippingAsync(currentLanguage.Id))
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = await LocalizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == defaultEstimateCountryId
                    });

                //states
                var defaultEstimateStateId = (setEstimateShippingDefaultAddress && shippingAddress != null)
                    ? shippingAddress.StateProvinceId
                    : model.StateProvinceId;
                var states = defaultEstimateCountryId.HasValue
                    ? (await StateProvinceService.GetStateProvincesByCountryIdAsync(defaultEstimateCountryId.Value, currentLanguage.Id)).ToList()
                    : new List<StateProvince>();
                if (states.Any())
                {
                    foreach (var s in states)
                    {
                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = await LocalizationService.GetLocalizedAsync(s, x => x.Name),
                            Value = s.Id.ToString(),
                            Selected = s.Id == defaultEstimateStateId
                        });
                    }
                }
                else
                {
                    model.AvailableStates.Add(new SelectListItem
                    {
                        Text = await LocalizationService.GetResourceAsync("Address.Other"),
                        Value = "0"
                    });
                }

                if (setEstimateShippingDefaultAddress && shippingAddress != null)
                {
                    if (!ShippingSettings.EstimateShippingCityNameEnabled)
                        model.ZipPostalCode = shippingAddress.ZipPostalCode;
                    else
                        model.City = shippingAddress.City;
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the shopping cart model
        /// </summary>
        /// <param name="model">Shopping cart model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <param name="validateCheckoutAttributes">Whether to validate checkout attributes</param>
        /// <param name="prepareAndDisplayOrderReviewData">Whether to prepare and display order review data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart model
        /// </returns>
        public virtual async Task<ShoppingCartModel> PrepareShoppingCartModelAsync(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //simple properties
            model.OnePageCheckoutEnabled = OrderSettings.OnePageCheckoutEnabled;

            if (!cart.Any())
                return model;

            model.IsEditable = isEditable;
            model.ShowProductImages = ShoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = CatalogSettings.ShowSkuOnProductDetailsPage;
            model.ShowVendorName = VendorSettings.ShowVendorOnOrderDetailsPage;
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var checkoutAttributesXml = await GenericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.CheckoutAttributes, store.Id);
            var minOrderSubtotalAmountOk = await OrderProcessingService.ValidateMinOrderSubtotalAmountAsync(cart);
            if (!minOrderSubtotalAmountOk)
            {
                var minOrderSubtotalAmount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(OrderSettings.MinOrderSubtotalAmount, await WorkContext.GetWorkingCurrencyAsync());
                model.MinOrderSubtotalWarning = string.Format(await LocalizationService.GetResourceAsync("Checkout.MinOrderSubtotalAmount"), await PriceFormatter.FormatPriceAsync(minOrderSubtotalAmount, true, false));
            }

            model.TermsOfServiceOnShoppingCartPage = OrderSettings.TermsOfServiceOnShoppingCartPage;
            model.TermsOfServiceOnOrderConfirmPage = OrderSettings.TermsOfServiceOnOrderConfirmPage;
            model.TermsOfServicePopup = CommonSettings.PopupForTermsOfServiceLinks;
            model.DisplayTaxShippingInfo = CatalogSettings.DisplayTaxShippingInfoShoppingCart;

            //discount and gift card boxes
            model.DiscountBox.Display = ShoppingCartSettings.ShowDiscountBox;
            var discountCouponCodes = await CustomerService.ParseAppliedDiscountCouponCodesAsync(customer);
            foreach (var couponCode in discountCouponCodes)
            {
                var discount = await (await DiscountService.GetAllDiscountsAsync(couponCode: couponCode))
                    .FirstOrDefaultAwaitAsync(async d => d.RequiresCouponCode && (await DiscountService.ValidateDiscountAsync(d, customer)).IsValid);

                if (discount != null)
                {
                    model.DiscountBox.AppliedDiscountsWithCodes.Add(new ShoppingCartModel.DiscountBoxModel.DiscountInfoModel
                    {
                        Id = discount.Id,
                        CouponCode = discount.CouponCode
                    });
                }
            }

            model.GiftCardBox.Display = ShoppingCartSettings.ShowGiftCardBox;

            //cart warnings
            var cartWarnings = await ShoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributesXml, validateCheckoutAttributes);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            //checkout attributes
            model.CheckoutAttributes = await PrepareCheckoutAttributeModelsAsync(cart);

            //cart items
            foreach (var sci in cart)
            {
                var cartItemModel = await PrepareShoppingCartItemModelAsync(cart, sci);
                model.Items.Add(cartItemModel);
            }

            //payment methods
            //all payment methods (do not filter by country here as it could be not specified yet)
            var paymentMethods = await (await PaymentPluginManager
                .LoadActivePluginsAsync(customer, store.Id))
                .WhereAwait(async pm => !await pm.HidePaymentMethodAsync(cart)).ToListAsync();
            //payment methods displayed during checkout (not with "Button" type)
            var nonButtonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType != PaymentMethodType.Button)
                .ToList();
            //"button" payment methods(*displayed on the shopping cart page)
            var buttonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .ToList();
            foreach (var pm in buttonPaymentMethods)
            {
                if (await ShoppingCartService.ShoppingCartIsRecurringAsync(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var viewComponentName = pm.GetPublicViewComponentName();
                model.ButtonPaymentMethodViewComponentNames.Add(viewComponentName);
            }
            //hide "Checkout" button if we have only "Button" payment methods
            model.HideCheckoutButton = !nonButtonPaymentMethods.Any() && model.ButtonPaymentMethodViewComponentNames.Any();

            //order review data
            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData = await PrepareOrderReviewDataModelAsync(cart);
            }

            return model;
        }

        /// <summary>
        /// Prepare the wishlist model
        /// </summary>
        /// <param name="model">Wishlist model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the wishlist model
        /// </returns>
        public virtual async Task<WishlistModel> PrepareWishlistModelAsync(WishlistModel model, IList<ShoppingCartItem> cart, bool isEditable = true)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.EmailWishlistEnabled = ShoppingCartSettings.EmailWishlistEnabled;
            model.IsEditable = isEditable;
            model.DisplayAddToCart = await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart);
            model.DisplayTaxShippingInfo = CatalogSettings.DisplayTaxShippingInfoWishlist;

            if (!cart.Any())
                return model;

            //simple properties
            var customer = await CustomerService.GetShoppingCartCustomerAsync(cart);

            model.CustomerGuid = customer.CustomerGuid;
            model.CustomerFullname = await CustomerService.GetCustomerFullNameAsync(customer);
            model.ShowProductImages = ShoppingCartSettings.ShowProductImagesOnWishList;
            model.ShowSku = CatalogSettings.ShowSkuOnProductDetailsPage;

            //cart warnings
            var cartWarnings = await ShoppingCartService.GetShoppingCartWarningsAsync(cart, string.Empty, false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            //cart items
            foreach (var sci in cart)
            {
                var cartItemModel = await PrepareWishlistItemModelAsync(sci);
                model.Items.Add(cartItemModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the mini shopping cart model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the mini shopping cart model
        /// </returns>
        public virtual async Task<MiniShoppingCartModel> PrepareMiniShoppingCartModelAsync()
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var model = new MiniShoppingCartModel
            {
                ShowProductImages = ShoppingCartSettings.ShowProductImagesInMiniShoppingCart,
                //let's always display it
                DisplayShoppingCartButton = true,
                CurrentCustomerIsGuest = await CustomerService.IsGuestAsync(customer),
                AnonymousCheckoutAllowed = OrderSettings.AnonymousCheckoutAllowed,
            };

            //performance optimization (use "HasShoppingCartItems" property)
            if (customer.HasShoppingCartItems)
            {
                var store = await StoreContext.GetCurrentStoreAsync();
                var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                if (cart.Any())
                {
                    model.TotalProducts = cart.Sum(item => item.Quantity);

                    //subtotal
                    var subTotalIncludingTax = await WorkContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax && !TaxSettings.ForceTaxExclusionFromOrderSubtotal;
                    var (_, _, _, subTotalWithoutDiscountBase, _) = await OrderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, subTotalIncludingTax);
                    var subtotalBase = subTotalWithoutDiscountBase;
                    var currentCurrency = await WorkContext.GetWorkingCurrencyAsync();
                    var subtotal = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(subtotalBase, currentCurrency);
                    model.SubTotal = await PriceFormatter.FormatPriceAsync(subtotal, false, currentCurrency, (await WorkContext.GetWorkingLanguageAsync()).Id, subTotalIncludingTax);
                    model.SubTotalValue = subtotal;

                    var requiresShipping = await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart);
                    //a customer should visit the shopping cart page (hide checkout button) before going to checkout if:
                    //1. "terms of service" are enabled
                    //2. min order sub-total is OK
                    //3. we have at least one checkout attribute
                    var checkoutAttributesExist = (await CheckoutAttributeService
                        .GetAllCheckoutAttributesAsync(store.Id, !requiresShipping))
                        .Any();

                    var minOrderSubtotalAmountOk = await OrderProcessingService.ValidateMinOrderSubtotalAmountAsync(cart);

                    var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();

                    var downloadableProductsRequireRegistration =
                        CustomerSettings.RequireRegistrationForDownloadableProducts && await ProductService.HasAnyDownloadableProductAsync(cartProductIds);

                    model.DisplayCheckoutButton = !OrderSettings.TermsOfServiceOnShoppingCartPage &&
                        minOrderSubtotalAmountOk &&
                        !checkoutAttributesExist &&
                        !(downloadableProductsRequireRegistration
                            && await CustomerService.IsGuestAsync(customer));

                    //products. sort descending (recently added products)
                    foreach (var sci in cart
                        .OrderByDescending(x => x.Id)
                        .Take(ShoppingCartSettings.MiniShoppingCartProductNumber)
                        .ToList())
                    {
                        var product = await ProductService.GetProductByIdAsync(sci.ProductId);

                        var cartItemModel = new MiniShoppingCartModel.ShoppingCartItemModel
                        {
                            Id = sci.Id,
                            ProductId = sci.ProductId,
                            ProductName = await LocalizationService.GetLocalizedAsync(product, x => x.Name),
                            ProductSeName = await UrlRecordService.GetSeNameAsync(product),
                            Quantity = sci.Quantity,
                            AttributeInfo = await ProductAttributeFormatter.FormatAttributesAsync(product, sci.AttributesXml)
                        };

                        //unit prices
                        if (product.CallForPrice &&
                            //also check whether the current user is impersonated
                            (!OrderSettings.AllowAdminsToBuyCallForPriceProducts || WorkContext.OriginalCustomerIfImpersonated == null))
                        {
                            cartItemModel.UnitPrice = await LocalizationService.GetResourceAsync("Products.CallForPrice");
                            cartItemModel.UnitPriceValue = 0;
                        }
                        else
                        {
                            var (shoppingCartUnitPriceWithDiscountBase, _) = await TaxService.GetProductPriceAsync(product, (await ShoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice);
                            var shoppingCartUnitPriceWithDiscount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartUnitPriceWithDiscountBase, currentCurrency);
                            cartItemModel.UnitPrice = await PriceFormatter.FormatPriceAsync(shoppingCartUnitPriceWithDiscount);
                            cartItemModel.UnitPriceValue = shoppingCartUnitPriceWithDiscount;
                        }

                        //picture
                        if (ShoppingCartSettings.ShowProductImagesInMiniShoppingCart)
                        {
                            cartItemModel.Picture = await PrepareCartItemPictureModelAsync(sci,
                                MediaSettings.MiniCartThumbPictureSize, true, cartItemModel.ProductName);
                        }

                        model.Items.Add(cartItemModel);
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare selected checkout attributes
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the formatted attributes
        /// </returns>
        public virtual async Task<string> FormatSelectedCheckoutAttributesAsync()
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var checkoutAttributesXml = await GenericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.CheckoutAttributes, store.Id);

            return await CheckoutAttributeFormatter.FormatAttributesAsync(checkoutAttributesXml, customer);
        }

        /// <summary>
        /// Prepare the order totals model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order totals model
        /// </returns>
        public virtual async Task<OrderTotalsModel> PrepareOrderTotalsModelAsync(IList<ShoppingCartItem> cart, bool isEditable)
        {
            var model = new OrderTotalsModel
            {
                IsEditable = isEditable
            };

            if (cart.Any())
            {
                //subtotal
                var subTotalIncludingTax = await WorkContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax && !TaxSettings.ForceTaxExclusionFromOrderSubtotal;
                var (orderSubTotalDiscountAmountBase, _, subTotalWithoutDiscountBase, _, _) = await OrderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, subTotalIncludingTax);
                var subtotalBase = subTotalWithoutDiscountBase;
                var currentCurrency = await WorkContext.GetWorkingCurrencyAsync();
                var subtotal = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(subtotalBase, currentCurrency);
                var currentLanguage = await WorkContext.GetWorkingLanguageAsync();
                model.SubTotal = await PriceFormatter.FormatPriceAsync(subtotal, true, currentCurrency, currentLanguage.Id, subTotalIncludingTax);

                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    var orderSubTotalDiscountAmount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(orderSubTotalDiscountAmountBase, currentCurrency);
                    model.SubTotalDiscount = await PriceFormatter.FormatPriceAsync(-orderSubTotalDiscountAmount, true, currentCurrency, currentLanguage.Id, subTotalIncludingTax);
                }

                //shipping info
                model.RequiresShipping = await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart);
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var store = await StoreContext.GetCurrentStoreAsync();
                if (model.RequiresShipping)
                {
                    var shoppingCartShippingBase = await OrderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart);
                    if (shoppingCartShippingBase.HasValue)
                    {
                        var shoppingCartShipping = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartShippingBase.Value, currentCurrency);
                        model.Shipping = await PriceFormatter.FormatShippingPriceAsync(shoppingCartShipping, true);

                        //selected shipping method
                        var shippingOption = await GenericAttributeService.GetAttributeAsync<ShippingOption>(customer,
                            NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
                        if (shippingOption != null)
                            model.SelectedShippingMethod = shippingOption.Name;
                    }
                }
                else
                {
                    model.HideShippingTotal = ShippingSettings.HideShippingTotal;
                }

                //payment method fee
                var paymentMethodSystemName = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
                var paymentMethodAdditionalFee = await PaymentService.GetAdditionalHandlingFeeAsync(cart, paymentMethodSystemName);
                var (paymentMethodAdditionalFeeWithTaxBase, _) = await TaxService.GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, customer);
                if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                {
                    var paymentMethodAdditionalFeeWithTax = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(paymentMethodAdditionalFeeWithTaxBase, currentCurrency);
                    model.PaymentMethodAdditionalFee = await PriceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeWithTax, true);
                }

                //tax
                bool displayTax;
                bool displayTaxRates;
                if (TaxSettings.HideTaxInOrderSummary && await WorkContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    var (shoppingCartTaxBase, taxRates) = await OrderTotalCalculationService.GetTaxTotalAsync(cart);
                    var shoppingCartTax = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartTaxBase, currentCurrency);

                    if (shoppingCartTaxBase == 0 && TaxSettings.HideZeroTax)
                    {
                        displayTax = false;
                        displayTaxRates = false;
                    }
                    else
                    {
                        displayTaxRates = TaxSettings.DisplayTaxRates && taxRates.Any();
                        displayTax = !displayTaxRates;

                        model.Tax = await PriceFormatter.FormatPriceAsync(shoppingCartTax, true, false);
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new OrderTotalsModel.TaxRate
                            {
                                Rate = PriceFormatter.FormatTaxRate(tr.Key),
                                Value = await PriceFormatter.FormatPriceAsync(await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(tr.Value, currentCurrency), true, false),
                            });
                        }
                    }
                }

                model.DisplayTaxRates = displayTaxRates;
                model.DisplayTax = displayTax;

                //total
                var (shoppingCartTotalBase, orderTotalDiscountAmountBase, _, appliedGiftCards, redeemedRewardPoints, redeemedRewardPointsAmount) = await OrderTotalCalculationService.GetShoppingCartTotalAsync(cart);
                if (shoppingCartTotalBase.HasValue)
                {
                    var shoppingCartTotal = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartTotalBase.Value, currentCurrency);
                    model.OrderTotal = await PriceFormatter.FormatPriceAsync(shoppingCartTotal, true, false);
                }

                //discount
                if (orderTotalDiscountAmountBase > decimal.Zero)
                {
                    var orderTotalDiscountAmount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(orderTotalDiscountAmountBase, currentCurrency);
                    model.OrderTotalDiscount = await PriceFormatter.FormatPriceAsync(-orderTotalDiscountAmount, true, false);
                }

                //gift cards
                if (appliedGiftCards != null && appliedGiftCards.Any())
                {
                    foreach (var appliedGiftCard in appliedGiftCards)
                    {
                        var gcModel = new OrderTotalsModel.GiftCard
                        {
                            Id = appliedGiftCard.GiftCard.Id,
                            CouponCode = appliedGiftCard.GiftCard.GiftCardCouponCode,
                        };
                        var amountCanBeUsed = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(appliedGiftCard.AmountCanBeUsed, currentCurrency);
                        gcModel.Amount = await PriceFormatter.FormatPriceAsync(-amountCanBeUsed, true, false);

                        var remainingAmountBase = await GiftCardService.GetGiftCardRemainingAmountAsync(appliedGiftCard.GiftCard) - appliedGiftCard.AmountCanBeUsed;
                        var remainingAmount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(remainingAmountBase, currentCurrency);
                        gcModel.Remaining = await PriceFormatter.FormatPriceAsync(remainingAmount, true, false);

                        model.GiftCards.Add(gcModel);
                    }
                }

                //reward points to be spent (redeemed)
                if (redeemedRewardPointsAmount > decimal.Zero)
                {
                    var redeemedRewardPointsAmountInCustomerCurrency = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(redeemedRewardPointsAmount, currentCurrency);
                    model.RedeemedRewardPoints = redeemedRewardPoints;
                    model.RedeemedRewardPointsAmount = await PriceFormatter.FormatPriceAsync(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
                }

                //reward points to be earned
                if (RewardPointsSettings.Enabled && RewardPointsSettings.DisplayHowMuchWillBeEarned && shoppingCartTotalBase.HasValue)
                {
                    //get shipping total
                    var shippingBaseInclTax = !model.RequiresShipping ? 0 : (await OrderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart, true)).shippingTotal ?? 0;

                    //get total for reward points
                    var totalForRewardPoints = OrderTotalCalculationService
                        .CalculateApplicableOrderTotalForRewardPoints(shippingBaseInclTax, shoppingCartTotalBase.Value);
                    if (totalForRewardPoints > decimal.Zero)
                        model.WillEarnRewardPoints = await OrderTotalCalculationService.CalculateRewardPointsAsync(customer, totalForRewardPoints);
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the estimate shipping result model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="request">Request to get shipping options</param>
        /// <param name="cacheShippingOptions">Indicates whether to cache offered shipping options</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the estimate shipping result model
        /// </returns>
        public virtual async Task<EstimateShippingResultModel> PrepareEstimateShippingResultModelAsync(IList<ShoppingCartItem> cart, EstimateShippingModel request, bool cacheShippingOptions)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var model = new EstimateShippingResultModel();

            if (await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            {
                var address = new Address
                {
                    CountryId = request.CountryId,
                    StateProvinceId = request.StateProvinceId,
                    ZipPostalCode = request.ZipPostalCode,
                    City = request.City
                };

                var rawShippingOptions = new List<ShippingOption>();
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var store = await StoreContext.GetCurrentStoreAsync();
                var getShippingOptionResponse = await ShippingService.GetShippingOptionsAsync(cart, address, customer, storeId: store.Id);
                if (getShippingOptionResponse.Success)
                {
                    if (getShippingOptionResponse.ShippingOptions.Any())
                    {
                        foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                        {
                            rawShippingOptions.Add(new ShippingOption
                            {
                                Name = shippingOption.Name,
                                Description = shippingOption.Description,
                                Rate = shippingOption.Rate,
                                TransitDays = shippingOption.TransitDays,
                                ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName
                            });
                        }
                    }
                    else
                    {
                        foreach (var error in getShippingOptionResponse.Errors)
                            model.Errors.Add(error);
                    }
                }

                var pickupPointsNumber = 0;
                if (ShippingSettings.AllowPickupInStore)
                {
                    var pickupPointsResponse = await ShippingService.GetPickupPointsAsync(address.Id, customer, storeId: store.Id);
                    if (pickupPointsResponse.Success)
                    {
                        if (pickupPointsResponse.PickupPoints.Any())
                        {
                            pickupPointsNumber = pickupPointsResponse.PickupPoints.Count();
                            var pickupPoint = pickupPointsResponse.PickupPoints.OrderBy(p => p.PickupFee).First();

                            rawShippingOptions.Add(new ShippingOption
                            {
                                Name = await LocalizationService.GetResourceAsync("Checkout.PickupPoints"),
                                Description = await LocalizationService.GetResourceAsync("Checkout.PickupPoints.Description"),
                                Rate = pickupPoint.PickupFee,
                                TransitDays = pickupPoint.TransitDays,
                                ShippingRateComputationMethodSystemName = pickupPoint.ProviderSystemName,
                                IsPickupInStore = true
                            });
                        }
                    }
                    else
                    {
                        foreach (var error in pickupPointsResponse.Errors)
                            model.Errors.Add(error);
                    }
                }

                ShippingOption selectedShippingOption = null;
                if (cacheShippingOptions)
                {
                    //performance optimization. cache returned shipping options.
                    //we'll use them later (after a customer has selected an option).
                    await GenericAttributeService.SaveAttributeAsync(customer,
                                                           NopCustomerDefaults.OfferedShippingOptionsAttribute,
                                                           rawShippingOptions,
                                                           store.Id);

                    //find a selected (previously) shipping option
                    selectedShippingOption = await GenericAttributeService.GetAttributeAsync<ShippingOption>(customer,
                            NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
                }

                if (rawShippingOptions.Any())
                {
                    foreach (var option in rawShippingOptions)
                    {
                        var (shippingRate, _) = await OrderTotalCalculationService.AdjustShippingRateAsync(option.Rate, cart, option.IsPickupInStore);
                        (shippingRate, _) = await TaxService.GetShippingPriceAsync(shippingRate, customer);
                        shippingRate = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(shippingRate, await WorkContext.GetWorkingCurrencyAsync());
                        var shippingRateString = await PriceFormatter.FormatShippingPriceAsync(shippingRate, true);

                        if (option.IsPickupInStore && pickupPointsNumber > 1)
                            shippingRateString = string.Format(await LocalizationService.GetResourceAsync("Shipping.EstimateShippingPopUp.Pickup.PriceFrom"), shippingRateString);

                        string deliveryDateFormat = null;
                        if (option.TransitDays.HasValue)
                        {
                            var currentCulture = CultureInfo.GetCultureInfo((await WorkContext.GetWorkingLanguageAsync()).LanguageCulture);
                            var customerDateTime = await DateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
                            deliveryDateFormat = customerDateTime.AddDays(option.TransitDays.Value).ToString("d", currentCulture);
                        }

                        var selected = selectedShippingOption != null &&
                                        !string.IsNullOrEmpty(option.ShippingRateComputationMethodSystemName) &&
                                        option.ShippingRateComputationMethodSystemName.Equals(selectedShippingOption.ShippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase) &&
                                        (!string.IsNullOrEmpty(option.Name) &&
                                         option.Name.Equals(selectedShippingOption.Name, StringComparison.InvariantCultureIgnoreCase) ||
                                         (option.IsPickupInStore && option.IsPickupInStore == selectedShippingOption.IsPickupInStore));

                        model.ShippingOptions.Add(new EstimateShippingResultModel.ShippingOptionModel
                        {
                            Name = option.Name,
                            ShippingRateComputationMethodSystemName = option.ShippingRateComputationMethodSystemName,
                            Description = option.Description,
                            Price = shippingRateString,
                            Rate = option.Rate,
                            DeliveryDateFormat = deliveryDateFormat,
                            Selected = selected
                        });
                    }

                    //if no option has been selected, let's do it for the first one
                    if (!model.ShippingOptions.Any(so => so.Selected))
                        model.ShippingOptions.First().Selected = true;
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the wishlist email a friend model
        /// </summary>
        /// <param name="model">Wishlist email a friend model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the wishlist email a friend model
        /// </returns>
        public virtual async Task<WishlistEmailAFriendModel> PrepareWishlistEmailAFriendModelAsync(WishlistEmailAFriendModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnEmailWishlistToFriendPage;
            if (!excludeProperties)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                model.YourEmailAddress = customer.Email;
            }

            return model;
        }

        /// <summary>
        /// Prepare the cart item picture model
        /// </summary>
        /// <param name="sci">Shopping cart item</param>
        /// <param name="pictureSize">Picture size</param>
        /// <param name="showDefaultPicture">Whether to show the default picture</param>
        /// <param name="productName">Product name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture model
        /// </returns>
        public virtual async Task<PictureModel> PrepareCartItemPictureModelAsync(ShoppingCartItem sci, int pictureSize, bool showDefaultPicture, string productName)
        {
            var pictureCacheKey = StaticCacheManager.PrepareKeyForShortTermCache(NopModelCacheDefaults.CartPictureModelKey
                , sci, pictureSize, true, await WorkContext.GetWorkingLanguageAsync(), WebHelper.IsCurrentConnectionSecured(), await StoreContext.GetCurrentStoreAsync());

            var model = await StaticCacheManager.GetAsync(pictureCacheKey, async () =>
            {
                var product = await ProductService.GetProductByIdAsync(sci.ProductId);

                //shopping cart item picture
                var sciPicture = await PictureService.GetProductPictureAsync(product, sci.AttributesXml);

                return new PictureModel
                {
                    ImageUrl = (await PictureService.GetPictureUrlAsync(sciPicture, pictureSize, showDefaultPicture)).Url,
                    Title = string.Format(await LocalizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat"), productName),
                    AlternateText = string.Format(await LocalizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat"), productName),
                };
            });

            return model;
        }

        #endregion
    }
}