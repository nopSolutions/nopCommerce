using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
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
        private readonly IAddressService _addressService;
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
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
            IAddressService addressService,
            ICategoryService categoryService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILogger logger,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            SendinBlueSettings sendinBlueSettings)
        {
            _currencySettings = currencySettings;
            _actionContextAccessor = actionContextAccessor;
            _addressService = addressService;
            _categoryService = categoryService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
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

            var customer = _customerService.GetCustomerById(cartItem.CustomerId);

            try
            {
                //create API client
                var client = CreateMarketingAutomationClient();

                //first, try to identify current customer
                client.Identify(new Identify(customer.Email));

                //get shopping cart GUID
                var shoppingCartGuid = _genericAttributeService
                    .GetAttribute<Guid?>(customer, SendinBlueDefaults.ShoppingCartGuidAttribute);

                //create track event object
                var trackEvent = new TrackEvent(customer.Email, string.Empty);

                //get current customer's shopping cart
                var cart = _shoppingCartService
                    .GetShoppingCart(customer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

                if (cart.Any())
                {
                    //get URL helper
                    var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

                    //get shopping cart amounts
                    _orderTotalCalculationService.GetShoppingCartSubTotal(cart,
                        _workContext.TaxDisplayType == TaxDisplayType.IncludingTax,
                        out var cartDiscount, out _, out var cartSubtotal, out _);
                    var cartTax = _orderTotalCalculationService.GetTaxTotal(cart, false);
                    var cartShipping = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart);
                    var cartTotal = _orderTotalCalculationService.GetShoppingCartTotal(cart, false, false);

                    //get products data by shopping cart items
                    var itemsData = cart.Where(item => item.ProductId != 0).Select(item =>
                    {
                        var product = _productService.GetProductById(item.ProductId);

                        //try to get product attribute combination
                        var combination = _productAttributeParser.FindProductAttributeCombination(product, item.AttributesXml);

                        //get default product picture
                        var picture = _pictureService.GetProductPicture(product, item.AttributesXml);

                        //get product SEO slug name
                        var seName = _urlRecordService.GetSeName(product);

                        //create product data
                        return new
                        {
                            id = product.Id,
                            name = product.Name,
                            variant_id = combination?.Id ?? product.Id,
                            variant_name = combination?.Sku ?? product.Name,
                            sku = combination?.Sku ?? product.Sku,
                            category = _categoryService.GetProductCategoriesByProductId(item.ProductId).Aggregate(",", (all, pc) =>
                            {
                                var res = _categoryService.GetCategoryById(pc.CategoryId).Name;
                                res = all == "," ? res : all + ", " + res;
                                return res;
                            }),
                            url = urlHelper.RouteUrl("Product", new { SeName = seName }, _webHelper.CurrentRequestProtocol),
                            image = _pictureService.GetPictureUrl(ref picture),
                            quantity = item.Quantity,
                            price = _shoppingCartService.GetSubTotal(item)
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
                        shoppingCartGuid ??= Guid.NewGuid();
                    }
                    trackEvent.EventName = SendinBlueDefaults.CartUpdatedEventName;
                    trackEvent.EventData = new { id = $"cart:{shoppingCartGuid}", data = cartData };
                }
                else
                {
                    //there are no items in the cart, so the cart is deleted
                    shoppingCartGuid ??= Guid.NewGuid();
                    trackEvent.EventName = SendinBlueDefaults.CartDeletedEventName;
                    trackEvent.EventData = new { id = $"cart:{shoppingCartGuid}" };
                }

                //track event
                client.TrackEvent(trackEvent);

                //update GUID for the current customer's shopping cart
                _genericAttributeService.SaveAttribute(customer, SendinBlueDefaults.ShoppingCartGuidAttribute, shoppingCartGuid);
            }
            catch (Exception exception)
            {
                //log full error
                _logger.Error($"SendinBlue Marketing Automation error: {exception.Message}.", exception, customer);
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

            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var customer = _customerService.GetCustomerById(order.CustomerId);

            try
            {
                //create API client
                var client = CreateMarketingAutomationClient();

                //first, try to identify current customer
                client.Identify(new Identify(customer.Email));

                //get URL helper
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

                //get products data by order items
                var itemsData = _orderService.GetOrderItems(order.Id).Select(item =>
                {
                    var product = _productService.GetProductById(item.ProductId);

                    //try to get product attribute combination
                    var combination = _productAttributeParser.FindProductAttributeCombination(product, item.AttributesXml);

                    //get default product picture
                    var picture = _pictureService.GetProductPicture(product, item.AttributesXml);

                    //get product SEO slug name
                    var seName = _urlRecordService.GetSeName(product);

                    //create product data
                    return new
                    {
                        id = product.Id,
                        name = product.Name,
                        variant_id = combination?.Id ?? product.Id,
                        variant_name = combination?.Sku ?? product.Name,
                        sku = combination?.Sku ?? product.Sku,
                        category = _categoryService.GetProductCategoriesByProductId(item.ProductId).Aggregate(",", (all, pc) =>
                        {
                            var res = _categoryService.GetCategoryById(pc.CategoryId).Name;
                            res = all == "," ? res : all + ", " + res;
                            return res;
                        }),
                        url = urlHelper.RouteUrl("Product", new { SeName = seName }, _webHelper.CurrentRequestProtocol),
                        image = _pictureService.GetPictureUrl(ref picture),
                        quantity = item.Quantity,
                        price = item.PriceInclTax,
                    };
                }).ToArray();

                var shippingAddress = _addressService.GetAddressById(order.ShippingAddressId ?? 0);
                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var shippingAddressData = new
                {
                    firstname = shippingAddress?.FirstName,
                    lastname = shippingAddress?.LastName,
                    company = shippingAddress?.Company,
                    phone = shippingAddress?.PhoneNumber,
                    address1 = shippingAddress?.Address1,
                    address2 = shippingAddress?.Address2,
                    city = shippingAddress?.City,
                    country = _countryService.GetCountryByAddress(shippingAddress)?.Name,
                    state = _stateProvinceService.GetStateProvinceByAddress(shippingAddress)?.Name,
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
                    country = _countryService.GetCountryByAddress(billingAddress)?.Name,
                    state = _stateProvinceService.GetStateProvinceByAddress(billingAddress)?.Name,
                    zipcode = billingAddress?.ZipPostalCode
                };

                //prepare cart data
                var cartData = new
                {
                    id = order.Id,
                    affiliation = customer.AffiliateId > 0 ? customer.AffiliateId.ToString() : _storeContext.CurrentStore.Name,
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
                var trackEvent = new TrackEvent(customer.Email, SendinBlueDefaults.OrderCompletedEventName,
                    eventData: new { id = $"cart:{shoppingCartGuid}", data = cartData });

                //track event
                client.TrackEvent(trackEvent);

                //update GUID for the current customer's shopping cart
                _genericAttributeService.SaveAttribute<Guid?>(order, SendinBlueDefaults.ShoppingCartGuidAttribute, null);
            }
            catch (Exception exception)
            {
                //log full error
                _logger.Error($"SendinBlue Marketing Automation error: {exception.Message}.", exception, customer);
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
            var shoppingCartGuid = _genericAttributeService.GetAttribute<Customer, Guid?>(order.CustomerId, SendinBlueDefaults.ShoppingCartGuidAttribute);
            _genericAttributeService.SaveAttribute(order, SendinBlueDefaults.ShoppingCartGuidAttribute, shoppingCartGuid);
        }

        #endregion
    }
}