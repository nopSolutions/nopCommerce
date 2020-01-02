using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using SendinBlueMarketingAutomation.Api;
using SendinBlueMarketingAutomation.Client;
using SendinBlueMarketingAutomation.Model;

namespace Nop.Plugin.Misc.SendinBlue.Services
{
    /// <summary>
    /// Represents SendinBlue marketing automation manager
    /// </summary>
    public class SendinBlueMarketingAutomationManager
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPictureService _pictureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly SendinBlueSettings _sendinBlueSettings;

        #endregion

        #region Ctor

        public SendinBlueMarketingAutomationManager(CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            ILogger logger,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPictureService pictureService,
            IPriceCalculationService priceCalculationService,
            IProductAttributeParser productAttributeParser,
            IShippingPluginManager shippingPluginManager,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            SendinBlueSettings sendinBlueSettings)
        {
            _currencySettings = currencySettings;
            _actionContextAccessor = actionContextAccessor;
            _currencyService = currencyService;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
            _orderTotalCalculationService = orderTotalCalculationService;
            _pictureService = pictureService;
            _priceCalculationService = priceCalculationService;
            _productAttributeParser = productAttributeParser;
            _shippingPluginManager = shippingPluginManager;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _sendinBlueSettings = sendinBlueSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare marketing automation API client
        /// </summary>
        /// <returns>Marketing automation API client</returns>
        private MarketingAutomationApi CreateMarketingAutomationClient()
        {
            //validate tracker identifier
            if (string.IsNullOrEmpty(_sendinBlueSettings.MarketingAutomationKey))
                throw new NopException($"Marketing automation not configured");

            var apiConfiguration = new Configuration()
            {
                MaKey = new Dictionary<string, string>
                {
                    [SendinBlueDefaults.MarketingAutomationKeyHeader] = _sendinBlueSettings.MarketingAutomationKey
                },
                UserAgent = SendinBlueDefaults.UserAgent
            };

            return new MarketingAutomationApi(apiConfiguration);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle shopping cart changed event
        /// </summary>
        /// <param name="cartItem">Shopping cart item</param>
        public void HandleShoppingCartChangedEvent(ShoppingCartItem cartItem)
        {
            //whether marketing automation is enabled
            if (!_sendinBlueSettings.UseMarketingAutomation)
                return;

            try
            {
                //create API client
                var client = CreateMarketingAutomationClient();

                //first, try to identify current customer
                client.Identify(new Identify(cartItem.Customer.Email));

                //get shopping cart GUID
                var shoppingCartGuid = _genericAttributeService
                    .GetAttribute<Guid?>(cartItem.Customer, SendinBlueDefaults.ShoppingCartGuidAttribute);

                //create track event object
                var trackEvent = new TrackEvent(cartItem.Customer.Email, string.Empty);

                //get current customer's shopping cart
                var cart = _shoppingCartService
                    .GetShoppingCart(cartItem.Customer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

                if (cart.Any())
                {
                    //get URL helper
                    var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

                    //get shopping cart amounts
                    var shippingRateComputationMethods = _shippingPluginManager
                        .LoadActivePlugins(cartItem.Customer, _storeContext.CurrentStore.Id);
                    _orderTotalCalculationService.GetShoppingCartSubTotal(cart,
                        _workContext.TaxDisplayType == TaxDisplayType.IncludingTax,
                        out var cartDiscount, out _, out var cartSubtotal, out _);
                    var cartTax = _orderTotalCalculationService.GetTaxTotal(cart, shippingRateComputationMethods, false);
                    var cartShipping = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart, shippingRateComputationMethods);
                    var cartTotal = _orderTotalCalculationService.GetShoppingCartTotal(cart, false, false);

                    //get products data by shopping cart items
                    var itemsData = cart.Where(item => item.Product != null).Select(item =>
                    {
                        //try to get product attribute combination
                        var combination = _productAttributeParser.FindProductAttributeCombination(item.Product, item.AttributesXml);

                        //get default product picture
                        var picture = _pictureService.GetProductPicture(item.Product, item.AttributesXml);

                        //get product SEO slug name
                        var seName = _urlRecordService.GetSeName(item.Product);

                        //create product data
                        return new
                        {
                            id = item.Product.Id,
                            name = item.Product.Name,
                            variant_id = combination?.Id ?? item.Product.Id,
                            variant_name = combination?.Sku ?? item.Product.Name,
                            sku = combination?.Sku ?? item.Product.Sku,
                            category = item.Product.ProductCategories.Aggregate(",", (all, category) =>
                            {
                                var res = category.Category.Name;
                                res = all == "," ? res : all + ", " + res;
                                return res;
                            }),
                            url = urlHelper.RouteUrl("Product", new { SeName = seName }, _webHelper.CurrentRequestProtocol),
                            image = _pictureService.GetPictureUrl(picture),
                            quantity = item.Quantity,
                            price = _priceCalculationService.GetSubTotal(item)
                        };
                    }).ToArray();

                    //prepare cart data
                    var cartData = new
                    {
                        affiliation = _storeContext.CurrentStore.Name,
                        subtotal = cartSubtotal,
                        shipping = cartShipping ?? decimal.Zero,
                        total_before_tax = cartSubtotal + cartShipping ?? decimal.Zero,
                        tax = cartTax,
                        discount = cartDiscount,
                        revenue = cartTotal ?? decimal.Zero,
                        url = urlHelper.RouteUrl("ShoppingCart", null, _webHelper.CurrentRequestProtocol),
                        currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)?.CurrencyCode,
                        //gift_wrapping = string.Empty, //currently we can't get this value
                        items = itemsData
                    };

                    //if there is a single item in the cart, so the cart is just created
                    if (cart.Count == 1)
                    {
                        shoppingCartGuid = Guid.NewGuid();
                    }
                    else
                    {
                        //otherwise cart is updated
                        shoppingCartGuid = shoppingCartGuid ?? Guid.NewGuid();
                    }
                    trackEvent.EventName = SendinBlueDefaults.CartUpdatedEventName;
                    trackEvent.EventData = new { id = $"cart:{shoppingCartGuid}", data = cartData };
                }
                else
                {
                    //there are no items in the cart, so the cart is deleted
                    shoppingCartGuid = shoppingCartGuid ?? Guid.NewGuid();
                    trackEvent.EventName = SendinBlueDefaults.CartDeletedEventName;
                    trackEvent.EventData = new { id = $"cart:{shoppingCartGuid}" };
                }

                //track event
                client.TrackEvent(trackEvent);

                //update GUID for the current customer's shopping cart
                _genericAttributeService.SaveAttribute(cartItem.Customer, SendinBlueDefaults.ShoppingCartGuidAttribute, shoppingCartGuid);
            }
            catch (Exception exception)
            {
                //log full error
                _logger.Error($"SendinBlue Marketing Automation error: {exception.Message}.", exception, cartItem.Customer);
            }
        }

        /// <summary>
        /// Handle order completed event
        /// </summary>
        /// <param name="order">Order</param>
        public void HandleOrderCompletedEvent(Order order)
        {
            //whether marketing automation is enabled
            if (!_sendinBlueSettings.UseMarketingAutomation)
                return;

            try
            {
                //create API client
                var client = CreateMarketingAutomationClient();

                //first, try to identify current customer
                client.Identify(new Identify(order.Customer.Email));

                //get URL helper
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

                //get products data by order items
                var itemsData = order.OrderItems.Where(item => item.Product != null).Select(item =>
                {
                    //try to get product attribute combination
                    var combination = _productAttributeParser.FindProductAttributeCombination(item.Product, item.AttributesXml);

                    //get default product picture
                    var picture = _pictureService.GetProductPicture(item.Product, item.AttributesXml);

                    //get product SEO slug name
                    var seName = _urlRecordService.GetSeName(item.Product);

                    //create product data
                    return new
                    {
                        id = item.Product.Id,
                        name = item.Product.Name,
                        variant_id = combination?.Id ?? item.Product.Id,
                        variant_name = combination?.Sku ?? item.Product.Name,
                        sku = combination?.Sku ?? item.Product.Sku,
                        category = item.Product.ProductCategories.Aggregate(",", (all, category) =>
                        {
                            var res = category.Category.Name;
                            res = all == "," ? res : all + ", " + res;
                            return res;
                        }),
                        url = urlHelper.RouteUrl("Product", new { SeName = seName }, _webHelper.CurrentRequestProtocol),
                        image = _pictureService.GetPictureUrl(picture),
                        quantity = item.Quantity,
                        price = item.PriceInclTax,
                    };
                }).ToArray();

                var shippingAddress = order.ShippingAddress;
                var billingAddress = order.BillingAddress;

                var shippingAddressData = new
                {
                    firstname = shippingAddress?.FirstName,
                    lastname = shippingAddress?.LastName,
                    company = shippingAddress?.Company,
                    phone = shippingAddress?.PhoneNumber,
                    address1 = shippingAddress?.Address1,
                    address2 = shippingAddress?.Address2,
                    city = shippingAddress?.City,
                    country = shippingAddress?.Country?.Name,
                    state = shippingAddress?.StateProvince?.Name,
                    zipcode = shippingAddress?.ZipPostalCode
                };

                var billingAddressData = new
                {
                    firstname = billingAddress?.FirstName,
                    lastname = billingAddress?.LastName,
                    company = billingAddress?.Company,
                    phone = billingAddress?.PhoneNumber,
                    address1 = billingAddress?.Address1,
                    address2 = billingAddress?.Address2,
                    city = billingAddress?.City,
                    country = billingAddress?.Country?.Name,
                    state = billingAddress?.StateProvince?.Name,
                    zipcode = billingAddress?.ZipPostalCode
                };

                //prepare cart data
                var cartData = new
                {
                    id = order.Id,
                    affiliation = order.Customer.AffiliateId > 0 ? order.Customer.AffiliateId.ToString() : _storeContext.CurrentStore.Name,
                    date = order.PaidDateUtc?.ToString("yyyy-MM-dd"),
                    subtotal = order.OrderSubtotalInclTax,
                    shipping = order.OrderShippingInclTax,
                    total_before_tax = order.OrderSubtotalInclTax + order.OrderShippingInclTax,
                    tax = order.OrderTax,
                    discount = order.OrderDiscount,
                    revenue = order.OrderTotal,
                    url = urlHelper.RouteUrl("OrderDetails", new { orderId = order.Id }, _webHelper.CurrentRequestProtocol),
                    currency = order.CustomerCurrencyCode,
                    //gift_wrapping = string.Empty, //currently we can't get this value
                    items = itemsData,
                    shipping_address = shippingAddressData,
                    billing_address = billingAddressData
                };

                //get shopping cart GUID
                var shoppingCartGuid = _genericAttributeService.GetAttribute<Guid?>(order,
                    SendinBlueDefaults.ShoppingCartGuidAttribute) ?? Guid.NewGuid();

                //create track event object
                var trackEvent = new TrackEvent(order.Customer.Email, SendinBlueDefaults.OrderCompletedEventName,
                    eventData: new { id = $"cart:{shoppingCartGuid}", data = cartData });

                //track event
                client.TrackEvent(trackEvent);

                //update GUID for the current customer's shopping cart
                _genericAttributeService.SaveAttribute<Guid?>(order, SendinBlueDefaults.ShoppingCartGuidAttribute, null);
            }
            catch (Exception exception)
            {
                //log full error
                _logger.Error($"SendinBlue Marketing Automation error: {exception.Message}.", exception, order.Customer);
            }
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="order">Order</param>
        public void HandleOrderPlacedEvent(Order order)
        {
            //whether marketing automation is enabled
            if (!_sendinBlueSettings.UseMarketingAutomation)
                return;

            //copy shopping cart GUID to order
            var shoppingCartGuid = _genericAttributeService.GetAttribute<Guid?>(order.Customer, SendinBlueDefaults.ShoppingCartGuidAttribute);
            _genericAttributeService.SaveAttribute(order, SendinBlueDefaults.ShoppingCartGuidAttribute, shoppingCartGuid);
        }

        #endregion
    }
}