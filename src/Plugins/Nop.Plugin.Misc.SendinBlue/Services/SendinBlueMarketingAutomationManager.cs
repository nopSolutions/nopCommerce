using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task HandleShoppingCartChangedEventAsync(ShoppingCartItem cartItem)
        {
            //whether marketing automation is enabled
            if (!_sendinBlueSettings.UseMarketingAutomation)
                return;

            var customer = await _customerService.GetCustomerByIdAsync(cartItem.CustomerId);

            try
            {
                //create API client
                var client = CreateMarketingAutomationClient();

                //first, try to identify current customer
                await client.IdentifyAsync(new Identify(customer.Email));

                //get shopping cart GUID
                var shoppingCartGuid = await _genericAttributeService
                    .GetAttributeAsync<Guid?>(customer, SendinBlueDefaults.ShoppingCartGuidAttribute);

                //create track event object
                var trackEvent = new TrackEvent(customer.Email, string.Empty);

                //get current customer's shopping cart
                var cart = await _shoppingCartService
                    .GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStoreAsync()).Id);

                if (cart.Any())
                {
                    //get URL helper
                    var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

                    //get shopping cart amounts
                    var (_, cartDiscount, _, cartSubtotal, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart,
                        await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax);
                    var cartTax = await _orderTotalCalculationService.GetTaxTotalAsync(cart, false);
                    var cartShipping = await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart);
                    var (cartTotal, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, false, false);

                    //get products data by shopping cart items
                    var itemsData = await cart.Where(item => item.ProductId != 0).SelectAwait(async item =>
                    {
                        var product = await _productService.GetProductByIdAsync(item.ProductId);

                        //try to get product attribute combination
                        var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, item.AttributesXml);

                        //get default product picture
                        var picture = await _pictureService.GetProductPictureAsync(product, item.AttributesXml);

                        //get product SEO slug name
                        var seName = await _urlRecordService.GetSeNameAsync(product);

                        //create product data
                        return new
                        {
                            id = product.Id,
                            name = product.Name,
                            variant_id = combination?.Id ?? product.Id,
                            variant_name = combination?.Sku ?? product.Name,
                            sku = combination?.Sku ?? product.Sku,
                            category = await (await _categoryService.GetProductCategoriesByProductIdAsync(item.ProductId)).AggregateAwaitAsync(",", async (all, pc) =>
                            {
                                var res = (await _categoryService.GetCategoryByIdAsync(pc.CategoryId)).Name;
                                res = all == "," ? res : all + ", " + res;
                                return res;
                            }),
                            url = urlHelper.RouteUrl("Product", new { SeName = seName }, _webHelper.GetCurrentRequestProtocol()),
                            image = (await _pictureService.GetPictureUrlAsync(picture)).Url,
                            quantity = item.Quantity,
                            price = (await _shoppingCartService.GetSubTotalAsync(item, true)).subTotal
                        };
                    }).ToArrayAsync();

                    //prepare cart data
                    var cartData = new
                    {
                        affiliation = (await _storeContext.GetCurrentStoreAsync()).Name,
                        subtotal = cartSubtotal,
                        shipping = cartShipping ?? decimal.Zero,
                        total_before_tax = cartSubtotal + cartShipping ?? decimal.Zero,
                        tax = cartTax,
                        discount = cartDiscount,
                        revenue = cartTotal ?? decimal.Zero,
                        url = urlHelper.RouteUrl("ShoppingCart", null, _webHelper.GetCurrentRequestProtocol()),
                        currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode,
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
                await client.TrackEventAsync(trackEvent);

                //update GUID for the current customer's shopping cart
                await _genericAttributeService.SaveAttributeAsync(customer, SendinBlueDefaults.ShoppingCartGuidAttribute, shoppingCartGuid);
            }
            catch (Exception exception)
            {
                //log full error
                await _logger.ErrorAsync($"SendinBlue Marketing Automation error: {exception.Message}.", exception, customer);
            }
        }

        /// <summary>
        /// Handle order completed event
        /// </summary>
        /// <param name="order">Order</param>
        public async Task HandleOrderCompletedEventAsync(Order order)
        {
            //whether marketing automation is enabled
            if (!_sendinBlueSettings.UseMarketingAutomation)
                return;

            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            try
            {
                //create API client
                var client = CreateMarketingAutomationClient();

                //first, try to identify current customer
                await client.IdentifyAsync(new Identify(customer.Email));

                //get URL helper
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

                //get products data by order items
                var itemsData = await (await _orderService.GetOrderItemsAsync(order.Id)).SelectAwait(async item =>
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);

                    //try to get product attribute combination
                    var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, item.AttributesXml);

                    //get default product picture
                    var picture = await _pictureService.GetProductPictureAsync(product, item.AttributesXml);

                    //get product SEO slug name
                    var seName = await _urlRecordService.GetSeNameAsync(product);

                    //create product data
                    return new
                    {
                        id = product.Id,
                        name = product.Name,
                        variant_id = combination?.Id ?? product.Id,
                        variant_name = combination?.Sku ?? product.Name,
                        sku = combination?.Sku ?? product.Sku,
                        category = await (await _categoryService.GetProductCategoriesByProductIdAsync(item.ProductId)).AggregateAwaitAsync(",", async (all, pc) =>
                        {
                            var res = (await _categoryService.GetCategoryByIdAsync(pc.CategoryId)).Name;
                            res = all == "," ? res : all + ", " + res;
                            return res;
                        }),
                        url = urlHelper.RouteUrl("Product", new { SeName = seName }, _webHelper.GetCurrentRequestProtocol()),
                        image = (await _pictureService.GetPictureUrlAsync(picture)).Url,
                        quantity = item.Quantity,
                        price = item.PriceInclTax,
                    };
                }).ToArrayAsync();

                var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);
                var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

                var shippingAddressData = new
                {
                    firstname = shippingAddress?.FirstName,
                    lastname = shippingAddress?.LastName,
                    company = shippingAddress?.Company,
                    phone = shippingAddress?.PhoneNumber,
                    address1 = shippingAddress?.Address1,
                    address2 = shippingAddress?.Address2,
                    city = shippingAddress?.City,
                    country = (await _countryService.GetCountryByAddressAsync(shippingAddress))?.Name,
                    state = (await _stateProvinceService.GetStateProvinceByAddressAsync(shippingAddress))?.Name,
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
                    country = (await _countryService.GetCountryByAddressAsync(billingAddress))?.Name,
                    state = (await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress))?.Name,
                    zipcode = billingAddress?.ZipPostalCode
                };

                //prepare cart data
                var cartData = new
                {
                    id = order.Id,
                    affiliation = customer.AffiliateId > 0 ? customer.AffiliateId.ToString() : (await _storeContext.GetCurrentStoreAsync()).Name,
                    date = order.PaidDateUtc?.ToString("yyyy-MM-dd"),
                    subtotal = order.OrderSubtotalInclTax,
                    shipping = order.OrderShippingInclTax,
                    total_before_tax = order.OrderSubtotalInclTax + order.OrderShippingInclTax,
                    tax = order.OrderTax,
                    discount = order.OrderDiscount,
                    revenue = order.OrderTotal,
                    url = urlHelper.RouteUrl("OrderDetails", new { orderId = order.Id }, _webHelper.GetCurrentRequestProtocol()),
                    currency = order.CustomerCurrencyCode,
                    //gift_wrapping = string.Empty, //currently we can't get this value
                    items = itemsData,
                    shipping_address = shippingAddressData,
                    billing_address = billingAddressData
                };

                //get shopping cart GUID
                var shoppingCartGuid = await _genericAttributeService.GetAttributeAsync<Guid?>(order,
                    SendinBlueDefaults.ShoppingCartGuidAttribute) ?? Guid.NewGuid();

                //create track event object
                var trackEvent = new TrackEvent(customer.Email, SendinBlueDefaults.OrderCompletedEventName,
                    eventData: new { id = $"cart:{shoppingCartGuid}", data = cartData });

                //track event
                client.TrackEvent(trackEvent);

                //update GUID for the current customer's shopping cart
                await _genericAttributeService.SaveAttributeAsync<Guid?>(order, SendinBlueDefaults.ShoppingCartGuidAttribute, null);
            }
            catch (Exception exception)
            {
                //log full error
                await _logger.ErrorAsync($"SendinBlue Marketing Automation error: {exception.Message}.", exception, customer);
            }
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="order">Order</param>
        public async Task HandleOrderPlacedEventAsync(Order order)
        {
            //whether marketing automation is enabled
            if (!_sendinBlueSettings.UseMarketingAutomation)
                return;

            //copy shopping cart GUID to order
            var shoppingCartGuid = await _genericAttributeService.GetAttributeAsync<Customer, Guid?>(order.CustomerId, SendinBlueDefaults.ShoppingCartGuidAttribute);
            await _genericAttributeService.SaveAttributeAsync(order, SendinBlueDefaults.ShoppingCartGuidAttribute, shoppingCartGuid);
        }

        #endregion
    }
}