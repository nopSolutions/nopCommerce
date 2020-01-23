using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Tax.Avalara.Domain;
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
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.Tax.Avalara.Factories
{
    /// <summary>
    /// Represents overridden shopping cart model factory
    /// </summary>
    public class OverriddenShoppingCartModelFactory : ShoppingCartModelFactory
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentService _paymentService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly ITaxService _taxService;
        private readonly IWorkContext _workContext;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public OverriddenShoppingCartModelFactory(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            IAddressService addressService,
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
            IShippingPluginManager shippingPluginManager,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            ITaxPluginManager taxPluginManager,
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
            VendorSettings vendorSettings) : base(addressSettings,
                captchaSettings,
                catalogSettings,
                commonSettings,
                customerSettings,
                addressModelFactory,
                checkoutAttributeFormatter,
                checkoutAttributeParser,
                checkoutAttributeService,
                countryService,
                currencyService,
                customerService,
                dateTimeHelper,
                discountService,
                downloadService,
                genericAttributeService,
                giftCardService,
                httpContextAccessor,
                localizationService,
                orderProcessingService,
                orderTotalCalculationService,
                paymentPluginManager,
                paymentService,
                permissionService,
                pictureService,
                priceFormatter,
                productAttributeFormatter,
                productService,
                shippingPluginManager,
                shippingService,
                shoppingCartService,
                stateProvinceService,
                cacheManager,
                storeContext,
                taxService,
                urlRecordService,
                vendorService,
                webHelper,
                workContext,
                mediaSettings,
                orderSettings,
                rewardPointsSettings,
                shippingSettings,
                shoppingCartSettings,
                taxSettings,
                vendorSettings)
        {
            _addressService = addressService;
            _countryService = countryService;
            _currencyService = currencyService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _httpContextAccessor = httpContextAccessor;
            _orderTotalCalculationService = orderTotalCalculationService;
            _paymentService = paymentService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _shippingPluginManager = shippingPluginManager;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxPluginManager = taxPluginManager;
            _taxService = taxService;
            _workContext = workContext;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare tax details by Avalara tax service
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        private void PrepareTaxDetails(IList<ShoppingCartItem> cart)
        {
            //ensure that Avalara tax provider is active
            var taxProvider = _taxPluginManager
                .LoadPluginBySystemName(AvalaraTaxDefaults.SystemName, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id)
                as AvalaraTaxProvider;
            if (!_taxPluginManager.IsPluginActive(taxProvider))
                return;

            //create dummy order for the tax request
            var order = new Order { CustomerId = _workContext.CurrentCustomer.Id };

            //addresses
            order.BillingAddressId = _workContext.CurrentCustomer.BillingAddressId ?? 0;
            order.ShippingAddressId = _workContext.CurrentCustomer.ShippingAddressId;

            if (_shippingSettings.AllowPickupInStore)
            {
                var pickupPoint = _genericAttributeService.GetAttribute<PickupPoint>(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedPickupPointAttribute, _storeContext.CurrentStore.Id);
                if (pickupPoint != null)
                {
                    var country = _countryService.GetCountryByTwoLetterIsoCode(pickupPoint.CountryCode);

                    var pickupAddress = new Address
                    {
                        Address1 = pickupPoint.Address,
                        City = pickupPoint.City,
                        CountryId = country?.Id,
                        StateProvinceId = _stateProvinceService.GetStateProvinceByAbbreviation(pickupPoint.StateAbbreviation, country?.Id)?.Id,
                        ZipPostalCode = pickupPoint.ZipPostalCode,
                        CreatedOnUtc = DateTime.UtcNow,
                    };

                    _addressService.InsertAddress(pickupAddress);

                    order.PickupAddressId = pickupAddress.Id;
                }
            }

            //checkout attributes
            order.CheckoutAttributesXml = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);

            //shipping method
            var shippingRateComputationMethods = _shippingPluginManager.LoadActivePlugins(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
            order.OrderShippingExclTax = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart, false, shippingRateComputationMethods) ?? 0;
            order.ShippingMethod = _genericAttributeService.GetAttribute<ShippingOption>(_workContext.CurrentCustomer,
                NopCustomerDefaults.SelectedShippingOptionAttribute, _storeContext.CurrentStore.Id)?.Name;

            //payment method
            var paymentMethod = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
            var paymentFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethod);
            order.PaymentMethodAdditionalFeeExclTax = _taxService.GetPaymentMethodAdditionalFee(paymentFee, false, _workContext.CurrentCustomer);
            order.PaymentMethodSystemName = paymentMethod;

            //add discount amount
            _orderTotalCalculationService.GetShoppingCartSubTotal(cart, false, out var orderSubTotalDiscountExclTax, out _, out _, out _);
            order.OrderSubTotalDiscountExclTax = orderSubTotalDiscountExclTax;

            var orderItems = new List<OrderItem>();

            //create dummy order items
            foreach (var cartItem in cart)
            {
                var orderItem = new OrderItem
                {
                    AttributesXml = cartItem.AttributesXml,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity
                };

                var product = _productService.GetProductById(cartItem.ProductId);

                var itemSubtotal = _shoppingCartService.GetSubTotal(cartItem, true, out _, out _, out _);
                orderItem.PriceExclTax = _taxService.GetProductPrice(product, itemSubtotal, false, _workContext.CurrentCustomer, out _);

                orderItems.Add(orderItem);
            }

            //get tax details
            var taxTransaction = taxProvider.CreateOrderTaxTransaction(order, orderItems, false);
            if (taxTransaction == null)
                return;

            //and save it for the further usage
            var taxDetails = new TaxDetails { TaxTotal = taxTransaction.totalTax };
            foreach (var item in taxTransaction.summary)
            {
                if (!item.rate.HasValue || !item.tax.HasValue)
                    continue;

                var taxRate = item.rate.Value * 100;
                var taxValue = item.tax.Value;

                if (!taxDetails.TaxRates.ContainsKey(taxRate))
                    taxDetails.TaxRates.Add(taxRate, taxValue);
                else
                    taxDetails.TaxRates[taxRate] = taxDetails.TaxRates[taxRate] + taxValue;
            }
            _httpContextAccessor.HttpContext.Session.Set(AvalaraTaxDefaults.TaxDetailsSessionValue, taxDetails);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the order totals model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>Order totals model</returns>
        public override OrderTotalsModel PrepareOrderTotalsModel(IList<ShoppingCartItem> cart, bool isEditable)
        {
            var model = new OrderTotalsModel
            {
                IsEditable = isEditable
            };

            if (cart.Any())
            {
                //Avalara plugin changes
                PrepareTaxDetails(cart);
                //Avalara plugin changes

                //subtotal
                var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax, out var orderSubTotalDiscountAmountBase, out var _, out var subTotalWithoutDiscountBase, out var _);
                var subtotalBase = subTotalWithoutDiscountBase;
                var subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);

                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    var orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);
                }

                //LoadAllShippingRateComputationMethods
                var shippingRateComputationMethods = _shippingPluginManager.LoadActivePlugins(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);

                //shipping info
                model.RequiresShipping = _shoppingCartService.ShoppingCartRequiresShipping(cart);
                if (model.RequiresShipping)
                {
                    var shoppingCartShippingBase = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart, shippingRateComputationMethods);
                    if (shoppingCartShippingBase.HasValue)
                    {
                        var shoppingCartShipping = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartShippingBase.Value, _workContext.WorkingCurrency);
                        model.Shipping = _priceFormatter.FormatShippingPrice(shoppingCartShipping, true);

                        //selected shipping method
                        var shippingOption = _genericAttributeService.GetAttribute<ShippingOption>(_workContext.CurrentCustomer,
                            NopCustomerDefaults.SelectedShippingOptionAttribute, _storeContext.CurrentStore.Id);
                        if (shippingOption != null)
                            model.SelectedShippingMethod = shippingOption.Name;
                    }
                }
                else
                {
                    model.HideShippingTotal = _shippingSettings.HideShippingTotal;
                }

                //payment method fee
                var paymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
                var paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                var paymentMethodAdditionalFeeWithTaxBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                {
                    var paymentMethodAdditionalFeeWithTax = _currencyService.ConvertFromPrimaryStoreCurrency(paymentMethodAdditionalFeeWithTaxBase, _workContext.WorkingCurrency);
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeWithTax, true);
                }

                //tax
                var displayTax = true;
                var displayTaxRates = true;
                if (_taxSettings.HideTaxInOrderSummary && _workContext.TaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    var shoppingCartTaxBase = _orderTotalCalculationService.GetTaxTotal(cart, shippingRateComputationMethods, out var taxRates);

                    //Avalara plugin changes
                    //get tax details from the Avalara tax service, it may slightly differ from the original calculated tax
                    var taxDetails = _httpContextAccessor.HttpContext.Session.Get<TaxDetails>(AvalaraTaxDefaults.TaxDetailsSessionValue);
                    if (taxDetails != null)
                    {
                        //adjust tax total according to received value from the Avalara
                        if (taxDetails.TaxTotal.HasValue)
                            shoppingCartTaxBase = taxDetails.TaxTotal.Value;

                        if (taxDetails.TaxRates?.Any() ?? false)
                            taxRates = new SortedDictionary<decimal, decimal>(taxDetails.TaxRates);
                    }
                    //Avalara plugin changes

                    var shoppingCartTax = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTaxBase, _workContext.WorkingCurrency);

                    if (shoppingCartTaxBase == 0 && _taxSettings.HideZeroTax)
                    {
                        displayTax = false;
                        displayTaxRates = false;
                    }
                    else
                    {
                        displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                        displayTax = !displayTaxRates;

                        model.Tax = _priceFormatter.FormatPrice(shoppingCartTax, true, false);
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new OrderTotalsModel.TaxRate
                            {
                                Rate = _priceFormatter.FormatTaxRate(tr.Key),
                                Value = _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(tr.Value, _workContext.WorkingCurrency), true, false),
                            });
                        }
                    }
                }
                model.DisplayTaxRates = displayTaxRates;
                model.DisplayTax = displayTax;

                //total
                var shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart, out var orderTotalDiscountAmountBase, out var _, out var appliedGiftCards, out var redeemedRewardPoints, out var redeemedRewardPointsAmount);
                if (shoppingCartTotalBase.HasValue)
                {
                    var shoppingCartTotal = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTotalBase.Value, _workContext.WorkingCurrency);
                    model.OrderTotal = _priceFormatter.FormatPrice(shoppingCartTotal, true, false);
                }

                //discount
                if (orderTotalDiscountAmountBase > decimal.Zero)
                {
                    var orderTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderTotalDiscountAmount, true, false);
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
                        var amountCanBeUsed = _currencyService.ConvertFromPrimaryStoreCurrency(appliedGiftCard.AmountCanBeUsed, _workContext.WorkingCurrency);
                        gcModel.Amount = _priceFormatter.FormatPrice(-amountCanBeUsed, true, false);

                        var remainingAmountBase = _giftCardService.GetGiftCardRemainingAmount(appliedGiftCard.GiftCard) - appliedGiftCard.AmountCanBeUsed;
                        var remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(remainingAmountBase, _workContext.WorkingCurrency);
                        gcModel.Remaining = _priceFormatter.FormatPrice(remainingAmount, true, false);

                        model.GiftCards.Add(gcModel);
                    }
                }

                //reward points to be spent (redeemed)
                if (redeemedRewardPointsAmount > decimal.Zero)
                {
                    var redeemedRewardPointsAmountInCustomerCurrency = _currencyService.ConvertFromPrimaryStoreCurrency(redeemedRewardPointsAmount, _workContext.WorkingCurrency);
                    model.RedeemedRewardPoints = redeemedRewardPoints;
                    model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
                }

                //reward points to be earned
                if (_rewardPointsSettings.Enabled && _rewardPointsSettings.DisplayHowMuchWillBeEarned && shoppingCartTotalBase.HasValue)
                {
                    //get shipping total
                    var shippingBaseInclTax = !model.RequiresShipping ? 0 : _orderTotalCalculationService.GetShoppingCartShippingTotal(cart, true, shippingRateComputationMethods) ?? 0;

                    //get total for reward points
                    var totalForRewardPoints = _orderTotalCalculationService
                        .CalculateApplicableOrderTotalForRewardPoints(shippingBaseInclTax, shoppingCartTotalBase.Value);
                    if (totalForRewardPoints > decimal.Zero)
                        model.WillEarnRewardPoints = _orderTotalCalculationService.CalculateRewardPoints(_workContext.CurrentCustomer, totalForRewardPoints);
                }

            }

            return model;
        }

        #endregion
    }
}