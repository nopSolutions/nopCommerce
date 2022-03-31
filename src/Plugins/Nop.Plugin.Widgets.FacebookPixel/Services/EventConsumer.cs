using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.FacebookPixel.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<CustomerRegisteredEvent>,
        IConsumer<EntityInsertedEvent<ShoppingCartItem>>,
        IConsumer<MessageTokensAddedEvent<Token>>,
        IConsumer<ModelPreparedEvent<BaseNopModel>>,
        IConsumer<OrderPlacedEvent>,
        IConsumer<PageRenderingEvent>,
        IConsumer<ProductSearchEvent>
    {
        #region Fields

        private readonly FacebookPixelService _facebookPixelService;
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public EventConsumer(FacebookPixelService facebookPixelService,
            ICategoryService categoryService,
            ICountryService countryService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            IHttpContextAccessor httpContextAccessor,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPriceCalculationService priceCalculationService,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _facebookPixelService = facebookPixelService;
            _categoryService = categoryService;
            _countryService = countryService;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
            _httpContextAccessor = httpContextAccessor;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _priceCalculationService = priceCalculationService;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxService = taxService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare user data for conversions api
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user data
        /// </returns>
        protected async Task<ConversionsEventUserData> PrepareUserDataAsync(Customer customer)
        {
            //prepare user object
            var twoLetterCountryIsoCode = (await _countryService.GetCountryByIdAsync(customer.CountryId))?.TwoLetterIsoCode;
            var stateName = (await _stateProvinceService.GetStateProvinceByIdAsync(customer.StateProvinceId))?.Abbreviation;
            var ipAddress = _webHelper.GetCurrentIpAddress();
            var userAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();

            return new ConversionsEventUserData
            {
                EmailAddress = new List<string> { HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.Email?.ToLowerInvariant() ?? string.Empty), "SHA256") },
                FirstName = new List<string> { HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.FirstName?.ToLowerInvariant() ?? string.Empty), "SHA256") },
                LastName = new List<string> { HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.LastName?.ToLowerInvariant() ?? string.Empty), "SHA256") },
                PhoneNumber = new List<string> { HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.Phone?.ToLowerInvariant() ?? string.Empty), "SHA256") },
                ExternalId = new List<string> { HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer?.CustomerGuid.ToString()?.ToLowerInvariant() ?? string.Empty), "SHA256") },
                Gender = new List<string> { HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.Gender?.FirstOrDefault().ToString() ?? string.Empty), "SHA256") },
                DateOfBirth = new List<string> { HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.DateOfBirth?.ToString("yyyyMMdd") ?? string.Empty), "SHA256") },
                City = new List<string> { HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.City?.ToLowerInvariant() ?? string.Empty), "SHA256") },
                State = new List<string> { HashHelper.CreateHash(Encoding.UTF8.GetBytes(stateName?.ToLowerInvariant() ?? string.Empty), "SHA256") },
                Zip = new List<string> { HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.ZipPostalCode?.ToLowerInvariant() ?? string.Empty), "SHA256") },
                Country = new List<string> { HashHelper.CreateHash(Encoding.UTF8.GetBytes(twoLetterCountryIsoCode?.ToLowerInvariant() ?? string.Empty), "SHA256") },
                ClientIpAddress = ipAddress?.ToLowerInvariant(),
                ClientUserAgent = userAgent?.ToLowerInvariant(),
                Id = customer.Id
            };
        }

        /// <summary>
        /// Prepare add to cart event model
        /// </summary>
        /// <param name="item">Shopping cart item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ConversionsEvent model
        /// </returns>
        protected async Task<ConversionsEvent> PrepareAddToCartEventModelAsync(ShoppingCartItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var eventName = item.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart
                    ? FacebookPixelDefaults.ADD_TO_CART
                    : FacebookPixelDefaults.ADD_TO_WISHLIST;

            var product = await _productService.GetProductByIdAsync(item.ProductId);
            var categoryMapping = (await _categoryService.GetProductCategoriesByProductIdAsync(product?.Id ?? 0)).FirstOrDefault();
            var categoryName = (await _categoryService.GetCategoryByIdAsync(categoryMapping?.CategoryId ?? 0))?.Name;
            var sku = product != null ? await _productService.FormatSkuAsync(product, item.AttributesXml) : string.Empty;
            var quantity = product != null ? (int?)item.Quantity : null;
            var customer = await _workContext.GetCurrentCustomerAsync();
            var (productPrice, _, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, includeDiscounts: false);
            var (price, _) = await _taxService.GetProductPriceAsync(product, productPrice);
            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
            var priceValue = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(price, currentCurrency);
            var currency = currentCurrency?.CurrencyCode;

            var eventObject = new ConversionsEventCustomData
            {
                ContentCategory = categoryName,
                ContentIds = new List<string> { sku },
                ContentName = product?.Name,
                ContentType = "product",
                Contents = new List<object>
                {
                    new
                    {
                        id = sku,
                        quantity = quantity,
                        item_price = priceValue
                    }
                },
                Currency = currency,
                Value = priceValue
            };

            return new ConversionsEvent
            {
                Data = new List<ConversionsEventDatum>
                {
                    new ConversionsEventDatum
                    {
                        EventName = eventName,
                        EventTime = new DateTimeOffset(item.CreatedOnUtc).ToUnixTimeSeconds(),
                        EventSourceUrl = _webHelper.GetThisPageUrl(true),
                        ActionSource = "website",
                        UserData = await PrepareUserDataAsync(customer),
                        CustomData = eventObject,
                        StoreId = item.StoreId
                    }
                }
            };
        }

        /// <summary>
        /// Prepare purchase event model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ConversionsEvent model
        /// </returns>
        protected async Task<ConversionsEvent> PreparePurchaseModelAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //check whether the purchase was initiated by the customer
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (order.CustomerId != customer.Id)
                throw new NopException("Purchase was not initiated by customer");

            //prepare event object
            var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
            var contentsProperties = await (await _orderService.GetOrderItemsAsync(order.Id)).SelectAwait(async item =>
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                var sku = product != null ? await _productService.FormatSkuAsync(product, item.AttributesXml) : string.Empty;
                var quantity = product != null ? (int?)item.Quantity : null;
                return new { id = sku, quantity = quantity };
            }).Cast<object>().ToListAsync();
            var eventObject = new ConversionsEventCustomData
            {
                ContentType = "product",
                Contents = contentsProperties,
                Currency = currency?.CurrencyCode,
                Value = order.OrderTotal
            };

            return new ConversionsEvent
            {
                Data = new List<ConversionsEventDatum>
                {
                    new ConversionsEventDatum
                    {
                        EventName = FacebookPixelDefaults.PURCHASE,
                        EventTime = new DateTimeOffset(order.CreatedOnUtc).ToUnixTimeSeconds(),
                        EventSourceUrl = _webHelper.GetThisPageUrl(true),
                        ActionSource = "website",
                        UserData = await PrepareUserDataAsync(customer),
                        CustomData = eventObject,
                        StoreId = order.StoreId
                    }
                }
            };
        }

        /// <summary>
        /// Prepare view content event model
        /// </summary>
        /// <param name="productDetails">Product details model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ConversionsEvent model
        /// </returns>
        protected async Task<ConversionsEvent> PrepareViewContentModelAsync(ProductDetailsModel productDetails)
        {
            if (productDetails == null)
                throw new ArgumentNullException(nameof(productDetails));

            //prepare event object
            var product = await _productService.GetProductByIdAsync(productDetails.Id);
            var categoryMapping = (await _categoryService.GetProductCategoriesByProductIdAsync(product?.Id ?? 0)).FirstOrDefault();
            var categoryName = (await _categoryService.GetCategoryByIdAsync(categoryMapping?.CategoryId ?? 0))?.Name;
            var sku = productDetails.Sku;
            var priceValue = productDetails.ProductPrice.PriceValue;
            var currency = (await _workContext.GetWorkingCurrencyAsync())?.CurrencyCode;

            var eventObject = new ConversionsEventCustomData
            {
                ContentCategory = categoryName,
                ContentIds = new List<string> { sku },
                ContentName = product?.Name,
                ContentType = "product",
                Currency = currency,
                Value = priceValue
            };

            var customer = await _workContext.GetCurrentCustomerAsync();

            return new ConversionsEvent
            {
                Data = new List<ConversionsEventDatum>
                {
                    new ConversionsEventDatum
                    {
                        EventName = FacebookPixelDefaults.VIEW_CONTENT,
                        EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                        EventSourceUrl = _webHelper.GetThisPageUrl(true),
                        ActionSource = "website",
                        UserData = await PrepareUserDataAsync(customer),
                        CustomData = eventObject
                    }
                }
            };
        }

        /// <summary>
        /// Prepare initiate checkout event model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ConversionsEvent model
        /// </returns>
        protected async Task<ConversionsEvent> PrepareInitiateCheckoutModelAsync()
        {
            //prepare event object
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService
                .GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
            var (price, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, false, false);
            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
            var priceValue = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(price ?? 0, currentCurrency);
            var currency = currentCurrency?.CurrencyCode;

            var contentsProperties = await cart.SelectAwait(async item =>
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                var sku = product != null ? await _productService.FormatSkuAsync(product, item.AttributesXml) : string.Empty;
                var quantity = product != null ? (int?)item.Quantity : null;
                return new { id = sku, quantity = quantity };
            }).Cast<object>().ToListAsync();

            var eventObject = new ConversionsEventCustomData
            {
                ContentType = "product",
                Contents = contentsProperties,
                Currency = currency,
                Value = priceValue
            };

            var customer = await _workContext.GetCurrentCustomerAsync();

            return new ConversionsEvent
            {
                Data = new List<ConversionsEventDatum>
                {
                    new ConversionsEventDatum
                    {
                        EventName = FacebookPixelDefaults.INITIATE_CHECKOUT,
                        EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                        EventSourceUrl = _webHelper.GetThisPageUrl(true),
                        ActionSource = "website",
                        UserData = await PrepareUserDataAsync(customer),
                        CustomData = eventObject
                    }
                }
            };
        }

        /// <summary>
        /// Prepare page view event model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ConversionsEvent model
        /// </returns>
        protected async Task<ConversionsEvent> PreparePageViewModelAsync()
        {
            //prepare event object
            var eventObject = new ConversionsEventCustomData();

            var customer = await _workContext.GetCurrentCustomerAsync();

            return new ConversionsEvent
            {
                Data = new List<ConversionsEventDatum>
                {
                    new ConversionsEventDatum
                    {
                        EventName = FacebookPixelDefaults.PAGE_VIEW,
                        EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                        EventSourceUrl = _webHelper.GetThisPageUrl(true),
                        ActionSource = "website",
                        UserData = await PrepareUserDataAsync(customer),
                        CustomData = eventObject
                    }
                }
            };
        }

        /// <summary>
        /// Prepare search event model
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ConversionsEvent model
        /// </returns>
        protected async Task<ConversionsEvent> PrepareSearchModelAsync(string searchTerm)
        {
            //prepare event object
            var eventObject = new ConversionsEventCustomData
            {
                SearchString = searchTerm
            };

            var customer = await _workContext.GetCurrentCustomerAsync();

            return new ConversionsEvent
            {
                Data = new List<ConversionsEventDatum>
                {
                    new ConversionsEventDatum
                    {
                        EventName = FacebookPixelDefaults.SEARCH,
                        EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                        EventSourceUrl = _webHelper.GetThisPageUrl(true),
                        ActionSource = "website",
                        UserData = await PrepareUserDataAsync(customer),
                        CustomData = eventObject
                    }
                }
            };
        }

        /// <summary>
        /// Prepare contact event model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ConversionsEvent model
        /// </returns>
        protected async Task<ConversionsEvent> PrepareContactModelAsync()
        {
            //prepare event object
            var eventObject = new ConversionsEventCustomData();

            var customer = await _workContext.GetCurrentCustomerAsync();

            return new ConversionsEvent
            {
                Data = new List<ConversionsEventDatum>
                {
                    new ConversionsEventDatum
                    {
                        EventName = FacebookPixelDefaults.CONTACT,
                        EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                        EventSourceUrl = _webHelper.GetThisPageUrl(true),
                        ActionSource = "website",
                        UserData = await PrepareUserDataAsync(customer),
                        CustomData = eventObject
                    }
                }
            };
        }

        /// <summary>
        /// Prepare complete registration event model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ConversionsEvent model
        /// </returns>
        protected async Task<ConversionsEvent> PrepareCompleteRegistrationModelAsync()
        {
            var eventObject = new ConversionsEventCustomData
            {
                Status = true.ToString()
            };

            var customer = await _workContext.GetCurrentCustomerAsync();

            return new ConversionsEvent
            {
                Data = new List<ConversionsEventDatum>
                {
                    new ConversionsEventDatum
                    {
                        EventName = FacebookPixelDefaults.COMPLETE_REGISTRATION,
                        EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                        EventSourceUrl = _webHelper.GetThisPageUrl(true),
                        ActionSource = "website",
                        UserData = await PrepareUserDataAsync(customer),
                        CustomData = eventObject
                    }
                }
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle shopping cart item inserted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
        {
            if (eventMessage?.Entity == null)
                return;

            var eventName = eventMessage.Entity.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart
                ? FacebookPixelDefaults.ADD_TO_CART
                : FacebookPixelDefaults.ADD_TO_WISHLIST;

            var configurations = await _facebookPixelService.GetConfigurationsByEventNameAsync(eventName, eventMessage?.Entity.StoreId ?? 0);
            if (configurations.Count == 0)
                return;

            var model = await PrepareAddToCartEventModelAsync(eventMessage.Entity);

            var conversionsApiConfigurations = configurations.Where(configuration => configuration.ConversionsApiEnabled).ToList();
            if (conversionsApiConfigurations.Count > 0)
                await _facebookPixelService.SendConversionsEventAsync(conversionsApiConfigurations, model);

            var pixelConfigurations = configurations.Where(configuration => configuration.PixelScriptEnabled).ToList();
            if (pixelConfigurations.Count > 0)
                await _facebookPixelService.PreparePixelScriptAsync(model);
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (eventMessage?.Order == null)
                return;

            var configurations = await _facebookPixelService.GetConfigurationsByEventNameAsync(FacebookPixelDefaults.PURCHASE, eventMessage?.Order.StoreId ?? 0);
            if (configurations.Count == 0)
                return;

            var model = await PreparePurchaseModelAsync(eventMessage.Order);

            var conversionsApiConfigurations = configurations.Where(configuration => configuration.ConversionsApiEnabled).ToList();
            if (conversionsApiConfigurations.Count > 0)
                await _facebookPixelService.SendConversionsEventAsync(conversionsApiConfigurations, model);

            var pixelConfigurations = configurations.Where(configuration => configuration.PixelScriptEnabled).ToList();
            if (pixelConfigurations.Count > 0)
                await _facebookPixelService.PreparePixelScriptAsync(model);
        }

        /// <summary>
        /// Handle product details model prepared event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
        {
            if (eventMessage?.Model is not ProductDetailsModel productDetailsModel)
                return;

            var configurations = await _facebookPixelService.GetConfigurationsByEventNameAsync(FacebookPixelDefaults.VIEW_CONTENT);
            if (configurations.Count == 0)
                return;

            var model = await PrepareViewContentModelAsync(productDetailsModel);

            var conversionsApiConfigurations = configurations.Where(configuration => configuration.ConversionsApiEnabled).ToList();
            if (conversionsApiConfigurations.Count > 0)
                await _facebookPixelService.SendConversionsEventAsync(conversionsApiConfigurations, model);

            var pixelConfigurations = configurations.Where(configuration => configuration.PixelScriptEnabled).ToList();
            if (pixelConfigurations.Count > 0)
                await _facebookPixelService.PreparePixelScriptAsync(model);
        }

        /// <summary>
        /// Handle page rendering event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(PageRenderingEvent eventMessage)
        {
            var routeName = eventMessage.GetRouteName() ?? string.Empty;
            if (routeName == FacebookPixelDefaults.CheckoutRouteName || routeName == FacebookPixelDefaults.CheckoutOnePageRouteName)
            {
                var configurations = await _facebookPixelService.GetConfigurationsByEventNameAsync(FacebookPixelDefaults.INITIATE_CHECKOUT);
                if (configurations.Count > 0)
                {
                    var model = await PrepareInitiateCheckoutModelAsync();

                    var conversionsApiConfigurations = configurations.Where(configuration => configuration.ConversionsApiEnabled).ToList();
                    if (conversionsApiConfigurations.Count > 0)
                        await _facebookPixelService.SendConversionsEventAsync(conversionsApiConfigurations, model);

                    var pixelConfigurations = configurations.Where(configuration => configuration.PixelScriptEnabled).ToList();
                    if (pixelConfigurations.Count > 0)
                        await _facebookPixelService.PreparePixelScriptAsync(model);
                }
            }

            if (string.IsNullOrEmpty(routeName) || !routeName.Equals(FacebookPixelDefaults.AreaRouteName, StringComparison.OrdinalIgnoreCase))
            {
                var configurations = await _facebookPixelService.GetConfigurationsByEventNameAsync(FacebookPixelDefaults.PAGE_VIEW);
                if (configurations.Count > 0)
                {
                    var model = await PreparePageViewModelAsync();

                    var conversionsApiConfigurations = configurations.Where(configuration => configuration.ConversionsApiEnabled).ToList();
                    if (conversionsApiConfigurations.Count > 0)
                        await _facebookPixelService.SendConversionsEventAsync(conversionsApiConfigurations, model);
                }
            }
        }

        /// <summary>
        /// Handle product search event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ProductSearchEvent eventMessage)
        {
            if (eventMessage?.SearchTerm == null)
                return;

            var configurations = await _facebookPixelService.GetConfigurationsByEventNameAsync(FacebookPixelDefaults.SEARCH);
            if (configurations.Count == 0)
                return;

            var model = await PrepareSearchModelAsync(eventMessage.SearchTerm);

            var conversionsApiConfigurations = configurations.Where(configuration => configuration.ConversionsApiEnabled).ToList();
            if (conversionsApiConfigurations.Count > 0)
                await _facebookPixelService.SendConversionsEventAsync(conversionsApiConfigurations, model);

            var pixelConfigurations = configurations.Where(configuration => configuration.PixelScriptEnabled).ToList();
            if (pixelConfigurations.Count > 0)
                await _facebookPixelService.PreparePixelScriptAsync(model);
        }

        /// <summary>
        /// Handle message token added event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(MessageTokensAddedEvent<Token> eventMessage)
        {
            if (eventMessage?.Message?.Name == MessageTemplateSystemNames.ContactUsMessage)
            {
                var configurations = await _facebookPixelService.GetConfigurationsByEventNameAsync(FacebookPixelDefaults.CONTACT);
                if (configurations.Count == 0)
                    return;

                var model = await PrepareContactModelAsync();

                var conversionsApiConfigurations = configurations.Where(configuration => configuration.ConversionsApiEnabled).ToList();
                if (conversionsApiConfigurations.Count > 0)
                    await _facebookPixelService.SendConversionsEventAsync(conversionsApiConfigurations, model);

                var pixelConfigurations = configurations.Where(configuration => configuration.PixelScriptEnabled).ToList();
                if (pixelConfigurations.Count > 0)
                    await _facebookPixelService.PreparePixelScriptAsync(model);
            }
        }

        /// <summary>
        /// Handle customer registered event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(CustomerRegisteredEvent eventMessage)
        {
            if (eventMessage?.Customer == null)
                return;

            var configurations = await _facebookPixelService.GetConfigurationsByEventNameAsync(FacebookPixelDefaults.COMPLETE_REGISTRATION);
            if (configurations.Count == 0)
                return;

            var model = await PrepareCompleteRegistrationModelAsync();

            var conversionsApiConfigurations = configurations.Where(configuration => configuration.ConversionsApiEnabled).ToList();
            if (conversionsApiConfigurations.Count > 0)
                await _facebookPixelService.SendConversionsEventAsync(conversionsApiConfigurations, model);

            var pixelConfigurations = configurations.Where(configuration => configuration.PixelScriptEnabled).ToList();
            if (pixelConfigurations.Count > 0)
                await _facebookPixelService.PreparePixelScriptAsync(model);
        }

        #endregion
    }
}