using System.Drawing;
using System.Drawing.Imaging;
using System.Linq.Dynamic.Core;
using DocumentFormat.OpenXml.EMMA;
using LinqToDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Models.Orders;
//using Nop.Web.Areas.Admin.Factories;
//using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.Pos.Controllers
{
    public class PosController : BaseController
    {

        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IDownloadService _downloadService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;
        private readonly INopUrlHelper _nopUrlHelper;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly MediaSettings _mediaSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly VendorSettings _vendorSettings;
        private readonly IVendorService _vendorService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISearchTermService _searchTermService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IProductModelFactory _productModelFactory;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly Nop.Web.Factories.IOrderModelFactory _orderModelFactory;
        private readonly Nop.Web.Areas.Admin.Factories.IOrderModelFactory _orderModelFactory1;
        private readonly IOrderService _orderService;
        private readonly IRepository<ShoppingCartItem> _sciRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Address> _addressRepository;

        
        private readonly IRepository<Order> _orderRepository;

        #endregion

        #region CTOR

        public PosController(CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            INopUrlHelper nopUrlHelper,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings,
            ShippingSettings shippingSettings,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            VendorSettings vendorSettings,
            IVendorService vendorService,
            CatalogSettings catalogSettings,
            IHttpContextAccessor httpContextAccessor,
            ISearchTermService searchTermService,
            IEventPublisher eventPublisher,
            IProductModelFactory productModelFactory,
            Nop.Web.Factories.IOrderModelFactory orderModelFactory,
            Nop.Web.Areas.Admin.Factories.IOrderModelFactory orderModelFactory1,
            ICheckoutModelFactory checkoutModelFactory,
            IOrderService orderService,
            IRepository<ShoppingCartItem> ShoppingCartItemrepository,
            IRepository<Customer> customerRepository,
            IRepository<Address> addressRepository,
            IRepository<Order> orderRepository)
        {
            _captchaSettings = captchaSettings;
            _customerSettings = customerSettings;
            _checkoutAttributeParser = checkoutAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _discountService = discountService;
            _downloadService = downloadService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _htmlFormatter = htmlFormatter;
            _localizationService = localizationService;
            _fileProvider = fileProvider;
            _nopUrlHelper = nopUrlHelper;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _shippingService = shippingService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _taxService = taxService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _mediaSettings = mediaSettings;
            _orderSettings = orderSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _shippingSettings = shippingSettings;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _vendorSettings = vendorSettings;
            _vendorService = vendorService;
            _catalogSettings = catalogSettings;
            _httpContextAccessor = httpContextAccessor;
            _searchTermService = searchTermService;
            _eventPublisher = eventPublisher;
            _productModelFactory = productModelFactory;
            _checkoutModelFactory = checkoutModelFactory;
            _orderService = orderService;
            _orderModelFactory = orderModelFactory;
            _sciRepository = ShoppingCartItemrepository;
            _orderModelFactory1 = orderModelFactory1;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _addressRepository = addressRepository;
        }

        #endregion

        #region UIMethods

        public async Task<IActionResult> Index()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerrole = await _customerService.GetCustomerRoleIdsAsync(customer);
            foreach (var item in customerrole)
            {
                if (item == 1 || item == 6)
                {
                    return View("~/Plugins/Pos/Views/Index.cshtml");
                }
            }

            return RedirectToAction("Login", "Customer");
        }

        public async Task<IActionResult> Neworder(IFormCollection form)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerrole = await _customerService.GetCustomerRoleIdsAsync(customer);
            foreach (var item in customerrole)
            {
                if (item == 1 || item == 6)
                {
                    var model = Cart().Result;

                    model = updatecart(form).Result;

                    //ViewBag.product = model;

                    return View("~/Plugins/Pos/Views/Neworder.cshtml" , model);
                }
            }

            return RedirectToAction("Login", "Customer");
        }

        public async Task<IActionResult> Searchorder()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerrole = await _customerService.GetCustomerRoleIdsAsync(customer);
            foreach (var item in customerrole)
            {
                if (item == 1 || item == 6)
                {
                    return View("~/Plugins/Pos/Views/Searchorder.cshtml");
                }
            }
            return RedirectToAction("Login", "Customer");

        }

        [HttpPost]
        public async Task<IActionResult> Scan(string myInputValue)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var carttype = ShoppingCartType.ShoppingCart;
            var model = new ShoppingCartModel();
            if (myInputValue != null)
            {
                var product = await _productService.GetProductBySkuAsync(myInputValue);
                if (product != null)
                {
                    var test = AddToCartAsync(await _workContext.GetCurrentCustomerAsync(), product, carttype, store.Id);
                   
                    var cart = GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), carttype, store.Id);

                    model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
                   
                }
            }

            //ViewBag.product = model;
            // return View("~/Plugins/Pos/Views/Neworder.cshtml");
            //return PartialView("PosCart", model);
            if (model.Items.Count != 0)
            {
                return RedirectToAction("Neworder", "Pos");
            }
            return View(model);
            //return "";
        }

        public virtual async Task<IActionResult> StartCheckout(IFormCollection form)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            //validate attributes
            var checkoutAttributes = await _genericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.CheckoutAttributes, store.Id);
            var checkoutAttributeWarnings = await _shoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributes, true);
            //if (checkoutAttributeWarnings.Any())
            //{
            //    //something wrong, redisplay the page with warnings
            //    var model = new ShoppingCartModel();
            //    model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart, validateCheckoutAttributes: true);
            //    return View(model);
            //}

            var anonymousPermissed = _orderSettings.AnonymousCheckoutAllowed
                                     && _customerSettings.UserRegistrationType == UserRegistrationType.Disabled;

            if (anonymousPermissed || !await _customerService.IsGuestAsync(customer))
                return RedirectToAction("Index", "CheckoutPos");

            var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();
            var downloadableProductsRequireRegistration =
                _customerSettings.RequireRegistrationForDownloadableProducts && await _productService.HasAnyDownloadableProductAsync(cartProductIds);

            if (!_orderSettings.AnonymousCheckoutAllowed || downloadableProductsRequireRegistration)
            {
                //verify user identity (it may be facebook login page, or google, or local)
                return Challenge();
            }

            return RedirectToAction("Index", "CheckoutPos");
        }

        #endregion

        #region Methods
        public virtual async Task<ShoppingCartModel> Cart()
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
            var model = new ShoppingCartModel();
            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
            return model;
        }

        public virtual async Task<ShoppingCartModel> updatecart(IFormCollection form, IList<ShoppingCartItem> scancart = null)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            //var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            var cart = GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            var itemIdsToRemove = form["removefromcart"]
                .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(idString => int.TryParse(idString, out var id) ? id : 0)
                .Distinct().ToList();

            var products = (await _productService.GetProductsByIdsAsync(cart.Select(item => item.ProductId).Distinct().ToArray()))
                .ToDictionary(item => item.Id, item => item);

            //get order items with changed quantity
            var itemsWithNewQuantity = cart.Select(item => new
            {
                //try to get a new quantity for the item, set 0 for items to remove
                NewQuantity = itemIdsToRemove.Contains(item.Id) ? 0 : int.TryParse(form[$"itemquantity{item.Id}"], out var quantity) ? quantity : item.Quantity,
                Item = item,
                Product = products.ContainsKey(item.ProductId) ? products[item.ProductId] : null
            }).Where(item => item.NewQuantity != item.Item.Quantity);

            //order cart items
            //first should be items with a reduced quantity and that require other products; or items with an increased quantity and are required for other products
            var orderedCart = await itemsWithNewQuantity
                .OrderByDescendingAwait(async cartItem =>
                    (cartItem.NewQuantity < cartItem.Item.Quantity &&
                     (cartItem.Product?.RequireOtherProducts ?? false)) ||
                    (cartItem.NewQuantity > cartItem.Item.Quantity && cartItem.Product != null && (await _shoppingCartService
                         .GetProductsRequiringProductAsync(cart, cartItem.Product)).Any()))
                .ToListAsync();

            //try to update cart items with new quantities and get warnings
            var warnings = await orderedCart.SelectAwait(async cartItem => new
            {
                ItemId = cartItem.Item.Id,
                Warnings = await _shoppingCartService.UpdateShoppingCartItemAsync(customer,
                    cartItem.Item.Id, cartItem.Item.AttributesXml, cartItem.Item.CustomerEnteredPrice,
                    cartItem.Item.RentalStartDateUtc, cartItem.Item.RentalEndDateUtc, cartItem.NewQuantity, true)
            }).ToListAsync();

            //updated cart
            //cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            cart = GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            //parse and save checkout attributes
            await ParseAndSaveCheckoutAttributesAsync(cart, form);

            //prepare model
            var model = new ShoppingCartModel();
            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);

            //update current warnings
            foreach (var warningItem in warnings.Where(warningItem => warningItem.Warnings.Any()))
            {
                //find shopping cart item model to display appropriate warnings
                var itemModel = model.Items.FirstOrDefault(item => item.Id == warningItem.ItemId);
                if (itemModel != null)
                    itemModel.Warnings = warningItem.Warnings.Concat(itemModel.Warnings).Distinct().ToList();
            }

            return model;
        }

        public virtual IList<ShoppingCartItem> GetShoppingCartAsync(Customer customer, ShoppingCartType? shoppingCartType = null,
           int storeId = 0, int? productId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var items = _sciRepository.Table.Where(sci => sci.CustomerId == customer.Id);

            //filter by type
            if (shoppingCartType.HasValue)
                items = items.Where(item => item.ShoppingCartTypeId == (int)shoppingCartType.Value);

            //filter shopping cart items by store
            if (storeId > 0 && !_shoppingCartSettings.CartsSharedBetweenStores)
                items = items.Where(item => item.StoreId == storeId);

            //filter shopping cart items by product
            if (productId > 0)
                items = items.Where(item => item.ProductId == productId);

            //filter shopping cart items by date
            if (createdFromUtc.HasValue)
                items = items.Where(item => createdFromUtc.Value <= item.CreatedOnUtc);
            if (createdToUtc.HasValue)
                items = items.Where(item => createdToUtc.Value >= item.CreatedOnUtc);

            var key = _staticCacheManager.PrepareKeyForShortTermCache(NopOrderDefaults.ShoppingCartItemsAllCacheKey, customer, shoppingCartType, storeId, productId, createdFromUtc, createdToUtc);

            return _staticCacheManager.GetAsync(key, async () => await items.ToListAsync()).Result;
        }

        protected virtual async Task ParseAndSaveCheckoutAttributesAsync(IList<ShoppingCartItem> cart, IFormCollection form)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var excludeShippableAttributes = !await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            var store = await _storeContext.GetCurrentStoreAsync();
            var checkoutAttributes = await _checkoutAttributeService.GetAllCheckoutAttributesAsync(store.Id, excludeShippableAttributes);
            foreach (var attribute in checkoutAttributes)
            {
                var controlId = $"checkout_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = "1";
                            //var ctrlAttributes = controlId;
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = await _checkoutAttributeService.GetCheckoutAttributeValuesAsync(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }

                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(date));
                            }
                            catch
                            {
                                // ignored
                            }

                            if (selectedDate.HasValue)
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                        }

                        break;
                    case AttributeControlType.FileUpload:
                        {
                            _ = Guid.TryParse(form[controlId], out var downloadGuid);
                            var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                           attribute, download.DownloadGuid.ToString());
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            //validate conditional attributes (if specified)
            foreach (var attribute in checkoutAttributes)
            {
                var conditionMet = await _checkoutAttributeParser.IsConditionMetAsync(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                    attributesXml = _checkoutAttributeParser.RemoveCheckoutAttribute(attributesXml, attribute);
            }

            //save checkout attributes
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.CheckoutAttributes, attributesXml, store.Id);
        }


        public virtual async Task<ShoppingCartItem> FindShoppingCartItemInTheCartAsync(IList<ShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
            Product product,
            string attributesXml = "",
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null,
            DateTime? rentalEndDate = null)
        {
            if (shoppingCart == null)
                throw new ArgumentNullException(nameof(shoppingCart));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return await shoppingCart.Where(sci => sci.ShoppingCartType == shoppingCartType)
                .FirstOrDefaultAwaitAsync(async sci => await ShoppingCartItemIsEqualAsync(sci, product, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate));
        }

        protected virtual async Task<bool> ShoppingCartItemIsEqualAsync(ShoppingCartItem shoppingCartItem,
            Product product,
            string attributesXml,
            decimal customerEnteredPrice,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate)
        {
            if (shoppingCartItem.ProductId != product.Id)
                return false;

            //attributes
            var attributesEqual = await _productAttributeParser.AreProductAttributesEqualAsync(shoppingCartItem.AttributesXml, attributesXml, false, false);
            if (!attributesEqual)
                return false;

            //gift cards
            if (product.IsGiftCard)
            {
                _productAttributeParser.GetGiftCardAttribute(attributesXml, out var giftCardRecipientName1, out var _, out var giftCardSenderName1, out var _, out var _);

                _productAttributeParser.GetGiftCardAttribute(shoppingCartItem.AttributesXml, out var giftCardRecipientName2, out var _, out var giftCardSenderName2, out var _, out var _);

                var giftCardsAreEqual = giftCardRecipientName1.Equals(giftCardRecipientName2, StringComparison.InvariantCultureIgnoreCase)
                    && giftCardSenderName1.Equals(giftCardSenderName2, StringComparison.InvariantCultureIgnoreCase);
                if (!giftCardsAreEqual)
                    return false;
            }

            //price is the same (for products which require customers to enter a price)
            if (product.CustomerEntersPrice)
            {
                //we use rounding to eliminate errors associated with storing real numbers in memory when comparing
                var customerEnteredPricesEqual = Math.Round(shoppingCartItem.CustomerEnteredPrice, 2) == Math.Round(customerEnteredPrice, 2);
                if (!customerEnteredPricesEqual)
                    return false;
            }

            if (!product.IsRental)
                return true;

            //rental products
            var rentalInfoEqual = shoppingCartItem.RentalStartDateUtc == rentalStartDate && shoppingCartItem.RentalEndDateUtc == rentalEndDate;

            return rentalInfoEqual;
        }


        protected virtual bool IsCustomerShoppingCartEmpty(Customer customer)
        {
            return !_sciRepository.Table.Any(sci => sci.CustomerId == customer.Id);
        }

        //add to cart
        public virtual IList<string> AddToCartAsync(Customer customer, Product product,
            ShoppingCartType shoppingCartType, int storeId, string attributesXml = null,
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool addRequiredProducts = true)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var warnings = new List<string>();
            if (shoppingCartType == ShoppingCartType.ShoppingCart && ! _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart, customer).Result)
            {
                warnings.Add("Shopping cart is disabled");
                return warnings;
            }

            if (shoppingCartType == ShoppingCartType.Wishlist && ! _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist, customer).Result)
            {
                warnings.Add("Wishlist is disabled");
                return warnings;
            }

            if (customer.IsSearchEngineAccount())
            {
                warnings.Add("Search engine can't add to cart");
                return warnings;
            }

            if (quantity <= 0)
            {
                warnings.Add( _localizationService.GetResourceAsync("ShoppingCart.QuantityShouldPositive").Result);
                return warnings;
            }

            //reset checkout info
             _customerService.ResetCheckoutDataAsync(customer, storeId);

            var cart = GetShoppingCartAsync(customer, shoppingCartType, storeId);

            var shoppingCartItem = FindShoppingCartItemInTheCartAsync(cart,
                shoppingCartType, product, attributesXml, customerEnteredPrice,
                rentalStartDate, rentalEndDate).Result;

            if (shoppingCartItem != null)
            {
                //update existing shopping cart item
                var newQuantity = shoppingCartItem.Quantity + quantity;
                warnings.AddRange(_shoppingCartService.GetShoppingCartItemWarningsAsync(customer, shoppingCartType, product,
                    storeId, attributesXml,
                    customerEnteredPrice, rentalStartDate, rentalEndDate,
                    newQuantity, addRequiredProducts, shoppingCartItem.Id).Result);

                if (warnings.Any())
                    return warnings;

                addRequiredProductsToCartAsync();

                if (warnings.Any())
                    return warnings;

                shoppingCartItem.AttributesXml = attributesXml;
                shoppingCartItem.Quantity = newQuantity;
                shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

                 _sciRepository.UpdateAsync(shoppingCartItem);
            }
            else
            {
                //new shopping cart item
                warnings.AddRange( _shoppingCartService.GetShoppingCartItemWarningsAsync(customer, shoppingCartType, product,
                    storeId, attributesXml, customerEnteredPrice,
                    rentalStartDate, rentalEndDate,
                    quantity, addRequiredProducts).Result);

                //if (warnings.Any())
                //    return warnings;

                addRequiredProductsToCartAsync();

                //if (warnings.Any())
                //    return warnings;

                //maximum items validation
                switch (shoppingCartType)
                {
                    case ShoppingCartType.ShoppingCart:
                        if (cart.Count >= _shoppingCartSettings.MaximumShoppingCartItems)
                        {
                            warnings.Add(string.Format( _localizationService.GetResourceAsync("ShoppingCart.MaximumShoppingCartItems").Result, _shoppingCartSettings.MaximumShoppingCartItems));
                            return warnings;
                        }

                        break;
                    case ShoppingCartType.Wishlist:
                        if (cart.Count >= _shoppingCartSettings.MaximumWishlistItems)
                        {
                            warnings.Add(string.Format(_localizationService.GetResourceAsync("ShoppingCart.MaximumWishlistItems").Result, _shoppingCartSettings.MaximumWishlistItems));
                            return warnings;
                        }

                        break;
                    default:
                        break;
                }

                var now = DateTime.UtcNow;
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartType = shoppingCartType,
                    StoreId = storeId,
                    ProductId = product.Id,
                    AttributesXml = attributesXml,
                    CustomerEnteredPrice = customerEnteredPrice,
                    Quantity = quantity,
                    RentalStartDateUtc = rentalStartDate,
                    RentalEndDateUtc = rentalEndDate,
                    CreatedOnUtc = now,
                    UpdatedOnUtc = now,
                    CustomerId = customer.Id
                };

                _sciRepository.InsertAsync(shoppingCartItem);

                //updated "HasShoppingCartItems" property used for performance optimization
                customer.HasShoppingCartItems = ! IsCustomerShoppingCartEmpty(customer);

                _customerService.UpdateCustomerAsync(customer);
            }

            return warnings;

            async Task addRequiredProductsToCartAsync()
            {
                //get these required products
                var requiredProducts = await _productService.GetProductsByIdsAsync(_productService.ParseRequiredProductIds(product));
                if (!requiredProducts.Any())
                    return;

                foreach (var requiredProduct in requiredProducts)
                {
                    var productsRequiringRequiredProduct = await _shoppingCartService.GetProductsRequiringProductAsync(cart, requiredProduct);

                    //get the required quantity of the required product
                    var requiredProductRequiredQuantity = quantity +
                        cart.Where(ci => productsRequiringRequiredProduct.Any(p => p.Id == ci.ProductId))
                            .Where(item => item.Id != (shoppingCartItem?.Id ?? 0))
                            .Sum(item => item.Quantity);

                    //whether required product is already in the cart in the required quantity
                    var quantityToAdd = requiredProductRequiredQuantity - (cart.FirstOrDefault(item => item.ProductId == requiredProduct.Id)?.Quantity ?? 0);
                    if (quantityToAdd <= 0)
                        continue;

                    if (addRequiredProducts && product.AutomaticallyAddRequiredProducts)
                    {
                        //do not add required products to prevent circular references
                        var addToCartWarnings = AddToCartAsync(customer, requiredProduct, shoppingCartType, storeId,
                            quantity: quantityToAdd, addRequiredProducts: requiredProduct.AutomaticallyAddRequiredProducts);

                        if (addToCartWarnings.Any())
                        {
                            warnings.AddRange(addToCartWarnings);
                            return;
                        }
                    }
                }
            }
        }
        public virtual async Task<IActionResult> Details(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (order == null)
                return Challenge();
            var model = await _orderModelFactory.PrepareOrderDetailsModelAsync(order);
            return View("~/Plugins/Pos/Views/Details.cshtml", model);
        }


        public virtual async Task<IActionResult> OrderList(List<Order> order, string SearchValue, IFormCollection form)
        {
            if (SearchValue == null)
            {
                order = await GetAllOrders();
                //return Json(model);
                return View("~/Plugins/Pos/Views/Orderlist.cshtml", order);

            }



            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            var OrderNumber = form["Search1"].FirstOrDefault();
            var CustomerName = form["Search2"].FirstOrDefault();
            var CustomerEmail = form["Search3"].FirstOrDefault();


            if (!string.IsNullOrEmpty(OrderNumber))
            {
                order = await GetAllOrderByID(SearchValue, "OrderNumber");

                return View("~/Plugins/Pos/Views/Orderlist.cshtml", order);
            }


            if (!string.IsNullOrEmpty(CustomerName))
            {
                order = await GetAllOrderByID(SearchValue, "CustomerName");

                return View("~/Plugins/Pos/Views/Orderlist.cshtml", order);
            }


            if (!string.IsNullOrEmpty(CustomerEmail))
            {
                order = await GetAllOrderByID(SearchValue, "CustomerEmail");

                return View("~/Plugins/Pos/Views/Orderlist.cshtml", order);
            }


            order = await GetAllOrders();
            //return Json(model);
            return View("~/Plugins/Pos/Views/Orderlist.cshtml", order);

        }


        //Displays the list of all orders
        public virtual async Task<List<Order>> GetAllOrders()
        {
            var query = await _orderRepository.Table.ToListAsync();
            return query;
        }


        //Displays the list of Orders based on OrderNumber , Customer Name , Customer Email.
        public virtual async Task<List<Order>> GetAllOrderByID(string SearchValue, string? selectedSize)
        {
            bool isIntString = SearchValue.All(char.IsDigit);

            if (isIntString == true && selectedSize == "OrderNumber")
            {
                int search = Convert.ToInt32(SearchValue);
                var query = await _orderRepository.Table.Where(c => c.Id == search).ToListAsync();
                return query;

            }

            var custid = await _customerRepository.Table.Where(c => c.FirstName.ToLower() == SearchValue.ToLower() || c.Email.ToLower() == SearchValue.ToLower()).Select(c => c.Id).ToListAsync();
            var queryy = new List<Order>();
            var que = new List<Order>();

            if (custid == null)
            {
                return null;
            }


            if (selectedSize == "CustomerName" || selectedSize == "CustomerEmail")
            {
                List<Order> resultados = new List<Order>();
                custid.ForEach(x =>
                {
                    var listaResultados = _orderRepository.Table.Where(y => y.CustomerId == x).ToList();
                    resultados.AddRange(listaResultados);
                });
                return resultados;
            }
            return null;
        }

        #endregion
    }
}
