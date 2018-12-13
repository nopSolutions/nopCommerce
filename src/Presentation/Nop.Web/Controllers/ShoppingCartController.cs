using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Html;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Controllers
{
    public partial class ShoppingCartController : BasePublicController
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
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly MediaSettings _mediaSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public ShoppingCartController(CaptchaSettings captchaSettings,
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
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings)
        {
            this._captchaSettings = captchaSettings;
            this._customerSettings = customerSettings;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._checkoutAttributeService = checkoutAttributeService;
            this._currencyService = currencyService;
            this._customerActivityService = customerActivityService;
            this._customerService = customerService;
            this._discountService = discountService;
            this._downloadService = downloadService;
            this._genericAttributeService = genericAttributeService;
            this._giftCardService = giftCardService;
            this._localizationService = localizationService;
            this._fileProvider = fileProvider;
            this._notificationService = notificationService;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._productAttributeParser = productAttributeParser;
            this._productAttributeService = productAttributeService;
            this._productService = productService;
            this._shoppingCartModelFactory = shoppingCartModelFactory;
            this._shoppingCartService = shoppingCartService;
            this._cacheManager = cacheManager;
            this._storeContext = storeContext;
            this._taxService = taxService;
            this._urlRecordService = urlRecordService;
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._mediaSettings = mediaSettings;
            this._orderSettings = orderSettings;
            this._shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Utilities

        protected virtual void ParseAndSaveCheckoutAttributes(IList<ShoppingCartItem> cart, IFormCollection form)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var excludeShippableAttributes = !_shoppingCartService.ShoppingCartRequiresShipping(cart);
            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, excludeShippableAttributes);
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
                            var ctrlAttributes = form[controlId];
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
                            var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
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
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid.TryParse(form[controlId], out Guid downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
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
                var conditionMet = _checkoutAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                    attributesXml = _checkoutAttributeParser.RemoveCheckoutAttribute(attributesXml, attribute);
            }

            //save checkout attributes
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, NopCustomerDefaults.CheckoutAttributes, attributesXml, _storeContext.CurrentStore.Id);
        }

        /// <summary>
        /// Parse product attributes on the product details page
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <param name="errors">Errors</param>
        /// <returns>Parsed attributes</returns>
        protected virtual string ParseProductAttributes(Product product, IFormCollection form, List<string> errors)
        {
            //product attributes
            var attributesXml = GetProductAttributesXml(product, form, errors);

            //gift cards
            AddGiftCardsAttributesXml(product, form, ref attributesXml);

            return attributesXml;
        }

        /// <summary>
        /// Parse product rental dates on the product details page
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        protected virtual void ParseRentalDates(Product product, IFormCollection form,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            var startControlId = $"rental_start_date_{product.Id}";
            var endControlId = $"rental_end_date_{product.Id}";
            var ctrlStartDate = form[startControlId];
            var ctrlEndDate = form[endControlId];
            try
            {
                //currently we support only this format (as in the \Views\Product\_RentalInfo.cshtml file)
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }

        protected virtual void AddGiftCardsAttributesXml(Product product, IFormCollection form, ref string attributesXml)
        {
            if (!product.IsGiftCard) return;

            var recipientName = "";
            var recipientEmail = "";
            var senderName = "";
            var senderEmail = "";
            var giftCardMessage = "";
            foreach (var formKey in form.Keys)
            {
                if (formKey.Equals($"giftcard_{product.Id}.RecipientName", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientName = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.RecipientEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientEmail = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.SenderName", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderName = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.SenderEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderEmail = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.Message", StringComparison.InvariantCultureIgnoreCase))
                {
                    giftCardMessage = form[formKey];
                }
            }

            attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
        }

        protected virtual string GetProductAttributesXml(Product product, IFormCollection form, List<string> errors)
        {
            var attributesXml = string.Empty;
            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                var controlId = $"product_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                {
                                    //get quantity entered by customer
                                    var quantity = 1;
                                    var quantityStr = form[$"product_attribute_{attribute.Id}_{selectedAttributeId}_qty"];
                                    if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                        (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                        errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.ToString()
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                    {
                                        //get quantity entered by customer
                                        var quantity = 1;
                                        var quantityStr = form[$"product_attribute_{attribute.Id}_{item}_qty"];
                                        if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                            (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                            errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                    }
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                //get quantity entered by customer
                                var quantity = 1;
                                var quantityStr = form[$"product_attribute_{attribute.Id}_{selectedAttributeId}_qty"];
                                if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                    (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                    errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
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
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                            }
                            catch
                            {
                            }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid.TryParse(form[controlId], out Guid downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in productAttributes)
            {
                var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                }
            }
            return attributesXml;
        }

        protected virtual void SaveItem(ShoppingCartItem updatecartitem, List<string> addToCartWarnings, Product product,
           ShoppingCartType cartType, string attributes, decimal customerEnteredPriceConverted, DateTime? rentalStartDate,
           DateTime? rentalEndDate, int quantity)
        {
            if (updatecartitem == null)
            {
                //add to the cart
                addToCartWarnings.AddRange(_shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                    product, cartType, _storeContext.CurrentStore.Id,
                    attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity, true));
            }
            else
            {
                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, updatecartitem.ShoppingCartType, _storeContext.CurrentStore.Id);

                var otherCartItemWithSameParameters = _shoppingCartService.FindShoppingCartItemInTheCart(
                    cart, updatecartitem.ShoppingCartType, product, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate);
                if (otherCartItemWithSameParameters != null &&
                    otherCartItemWithSameParameters.Id == updatecartitem.Id)
                {
                    //ensure it's some other shopping cart item
                    otherCartItemWithSameParameters = null;
                }
                //update existing item
                addToCartWarnings.AddRange(_shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                    updatecartitem.Id, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity + (otherCartItemWithSameParameters?.Quantity ?? 0), true));
                if (otherCartItemWithSameParameters != null && !addToCartWarnings.Any())
                {
                    //delete the same shopping cart item (the other one)
                    _shoppingCartService.DeleteShoppingCartItem(otherCartItemWithSameParameters);
                }
            }
        }

        protected virtual IActionResult GetProductToCartDetails(List<string> addToCartWarnings, ShoppingCartType cartType,
            Product product)
        {
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart/wishlist
                //let's display warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToWishlist",
                            string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name), product);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

                        var updatetopwishlistsectionhtml = string.Format(
                            _localizationService.GetResource("Wishlist.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));

                        return Json(new
                        {
                            success = true,
                            message = string.Format(
                                _localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist.Link"),
                                Url.RouteUrl("Wishlist")),
                            updatetopwishlistsectionhtml
                        });
                    }
                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart",
                            string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name), product);

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

                        var updatetopcartsectionhtml = string.Format(
                            _localizationService.GetResource("ShoppingCart.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));

                        var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
                            ? RenderViewComponentToString("FlyoutShoppingCart")
                            : "";

                        return Json(new
                        {
                            success = true,
                            message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"),
                                Url.RouteUrl("ShoppingCart")),
                            updatetopcartsectionhtml,
                            updateflyoutcartsectionhtml
                        });
                    }
            }
        }

        #endregion

        #region Shopping cart

        //add product to cart using AJAX
        //currently we use this method on catalog pages (category/manufacturer/etc)
        [HttpPost]
        public virtual IActionResult AddProductToCart_Catalog(int productId, int shoppingCartTypeId,
            int quantity, bool forceredirection = false)
        {
            var cartType = (ShoppingCartType)shoppingCartTypeId;

            var product = _productService.GetProductById(productId);
            if (product == null)
                //no product found
                return Json(new
                {
                    success = false,
                    message = "No product found with the specified ID"
                });

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = _urlRecordService.GetSeName(product) })
                });
            }

            //products with "minimum order quantity" more than a specified qty
            if (product.OrderMinimumQuantity > quantity)
            {
                //we cannot add to the cart such products from category pages
                //it can confuse customers. That's why we redirect customers to the product details page
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = _urlRecordService.GetSeName(product) })
                });
            }

            if (product.CustomerEntersPrice)
            {
                //cannot be added to the cart (requires a customer to enter price)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = _urlRecordService.GetSeName(product) })
                });
            }

            if (product.IsRental)
            {
                //rental products require start/end dates to be entered
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = _urlRecordService.GetSeName(product) })
                });
            }

            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            if (allowedQuantities.Length > 0)
            {
                //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = _urlRecordService.GetSeName(product) })
                });
            }

            //allow a product to be added to the cart when all attributes are with "read-only checkboxes" type
            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            if (productAttributes.Any(pam => pam.AttributeControlType != AttributeControlType.ReadonlyCheckboxes))
            {
                //product has some attributes. let a customer see them
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = _urlRecordService.GetSeName(product) })
                });
            }

            //creating XML for "read-only checkboxes" attributes
            var attXml = productAttributes.Aggregate(string.Empty, (attributesXml, attribute) =>
            {
                var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                foreach (var selectedAttributeId in attributeValues
                    .Where(v => v.IsPreSelected)
                    .Select(v => v.Id)
                    .ToList())
                {
                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                        attribute, selectedAttributeId.ToString());
                }
                return attributesXml;
            });

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, cartType, _storeContext.CurrentStore.Id);
            var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(cart, cartType, product);
            //if we already have the same product in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
            var addToCartWarnings = _shoppingCartService
                .GetShoppingCartItemWarnings(_workContext.CurrentCustomer, cartType,
                product, _storeContext.CurrentStore.Id, string.Empty,
                decimal.Zero, null, null, quantityToValidate, false, shoppingCartItem?.Id ?? 0, true, false, false, false);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //let's display standard warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //now let's try adding product to the cart (now including product attribute validation, etc)
            addToCartWarnings = _shoppingCartService.AddToCart(customer: _workContext.CurrentCustomer,
                product: product,
                shoppingCartType: cartType,
                storeId: _storeContext.CurrentStore.Id,
                attributesXml: attXml,
                quantity: quantity);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the product details page
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = _urlRecordService.GetSeName(product) })
                });
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToWishlist",
                            string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name), product);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct || forceredirection)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

                        var updatetopwishlistsectionhtml = string.Format(_localizationService.GetResource("Wishlist.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));
                        return Json(new
                        {
                            success = true,
                            message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
                            updatetopwishlistsectionhtml
                        });
                    }
                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart",
                            string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name), product);

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct || forceredirection)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

                        var updatetopcartsectionhtml = string.Format(_localizationService.GetResource("ShoppingCart.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));

                        var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
                            ? this.RenderViewComponentToString("FlyoutShoppingCart")
                            : "";

                        return Json(new
                        {
                            success = true,
                            message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
                            updatetopcartsectionhtml,
                            updateflyoutcartsectionhtml
                        });
                    }
            }
        }

        //add product to cart using AJAX
        //currently we use this method on the product details pages
        [HttpPost]
        public virtual IActionResult AddProductToCart_Details(int productId, int shoppingCartTypeId, IFormCollection form)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("HomePage")
                });
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return Json(new
                {
                    success = false,
                    message = "Only simple products could be added to the cart"
                });
            }

            //update existing shopping cart item
            var updatecartitemid = 0;
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"addtocart_{productId}.UpdatedShoppingCartItemId", StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out updatecartitemid);
                    break;
                }
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                //search with the same cart type as specified
                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, (ShoppingCartType)shoppingCartTypeId, _storeContext.CurrentStore.Id);

                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found? let's ignore it. in this case we'll add a new item
                //if (updatecartitem == null)
                //{
                //    return Json(new
                //    {
                //        success = false,
                //        message = "No shopping cart item found to update"
                //    });
                //}
                //is it this product?
                if (updatecartitem != null && product.Id != updatecartitem.ProductId)
                {
                    return Json(new
                    {
                        success = false,
                        message = "This product does not match a passed shopping cart item identifier"
                    });
                }
            }

            //customer entered price
            var customerEnteredPriceConverted = decimal.Zero;
            if (product.CustomerEntersPrice)
            {
                foreach (var formKey in form.Keys)
                {
                    if (formKey.Equals($"addtocart_{productId}.CustomerEnteredPrice", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (decimal.TryParse(form[formKey], out decimal customerEnteredPrice))
                            customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
                        break;
                    }
                }
            }

            //quantity
            var quantity = 1;
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"addtocart_{productId}.EnteredQuantity", StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out quantity);
                    break;
                }

            var addToCartWarnings = new List<string>();

            //product and gift card attributes
            var attributes = ParseProductAttributes(product, form, addToCartWarnings);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            var cartType = updatecartitem == null ? (ShoppingCartType)shoppingCartTypeId :
                //if the item to update is found, then we ignore the specified "shoppingCartTypeId" parameter
                updatecartitem.ShoppingCartType;

            SaveItem(updatecartitem, addToCartWarnings, product, cartType, attributes, customerEnteredPriceConverted, rentalStartDate, rentalEndDate, quantity);

            //return result
            return GetProductToCartDetails(addToCartWarnings, cartType, product);
        }

        //handle product attribute selection event. this way we return new price, overridden gtin/sku/mpn
        //currently we use this method on the product details pages
        [HttpPost]
        public virtual IActionResult ProductDetails_AttributeChange(int productId, bool validateAttributeConditions,
            bool loadPicture, IFormCollection form)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return new NullJsonResult();

            var errors = new List<string>();
            var attributeXml = ParseProductAttributes(product, form, errors);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            //sku, mpn, gtin
            var sku = _productService.FormatSku(product, attributeXml);
            var mpn = _productService.FormatMpn(product, attributeXml);
            var gtin = _productService.FormatGtin(product, attributeXml);

            // calculating weight adjustment
            var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributeXml);
            var totalWeight = product.BasepriceAmount;

            foreach (var attributeValue in attributeValues)
            {
                switch (attributeValue.AttributeValueType)
                {
                    case AttributeValueType.Simple:
                        //simple attribute
                        totalWeight += attributeValue.WeightAdjustment;
                        break;
                    case AttributeValueType.AssociatedToProduct:
                        //bundled product
                        var associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
                        if (associatedProduct != null)
                            totalWeight += associatedProduct.BasepriceAmount * attributeValue.Quantity;
                        break;
                }
            }

            //price
            var price = "";
            //base price
            var basepricepangv = "";
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices) && !product.CustomerEntersPrice)
            {
                //we do not calculate price of "customer enters price" option is enabled
                List<DiscountForCaching> scDiscounts;
                var finalPrice = _priceCalculationService.GetUnitPrice(product,
                    _workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart,
                    1, attributeXml, 0,
                    rentalStartDate, rentalEndDate,
                    true, out decimal _, out scDiscounts);
                var finalPriceWithDiscountBase = _taxService.GetProductPrice(product, finalPrice, out decimal _);
                var finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);
                price = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                basepricepangv = _priceFormatter.FormatBasePrice(product, finalPriceWithDiscountBase, totalWeight);
            }

            //stock
            var stockAvailability = _productService.FormatStockMessage(product, attributeXml);

            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                foreach (var attribute in attributes)
                {
                    var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributeXml);
                    if (conditionMet.HasValue)
                    {
                        if (conditionMet.Value)
                            enabledAttributeMappingIds.Add(attribute.Id);
                        else
                            disabledAttributeMappingIds.Add(attribute.Id);
                    }
                }
            }

            //picture. used when we want to override a default product picture when some attribute is selected
            var pictureFullSizeUrl = string.Empty;
            var pictureDefaultSizeUrl = string.Empty;
            if (loadPicture)
            {
                //first, try to get product attribute combination picture
                var pictureId = _productAttributeParser.FindProductAttributeCombination(product, attributeXml)?.PictureId ?? 0;

                //then, let's see whether we have attribute values with pictures
                if (pictureId == 0)
                {
                    pictureId = _productAttributeParser.ParseProductAttributeValues(attributeXml)
                        .FirstOrDefault(attributeValue => attributeValue.PictureId > 0)?.PictureId ?? 0;
                }

                if (pictureId > 0)
                {
                    var productAttributePictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCTATTRIBUTE_PICTURE_MODEL_KEY,
                        pictureId, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    var pictureModel = _cacheManager.Get(productAttributePictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(pictureId);
                        return picture == null ? new PictureModel() : new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                            ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductDetailsPictureSize)
                        };
                    });
                    pictureFullSizeUrl = pictureModel.FullSizeImageUrl;
                    pictureDefaultSizeUrl = pictureModel.ImageUrl;
                }

            }

            var isFreeShipping = product.IsFreeShipping;
            if (isFreeShipping && !string.IsNullOrEmpty(attributeXml))
            {
                isFreeShipping = _productAttributeParser.ParseProductAttributeValues(attributeXml)
                    .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    .Select(attributeValue => _productService.GetProductById(attributeValue.AssociatedProductId))
                    .All(associatedProduct => associatedProduct == null || !associatedProduct.IsShipEnabled || associatedProduct.IsFreeShipping);
            }

            return Json(new
            {
                gtin,
                mpn,
                sku,
                price,
                basepricepangv,
                stockAvailability,
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
                pictureFullSizeUrl,
                pictureDefaultSizeUrl,
                isFreeShipping,
                message = errors.Any() ? errors.ToArray() : null
            });
        }

        [HttpPost]
        public virtual IActionResult CheckoutAttributeChange(IFormCollection form, bool isEditable)
        {
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);
            
            //save selected attributes
            ParseAndSaveCheckoutAttributes(cart, form);
            var attributeXml = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);

            //conditions
            var enabledAttributeIds = new List<int>();
            var disabledAttributeIds = new List<int>();
            var excludeShippableAttributes = !_shoppingCartService.ShoppingCartRequiresShipping(cart);
            var attributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, excludeShippableAttributes);
            foreach (var attribute in attributes)
            {
                var conditionMet = _checkoutAttributeParser.IsConditionMet(attribute, attributeXml);
                if (conditionMet.HasValue)
                {
                    if (conditionMet.Value)
                        enabledAttributeIds.Add(attribute.Id);
                    else
                        disabledAttributeIds.Add(attribute.Id);
                }
            }

            //update blocks
            var ordetotalssectionhtml = this.RenderViewComponentToString("OrderTotals", new { isEditable });
            var selectedcheckoutattributesssectionhtml = this.RenderViewComponentToString("SelectedCheckoutAttributes");

            return Json(new
            {
                ordetotalssectionhtml,
                selectedcheckoutattributesssectionhtml,
                enabledattributeids = enabledAttributeIds.ToArray(),
                disabledattributeids = disabledAttributeIds.ToArray()
            });
        }

        [HttpPost]
        public virtual IActionResult UploadFileProductAttribute(int attributeId)
        {
            var attribute = _productAttributeService.GetProductAttributeMappingById(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty
                });
            }

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }

            var fileBinary = _downloadService.GetDownloadBits(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = _fileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = _fileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty
                    });
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid
            });
        }

        [HttpPost]
        public virtual IActionResult UploadFileCheckoutAttribute(int attributeId)
        {
            var attribute = _checkoutAttributeService.GetCheckoutAttributeById(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty
                });
            }

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }

            var fileBinary = _downloadService.GetDownloadBits(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = _fileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = _fileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty
                    });
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid
            });
        }

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult Cart()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);
            var model = new ShoppingCartModel();
            model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart);
            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("updatecart")]
        public virtual IActionResult UpdateCart(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            //get identifiers of items to remove
            var itemIdsToRemove = form["removefromcart"]
                .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(idString => int.TryParse(idString, out int id) ? id : 0)
                .Distinct().ToList();

            //get order items with changed quantity
            var itemsWithNewQuantity = cart.Select(item => new
            {
                //try to get a new quantity for the item, set 0 for items to remove
                NewQuantity = itemIdsToRemove.Contains(item.Id) ? 0 : int.TryParse(form[$"itemquantity{item.Id}"], out int quantity) ? quantity : item.Quantity,
                Item = item
            }).Where(item => item.NewQuantity != item.Item.Quantity);

            //order cart items
            //first should be items with a reduced quantity and that require other products; or items with an increased quantity and are required for other products
            var orderedCart = itemsWithNewQuantity
                .OrderByDescending(cartItem =>
                    (cartItem.NewQuantity < cartItem.Item.Quantity && (cartItem.Item.Product?.RequireOtherProducts ?? false)) ||
                    (cartItem.NewQuantity > cartItem.Item.Quantity &&
                        cart.Any(item => item.Product != null && item.Product.RequireOtherProducts && _productService.ParseRequiredProductIds(item.Product).Contains(cartItem.Item.ProductId))))
                .ToList();

            //try to update cart items with new quantities and get warnings
            var warnings = orderedCart.Select(cartItem => new
            {
                ItemId = cartItem.Item.Id,
                Warnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                    cartItem.Item.Id, cartItem.Item.AttributesXml, cartItem.Item.CustomerEnteredPrice,
                    cartItem.Item.RentalStartDateUtc, cartItem.Item.RentalEndDateUtc, cartItem.NewQuantity, true)
            }).ToList();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);

            //updated cart
            cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            //prepare model
            var model = new ShoppingCartModel();
            model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart);

            //update current warnings
            foreach (var warningItem in warnings.Where(warningItem => warningItem.Warnings.Any()))
            {
                //find shopping cart item model to display appropriate warnings
                var itemModel = model.Items.FirstOrDefault(item => item.Id == warningItem.ItemId);
                if (itemModel != null)
                    itemModel.Warnings = warningItem.Warnings.Concat(itemModel.Warnings).Distinct().ToList();
            }

            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("continueshopping")]
        public virtual IActionResult ContinueShopping()
        {
            var returnUrl = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.LastContinueShoppingPageAttribute, _storeContext.CurrentStore.Id);
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToRoute("HomePage");
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("checkout")]
        public virtual IActionResult StartCheckout(IFormCollection form)
        {
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);

            //validate attributes
            var checkoutAttributes = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);
            var checkoutAttributeWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributes, true);
            if (checkoutAttributeWarnings.Any())
            {
                //something wrong, redisplay the page with warnings
                var model = new ShoppingCartModel();
                model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart, validateCheckoutAttributes: true);
                return View(model);
            }

            var anonymousPermissed = _orderSettings.AnonymousCheckoutAllowed 
                                     && _customerSettings.UserRegistrationType == UserRegistrationType.Disabled;

            if (anonymousPermissed || !_workContext.CurrentCustomer.IsGuest())
                return RedirectToRoute("Checkout");
            
            var downloadableProductsRequireRegistration =
                _customerSettings.RequireRegistrationForDownloadableProducts && cart.Any(sci => sci.Product.IsDownload);

            if (!_orderSettings.AnonymousCheckoutAllowed || downloadableProductsRequireRegistration)
            {
                //verify user identity (it may be facebook login page, or google, or local)
                return Challenge();
            }

            return RedirectToRoute("LoginCheckoutAsGuest", new { returnUrl = Url.RouteUrl("ShoppingCart") });
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applydiscountcouponcode")]
        public virtual IActionResult ApplyDiscountCoupon(string discountcouponcode, IFormCollection form)
        {
            //trim
            if (discountcouponcode != null)
                discountcouponcode = discountcouponcode.Trim();

            //cart
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);

            var model = new ShoppingCartModel();
            if (!string.IsNullOrWhiteSpace(discountcouponcode))
            {
                //we find even hidden records here. this way we can display a user-friendly message if it's expired
                var discounts = _discountService.GetAllDiscountsForCaching(couponCode: discountcouponcode, showHidden: true)
                    .Where(d => d.RequiresCouponCode)
                    .ToList();
                if (discounts.Any())
                {
                    var userErrors = new List<string>();
                    var anyValidDiscount = discounts.Any(discount =>
                    {
                        var validationResult = _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer, new[] { discountcouponcode });
                        userErrors.AddRange(validationResult.Errors);

                        return validationResult.IsValid;
                    });

                    if (anyValidDiscount)
                    {
                        //valid
                        _customerService.ApplyDiscountCouponCode(_workContext.CurrentCustomer, discountcouponcode);
                        model.DiscountBox.Messages.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied"));
                        model.DiscountBox.IsApplied = true;
                    }
                    else
                    {
                        if (userErrors.Any())
                            //some user errors
                            model.DiscountBox.Messages = userErrors;
                        else
                            //general error text
                            model.DiscountBox.Messages.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                    }
                }
                else
                    //discount cannot be found
                    model.DiscountBox.Messages.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
            }
            else
                //empty coupon code
                model.DiscountBox.Messages.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));

            model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart);

            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applygiftcardcouponcode")]
        public virtual IActionResult ApplyGiftCard(string giftcardcouponcode, IFormCollection form)
        {
            //trim
            if (giftcardcouponcode != null)
                giftcardcouponcode = giftcardcouponcode.Trim();

            //cart
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);

            var model = new ShoppingCartModel();
            if (!_shoppingCartService.ShoppingCartIsRecurring(cart))
            {
                if (!string.IsNullOrWhiteSpace(giftcardcouponcode))
                {
                    var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: giftcardcouponcode).FirstOrDefault();
                    var isGiftCardValid = giftCard != null && _giftCardService.IsGiftCardValid(giftCard);
                    if (isGiftCardValid)
                    {
                        _customerService.ApplyGiftCardCouponCode(_workContext.CurrentCustomer, giftcardcouponcode);
                        model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.Applied");
                        model.GiftCardBox.IsApplied = true;
                    }
                    else
                    {
                        model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                        model.GiftCardBox.IsApplied = false;
                    }
                }
                else
                {
                    model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                    model.GiftCardBox.IsApplied = false;
                }
            }
            else
            {
                model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts");
                model.GiftCardBox.IsApplied = false;
            }

            model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart);
            return View(model);
        }

        [PublicAntiForgery]
        [HttpPost]
        public virtual IActionResult GetEstimateShipping(int? countryId, int? stateProvinceId, string zipPostalCode, IFormCollection form)
        {
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);

            var errors = new StringBuilder();

            if (string.IsNullOrEmpty(zipPostalCode))
            {
                errors.Append(_localizationService.GetResource("ShoppingCart.EstimateShipping.ZipPostalCode.Required"));
            }

            if (countryId == null || countryId == 0)
            {
                if (errors.Length > 0)
                    errors.Append("<br>");

                errors.Append(_localizationService.GetResource("ShoppingCart.EstimateShipping.Country.Required"));
            }

            if (errors.Length > 0)
            {
                return Content(errors.ToString());
            }

            var model = _shoppingCartModelFactory.PrepareEstimateShippingResultModel(cart, countryId, stateProvinceId, zipPostalCode);
            return PartialView("_EstimateShippingResult", model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removediscount-")]
        public virtual IActionResult RemoveDiscountCoupon(IFormCollection form)
        {
            var model = new ShoppingCartModel();

            //get discount identifier
            var discountId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("removediscount-", StringComparison.InvariantCultureIgnoreCase))
                    discountId = Convert.ToInt32(formValue.Substring("removediscount-".Length));
            var discount = _discountService.GetDiscountById(discountId);
            if (discount != null)
                _customerService.RemoveDiscountCouponCode(_workContext.CurrentCustomer, discount.CouponCode);

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart);
            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removegiftcard-")]
        public virtual IActionResult RemoveGiftCardCode(IFormCollection form)
        {
            var model = new ShoppingCartModel();

            //get gift card identifier
            var giftCardId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("removegiftcard-", StringComparison.InvariantCultureIgnoreCase))
                    giftCardId = Convert.ToInt32(formValue.Substring("removegiftcard-".Length));
            var gc = _giftCardService.GetGiftCardById(giftCardId);
            if (gc != null)
                _customerService.RemoveGiftCardCouponCode(_workContext.CurrentCustomer, gc.GiftCardCouponCode);

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

            model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart);
            return View(model);
        }

        #endregion

        #region Wishlist

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult Wishlist(Guid? customerGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("HomePage");

            var customer = customerGuid.HasValue ?
                _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (customer == null)
                return RedirectToRoute("HomePage");

            var cart = _shoppingCartService.GetShoppingCart(customer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

            var model = new WishlistModel();
            model = _shoppingCartModelFactory.PrepareWishlistModel(model, cart, !customerGuid.HasValue);
            return View(model);
        }

        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired("updatecart")]
        public virtual IActionResult UpdateWishlist(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("HomePage");

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

            var allIdsToRemove = form.ContainsKey("removefromcart")
                ? form["removefromcart"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                var remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _shoppingCartService.DeleteShoppingCartItem(sci);
                else
                {
                    foreach (var formKey in form.Keys)
                        if (formKey.Equals($"itemquantity{sci.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            int newQuantity;
                            if (int.TryParse(form[formKey], out newQuantity))
                            {
                                var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                    sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                    newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }
                            break;
                        }
                }
            }

            //updated wishlist
            cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);
            var model = new WishlistModel();
            model = _shoppingCartModelFactory.PrepareWishlistModel(model, cart);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!sciModel.Warnings.Contains(w))
                            sciModel.Warnings.Add(w);
            }
            return View(model);
        }

        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired("addtocartbutton")]
        public virtual IActionResult AddItemsToCartFromWishlist(Guid? customerGuid, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("HomePage");

            var pageCustomer = customerGuid.HasValue
                ? _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (pageCustomer == null)
                return RedirectToRoute("HomePage");

            var pageCart = _shoppingCartService.GetShoppingCart(pageCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

            var allWarnings = new List<string>();
            var numberOfAddedItems = 0;
            var allIdsToAdd = form.ContainsKey("addtocart")
                ? form["addtocart"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                : new List<int>();
            foreach (var sci in pageCart)
            {
                if (allIdsToAdd.Contains(sci.Id))
                {
                    var warnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                        sci.Product, ShoppingCartType.ShoppingCart,
                        _storeContext.CurrentStore.Id,
                        sci.AttributesXml, sci.CustomerEnteredPrice,
                        sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, true);
                    if (!warnings.Any())
                        numberOfAddedItems++;
                    if (_shoppingCartSettings.MoveItemsFromWishlistToCart && //settings enabled
                        !customerGuid.HasValue && //own wishlist
                        !warnings.Any()) //no warnings ( already in the cart)
                    {
                        //let's remove the item from wishlist
                        _shoppingCartService.DeleteShoppingCartItem(sci);
                    }
                    allWarnings.AddRange(warnings);
                }
            }

            if (numberOfAddedItems > 0)
            {
                //redirect to the shopping cart page

                if (allWarnings.Any())
                {
                    _notificationService.ErrorNotification(_localizationService.GetResource("Wishlist.AddToCart.Error"));
                }

                return RedirectToRoute("ShoppingCart");
            }
            //no items added. redisplay the wishlist page

            if (allWarnings.Any())
            {
                _notificationService.ErrorNotification(_localizationService.GetResource("Wishlist.AddToCart.Error"));
            }

            var cart = _shoppingCartService.GetShoppingCart(pageCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

            var model = new WishlistModel();
            model = _shoppingCartModelFactory.PrepareWishlistModel(model, cart, !customerGuid.HasValue);
            return View(model);
        }

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult EmailWishlist()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) || !_shoppingCartSettings.EmailWishlistEnabled)
                return RedirectToRoute("HomePage");

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

            if (!cart.Any())
                return RedirectToRoute("HomePage");

            var model = new WishlistEmailAFriendModel();
            model = _shoppingCartModelFactory.PrepareWishlistEmailAFriendModel(model, false);
            return View(model);
        }

        [HttpPost, ActionName("EmailWishlist")]
        [PublicAntiForgery]
        [FormValueRequired("send-email")]
        [ValidateCaptcha]
        public virtual IActionResult EmailWishlistSend(WishlistEmailAFriendModel model, bool captchaValid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) || !_shoppingCartSettings.EmailWishlistEnabled)
                return RedirectToRoute("HomePage");

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id);

            if (!cart.Any())
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailWishlistToFriendPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            //check whether the current customer is guest and ia allowed to email wishlist
            if (_workContext.CurrentCustomer.IsGuest() && !_shoppingCartSettings.AllowAnonymousUsersToEmailWishlist)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Wishlist.EmailAFriend.OnlyRegisteredUsers"));
            }

            if (ModelState.IsValid)
            {
                //email
                _workflowMessageService.SendWishlistEmailAFriendMessage(_workContext.CurrentCustomer,
                        _workContext.WorkingLanguage.Id, model.YourEmailAddress,
                        model.FriendEmail, HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("Wishlist.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = _shoppingCartModelFactory.PrepareWishlistEmailAFriendModel(model, true);
            return View(model);
        }

        #endregion
    }
}