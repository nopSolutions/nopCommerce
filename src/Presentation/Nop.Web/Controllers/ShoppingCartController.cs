using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.UI.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;
using Nop.Services.Logging;

namespace Nop.Web.Controllers
{
    public partial class ShoppingCartController : BaseNopController
    {
		#region Fields

        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDiscountService _discountService;
        private readonly ICustomerService _customerService;
        private readonly IGiftCardService _giftCardService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IPaymentService _paymentService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly ICacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;

        private readonly MediaSettings _mediaSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly AddressSettings _addressSettings;

        #endregion

		#region Constructors

        public ShoppingCartController(IProductService productService, IWorkContext workContext,
            IShoppingCartService shoppingCartService, IPictureService pictureService,
            ILocalizationService localizationService, 
            IProductAttributeService productAttributeService, IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            ITaxService taxService, ICurrencyService currencyService, 
            IPriceCalculationService priceCalculationService, IPriceFormatter priceFormatter,
            ICheckoutAttributeParser checkoutAttributeParser, ICheckoutAttributeFormatter checkoutAttributeFormatter, 
            IOrderProcessingService orderProcessingService,
            IDiscountService discountService,ICustomerService customerService, 
            IGiftCardService giftCardService, ICountryService countryService,
            IStateProvinceService stateProvinceService, IShippingService shippingService, 
            IOrderTotalCalculationService orderTotalCalculationService,
            ICheckoutAttributeService checkoutAttributeService, IPaymentService paymentService,
            IWorkflowMessageService workflowMessageService,
            IPermissionService permissionService, 
            IDownloadService downloadService, ICacheManager cacheManager,
            IWebHelper webHelper, ICustomerActivityService customerActivityService,
            MediaSettings mediaSettings, ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings, OrderSettings orderSettings,
            ShippingSettings shippingSettings, TaxSettings taxSettings,
            CaptchaSettings captchaSettings, AddressSettings addressSettings)
        {
            this._productService = productService;
            this._workContext = workContext;
            this._shoppingCartService = shoppingCartService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._productAttributeService = productAttributeService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._productAttributeParser = productAttributeParser;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._orderProcessingService = orderProcessingService;
            this._discountService = discountService;
            this._customerService = customerService;
            this._giftCardService = giftCardService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._paymentService = paymentService;
            this._workflowMessageService = workflowMessageService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
            this._cacheManager = cacheManager;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            
            this._mediaSettings = mediaSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._shippingSettings = shippingSettings;
            this._taxSettings = taxSettings;
            this._captchaSettings = captchaSettings;
            this._addressSettings = addressSettings;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected PictureModel PrepareCartItemPictureModel(ProductVariant productVariant,
            int pictureSize, bool showDefaultPicture, string productName)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            var pictureCacheKey = string.Format(ModelCacheEventConsumer.CART_PICTURE_MODEL_KEY, productVariant.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured());
            var model = _cacheManager.Get(pictureCacheKey, () =>
            {
                //first try to load product variant piture
                var picture = _pictureService.GetPictureById(productVariant.PictureId);
                if (picture == null)
                {
                    //if product variant doesn't have any picture assigned, then load product picture
                    picture = _pictureService.GetPicturesByProductId(productVariant.Product.Id, 1).FirstOrDefault();
                }
                return new PictureModel()
                {
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize, showDefaultPicture),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), productName),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), productName),
                };
            });
            return model;
        }

        /// <summary>
        /// Prepare shopping cart model
        /// </summary>
        /// <param name="model">Model instance</param>
        /// <param name="cart">Shopping cart</param>
        /// <param name="isEditable">A value indicating whether cart is editable</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether we should validate checkout attributes when preparing the model</param>
        /// <param name="prepareEstimateShippingIfEnabled">A value indicating whether we should prepare "Estimate shipping" model</param>
        /// <param name="setEstimateShippingDefaultAddress">A value indicating whether we should prefill "Estimate shipping" model with the default customer address</param>
        /// <param name="prepareAndDisplayOrderReviewData">A value indicating whether we should prepare review data (such as billing/shipping address, payment or shipping data entered during checkout)</param>
        /// <returns>Model</returns>
        [NonAction]
        protected void PrepareShoppingCartModel(ShoppingCartModel model, 
            IList<ShoppingCartItem> cart, bool isEditable = true, 
            bool validateCheckoutAttributes = false, 
            bool prepareEstimateShippingIfEnabled = true, bool setEstimateShippingDefaultAddress = true,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            if (cart.Count == 0)
                return;
            
            #region Simple properties

            model.IsEditable = isEditable;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowProductSku;
            model.CheckoutAttributeInfo = _checkoutAttributeFormatter.FormatAttributes(_workContext.CurrentCustomer.CheckoutAttributes, _workContext.CurrentCustomer);
            bool minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
            if (!minOrderSubtotalAmountOk)
            {
                decimal minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, _workContext.WorkingCurrency);
                model.MinOrderSubtotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"), _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false));
            }
            model.TermsOfServiceEnabled = _orderSettings.TermsOfServiceEnabled;

            //gift card and gift card boxes
            model.DiscountBox.Display= _shoppingCartSettings.ShowDiscountBox;
            var discount = _discountService.GetDiscountByCouponCode(_workContext.CurrentCustomer.DiscountCouponCode);
            if (discount != null &&
                discount.RequiresCouponCode &&
                _discountService.IsDiscountValid(discount, _workContext.CurrentCustomer))
                model.DiscountBox.CurrentCode = discount.CouponCode;
            model.GiftCardBox.Display = _shoppingCartSettings.ShowGiftCardBox;

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, _workContext.CurrentCustomer.CheckoutAttributes, validateCheckoutAttributes);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);
            
            #endregion

            #region Checkout attributes

            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes();
            if (!cart.RequiresShipping())
            {
                //remove attributes which require shippable products
                checkoutAttributes = checkoutAttributes.RemoveShippableAttributes();
            }
            foreach (var attribute in checkoutAttributes)
            {
                var caModel = new ShoppingCartModel.CheckoutAttributeModel()
                {
                    Id = attribute.Id,
                    Name = attribute.GetLocalized(x => x.Name),
                    TextPrompt = attribute.GetLocalized(x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var caValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                    foreach (var caValue in caValues)
                    {
                        var pvaValueModel = new ShoppingCartModel.CheckoutAttributeValueModel()
                        {
                            Id = caValue.Id,
                            Name = caValue.GetLocalized(x => x.Name),
                            IsPreSelected = caValue.IsPreSelected
                        };
                        caModel.Values.Add(pvaValueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            decimal priceAdjustmentBase = _taxService.GetCheckoutAttributePrice(caValue);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                pvaValueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                pvaValueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment);
                        }
                    }
                }



                //set already selected attributes
                string selectedCheckoutAttributes = _workContext.CurrentCustomer.CheckoutAttributes;
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                //clear default selection
                                foreach (var item in caModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedCaValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                                foreach (var caValue in selectedCaValues)
                                    foreach (var item in caModel.Values)
                                        if (caValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var enteredText = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                                if (enteredText.Count > 0)
                                    caModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            //keep in mind my that the code below works only in the current culture
                            var selectedDateStr = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                            if (selectedDateStr.Count > 0)
                            {
                                DateTime selectedDate;
                                if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                       DateTimeStyles.None, out selectedDate))
                                {
                                    //successfully parsed
                                    caModel.SelectedDay = selectedDate.Day;
                                    caModel.SelectedMonth = selectedDate.Month;
                                    caModel.SelectedYear = selectedDate.Year;
                                }
                            }
                            
                        }
                        break;
                    default:
                        break;
                }

                model.CheckoutAttributes.Add(caModel);
            }

            #endregion 

            #region Estimate shipping

            if (prepareEstimateShippingIfEnabled)
            {
                model.EstimateShipping.Enabled = cart.Count > 0 && cart.RequiresShipping() && _shippingSettings.EstimateShippingEnabled;
                if (model.EstimateShipping.Enabled)
                {
                    //countries
                    int? defaultEstimateCountryId = (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null) ? _workContext.CurrentCustomer.ShippingAddress.CountryId : model.EstimateShipping.CountryId;
                    model.EstimateShipping.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                    foreach (var c in _countryService.GetAllCountriesForShipping())
                        model.EstimateShipping.AvailableCountries.Add(new SelectListItem()
                        {
                            Text = c.GetLocalized(x => x.Name),
                            Value = c.Id.ToString(),
                            Selected = c.Id == defaultEstimateCountryId
                        });
                    //states
                    int? defaultEstimateStateId = (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null) ? _workContext.CurrentCustomer.ShippingAddress.StateProvinceId : model.EstimateShipping.StateProvinceId;
                    var states = defaultEstimateCountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(defaultEstimateCountryId.Value).ToList() : new List<StateProvince>();
                    if (states.Count > 0)
                        foreach (var s in states)
                            model.EstimateShipping.AvailableStates.Add(new SelectListItem()
                            {
                                Text = s.GetLocalized(x => x.Name),
                                Value = s.Id.ToString(),
                                Selected = s.Id == defaultEstimateStateId
                            });
                    else
                        model.EstimateShipping.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "0" });

                    if (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null)
                        model.EstimateShipping.ZipPostalCode = _workContext.CurrentCustomer.ShippingAddress.ZipPostalCode;
                }
            }

            #endregion

            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel()
                {
                    Id = sci.Id,
                    Sku = sci.ProductVariant.Sku,
                    ProductId = sci.ProductVariant.ProductId,
                    ProductSeName = sci.ProductVariant.Product.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.ProductVariant, sci.AttributesXml),
                };

                //allowed quantities
                var allowedQuantities = sci.ProductVariant.ParseAllowedQuatities();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem()
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }
                
                //recurring info
                if (sci.ProductVariant.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), sci.ProductVariant.RecurringCycleLength, sci.ProductVariant.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //unit prices
                if (sci.ProductVariant.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate = decimal.Zero;
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetUnitPrice(sci, true), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.ProductVariant.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                    decimal taxRate = decimal.Zero;
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetSubTotal(sci, true), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    decimal shoppingCartItemSubTotalWithoutDiscountBase = _taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetSubTotal(sci, false), out taxRate);
                    decimal shoppingCartItemDiscountBase = shoppingCartItemSubTotalWithoutDiscountBase - shoppingCartItemSubTotalWithDiscountBase;
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                        cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                    }
                }

                //product name
                if (!String.IsNullOrEmpty(sci.ProductVariant.GetLocalized(x=>x.Name)))
                    cartItemModel.ProductName = string.Format("{0} ({1})",sci.ProductVariant.Product.GetLocalized(x=>x.Name), sci.ProductVariant.GetLocalized(x=>x.Name));
                else
                    cartItemModel.ProductName = sci.ProductVariant.Product.GetLocalized(x=>x.Name);
                
                //picture
                if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci.ProductVariant,
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
                }

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.ShoppingCartType,
                    sci.ProductVariant,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion

            #region Button payment methods

            var boundPaymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer.Id)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .ToList();
            foreach (var pm in boundPaymentMethods)
            {
                if (cart.IsRecurring() && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                pm.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);

                model.ButtonPaymentMethodActionNames.Add(actionName);
                model.ButtonPaymentMethodControllerNames.Add(controllerName);
                model.ButtonPaymentMethodRouteValues.Add(routeValues);
            }

            #endregion

            #region Order review data

            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData.Display = true;

                //billing info
                var billingAddress = _workContext.CurrentCustomer.BillingAddress;
                if (billingAddress != null)
                    model.OrderReviewData.BillingAddress.PrepareModel(billingAddress, false, _addressSettings);
               
                //shipping info
                if (cart.RequiresShipping())
                {
                    model.OrderReviewData.IsShippable = true;

                    var shippingAddress = _workContext.CurrentCustomer.ShippingAddress;
                    if (shippingAddress != null)
                        model.OrderReviewData.ShippingAddress.PrepareModel(shippingAddress, false, _addressSettings);
                    
                    //selected shipping method
                    var shippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.LastShippingOption);
                    if (shippingOption != null)
                        model.OrderReviewData.ShippingMethod = shippingOption.Name;
                }
                //payment info
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(_workContext.CurrentCustomer.SelectedPaymentMethodSystemName);
                model.OrderReviewData.PaymentMethod = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : "";
            }
            #endregion
        }

        [NonAction]
        protected void PrepareWishlistModel(WishlistModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.EmailWishlistEnabled = _shoppingCartSettings.EmailWishlistEnabled;
            model.IsEditable = isEditable;
            model.DisplayAddToCart = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);

            if (cart.Count == 0)
                return;

            #region Simple properties

            var customer = cart.FirstOrDefault().Customer;
            model.CustomerGuid = customer.CustomerGuid;
            model.CustomerFullname = customer.GetFullName();
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowProductSku;
            
            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, "", false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            #endregion
            
            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new WishlistModel.ShoppingCartItemModel()
                {
                    Id = sci.Id,
                    Sku = sci.ProductVariant.Sku,
                    ProductId = sci.ProductVariant.ProductId,
                    ProductSeName = sci.ProductVariant.Product.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.ProductVariant, sci.AttributesXml),
                };

                //allowed quantities
                var allowedQuantities = sci.ProductVariant.ParseAllowedQuatities();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem()
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }
                

                //recurring info
                if (sci.ProductVariant.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), sci.ProductVariant.RecurringCycleLength, sci.ProductVariant.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //unit prices
                if (sci.ProductVariant.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate = decimal.Zero;
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetUnitPrice(sci, true), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.ProductVariant.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                    decimal taxRate = decimal.Zero;
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetSubTotal(sci, true), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    decimal shoppingCartItemSubTotalWithoutDiscountBase = _taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetSubTotal(sci, false), out taxRate);
                    decimal shoppingCartItemDiscountBase = shoppingCartItemSubTotalWithoutDiscountBase - shoppingCartItemSubTotalWithDiscountBase;
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                        cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                    }
                }

                //product name
                if (!String.IsNullOrEmpty(sci.ProductVariant.GetLocalized(x => x.Name)))
                    cartItemModel.ProductName = string.Format("{0} ({1})", sci.ProductVariant.Product.GetLocalized(x => x.Name), sci.ProductVariant.GetLocalized(x => x.Name));
                else
                    cartItemModel.ProductName = sci.ProductVariant.Product.GetLocalized(x => x.Name);

                //picture
                if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci.ProductVariant,
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
                }

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(_workContext.CurrentCustomer,
                            sci.ShoppingCartType,
                            sci.ProductVariant,
                            sci.AttributesXml,
                            sci.CustomerEnteredPrice,
                            sci.Quantity, false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion
        }

        [NonAction]
        protected MiniShoppingCartModel PrepareMiniShoppingCartModel()
        {
            var model = new MiniShoppingCartModel()
            {
                ShowProductImages = _shoppingCartSettings.ShowProductImagesInMiniShoppingCart,
                //let's always display it
                DisplayShoppingCartButton = true,
                CurrentCustomerIsGuest = _workContext.CurrentCustomer.IsGuest(),
                AnonymousCheckoutAllowed = _orderSettings.AnonymousCheckoutAllowed,
            };

            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            model.TotalProducts = cart.GetTotalProducts();
            if (cart.Count > 0)
            {
                //subtotal
                decimal subtotalBase = decimal.Zero;
                decimal orderSubTotalDiscountAmountBase = decimal.Zero;
                Discount orderSubTotalAppliedDiscount = null;
                decimal subTotalWithoutDiscountBase = decimal.Zero;
                decimal subTotalWithDiscountBase = decimal.Zero;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscount,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                subtotalBase = subTotalWithoutDiscountBase;
                decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal);

                //a customer should visit the shopping cart page before going to checkout if:
                //1. "terms of services" are enabled
                //2. we have at least one checkout attribute
                //3. min order sub-total is OK
                var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes();
                if (!cart.RequiresShipping())
                {
                    //remove attributes which require shippable products
                    checkoutAttributes = checkoutAttributes.RemoveShippableAttributes();
                }
                bool minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
                model.DisplayCheckoutButton = !_orderSettings.TermsOfServiceEnabled && 
                    checkoutAttributes.Count == 0 &&
                    minOrderSubtotalAmountOk;

                //products. sort descending (recently added products)
                foreach (var sci in cart
                    .OrderByDescending(x => x.Id)
                    .Take(_shoppingCartSettings.MiniShoppingCartProductNumber)
                    .ToList())
                {
                    var cartItemModel = new MiniShoppingCartModel.ShoppingCartItemModel()
                    {
                        Id = sci.Id,
                        ProductId = sci.ProductVariant.ProductId,
                        ProductSeName = sci.ProductVariant.Product.GetSeName(),
                        Quantity = sci.Quantity,
                        AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.ProductVariant, sci.AttributesXml)
                    };

                    //product name
                    if (!String.IsNullOrEmpty(sci.ProductVariant.GetLocalized(x => x.Name)))
                        cartItemModel.ProductName = string.Format("{0} ({1})", sci.ProductVariant.Product.GetLocalized(x => x.Name), sci.ProductVariant.GetLocalized(x => x.Name));
                    else
                        cartItemModel.ProductName = sci.ProductVariant.Product.GetLocalized(x => x.Name);

                    //unit prices
                    if (sci.ProductVariant.CallForPrice)
                    {
                        cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                    }
                    else
                    {
                        decimal taxRate = decimal.Zero;
                        decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetUnitPrice(sci, true), out taxRate);
                        decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                        cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                    }

                    //picture
                    if (_shoppingCartSettings.ShowProductImagesInMiniShoppingCart)
                    {
                        cartItemModel.Picture = PrepareCartItemPictureModel(sci.ProductVariant,
                            _mediaSettings.MiniCartThumbPictureSize, true, cartItemModel.ProductName);
                    }

                    model.Items.Add(cartItemModel);
                }
            }
            
            return model;
        }

        [NonAction]
        protected void ParseAndSaveCheckoutAttributes(List<ShoppingCartItem> cart, FormCollection form)
        {
            string selectedAttributes = "";
            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes();
            if (!cart.RequiresShipping())
            {
                //remove attributes which require shippable products
                checkoutAttributes = checkoutAttributes.RemoveShippableAttributes();
            }
            foreach (var attribute in checkoutAttributes)
            {
                string controlId = string.Format("checkout_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                        {
                            var ddlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ddlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ddlAttributes);
                                if (selectedAttributeId > 0)
                                    selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.RadioList:
                        {
                            var rblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(rblAttributes))
                            {
                                int selectedAttributeId = int.Parse(rblAttributes);
                                if (selectedAttributeId > 0)
                                    selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                        {
                            var txtAttribute = form[controlId];
                            if (!String.IsNullOrEmpty(txtAttribute))
                            {
                                string enteredText = txtAttribute.Trim();
                                selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.MultilineTextbox:
                        {
                            var txtAttribute = form[controlId];
                            if (!String.IsNullOrEmpty(txtAttribute))
                            {
                                string enteredText = txtAttribute.Trim();
                                selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
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
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(date));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            var httpPostedFile = this.Request.Files[controlId];
                            if ((httpPostedFile != null) && (!String.IsNullOrEmpty(httpPostedFile.FileName)))
                            {
                                int fileMaxSize = _catalogSettings.FileUploadMaximumSizeBytes;
                                if (httpPostedFile.ContentLength > fileMaxSize)
                                {
                                    //TODO display warning
                                    //warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), (int)(fileMaxSize / 1024)));
                                }
                                else
                                {
                                    //save an uploaded file
                                    var download = new Download()
                                    {
                                        DownloadGuid = Guid.NewGuid(),
                                        UseDownloadUrl = false,
                                        DownloadUrl = "",
                                        DownloadBinary = httpPostedFile.GetDownloadBits(),
                                        ContentType = httpPostedFile.ContentType,
                                        Filename = System.IO.Path.GetFileNameWithoutExtension(httpPostedFile.FileName),
                                        Extension = System.IO.Path.GetExtension(httpPostedFile.FileName),
                                        IsNew = true
                                    };
                                    _downloadService.InsertDownload(download);
                                    //save attribute
                                    selectedAttributes = _checkoutAttributeParser.AddCheckoutAttribute(selectedAttributes,
                                        attribute, download.DownloadGuid.ToString());
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //save checkout attributes
            _workContext.CurrentCustomer.CheckoutAttributes = selectedAttributes;
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);
        }

        #endregion

        #region Shopping cart
        
        //add product (not product variant) to cart using AJAX
        //currently we use this method on catalog pages (category/manufacturer/etc)
        [HttpPost]
        public ActionResult AddProductToCart(int productId, bool forceredirection = false)
        {
            //current we support only ShoppingCartType.ShoppingCart
            const ShoppingCartType shoppingCartType = ShoppingCartType.ShoppingCart;

            var product = _productService.GetProductById(productId);
            if (product == null)
                //no product found
                return Json(new
                {
                    success = false,
                    message = "No product found with the specified ID"
                });

            var productVariants = _productService.GetProductVariantsByProductId(productId);
            if (productVariants.Count != 1)
            {
                //we can add a product to the cart only if it has exactly one product variant
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            //get default product variant
            var defaultProductVariant = productVariants[0];
            if (defaultProductVariant.CustomerEntersPrice)
            {
                //cannot be added to the cart (requires a customer to enter price)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            //quantity to add
            var qtyToAdd = defaultProductVariant.OrderMinimumQuantity > 0 ? 
                defaultProductVariant.OrderMinimumQuantity : 1;
            
            var allowedQuantities = defaultProductVariant.ParseAllowedQuatities();
            if (allowedQuantities.Length > 0)
            {
                //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cart = _workContext
                .CurrentCustomer
                .ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == shoppingCartType)
                .ToList();
            var shoppingCartItem = _shoppingCartService
                .FindShoppingCartItemInTheCart(cart, shoppingCartType, defaultProductVariant);
            //if we already have the same product variant in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ?
                shoppingCartItem.Quantity + qtyToAdd : qtyToAdd;
            var addToCartWarnings = _shoppingCartService
                .GetShoppingCartItemWarnings(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart,
                defaultProductVariant, string.Empty, decimal.Zero, quantityToValidate, false, true, false, false, false);
            if (addToCartWarnings.Count > 0)
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
            addToCartWarnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                defaultProductVariant, ShoppingCartType.ShoppingCart,
                string.Empty, decimal.Zero, qtyToAdd, true);
            if (addToCartWarnings.Count > 0)
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the product details page
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            //now product is in the cart

            //activity log
            _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), defaultProductVariant.FullProductName);

            if (_shoppingCartSettings.DisplayCartAfterAddingProduct ||
                forceredirection)
            {
                //redirect to the shopping cart page
                return Json(new
                {
                    redirect = Url.RouteUrl("ShoppingCart"),
                });
            }

            //display notification message and update appropriate blocks
            var updatetopcartsectionhtml = string.Format("({0})",
                 _workContext
                 .CurrentCustomer
                 .ShoppingCartItems
                 .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                 .ToList()
                 .GetTotalProducts());
            var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
                ? this.RenderPartialViewToString("FlyoutShoppingCart", PrepareMiniShoppingCartModel())
                : "";

            return Json(new
            {
                success = true,
                message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
                updatetopcartsectionhtml = updatetopcartsectionhtml,
                updateflyoutcartsectionhtml = updateflyoutcartsectionhtml,
            });

        }

        //add product variant to cart using AJAX
        //currently we use this method only for desktop version
        //mobile version uses HTTP POST version of this method (CatalogController.AddProductVariantToCart)
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddProductVariantToCart(int productVariantId, int shoppingCartTypeId, FormCollection form)
        {
            var productVariant = _productService.GetProductVariantById(productVariantId);
            if (productVariant == null)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("HomePage"),
                });
            }

            #region Customer entered price
            decimal customerEnteredPriceConverted = decimal.Zero;
            if (productVariant.CustomerEntersPrice)
            {
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("addtocart_{0}.CustomerEnteredPrice", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        decimal customerEnteredPrice = decimal.Zero;
                        if (decimal.TryParse(form[formKey], out customerEnteredPrice))
                            customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
                        break;
                    }
                }
            }
            #endregion

            #region Quantity

            int quantity = 1;
            foreach (string formKey in form.AllKeys)
                if (formKey.Equals(string.Format("addtocart_{0}.EnteredQuantity", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out quantity);
                    break;
                }

            #endregion

            var addToCartWarnings = new List<string>();
            string attributes = "";

            #region Product attributes
            string selectedAttributes = string.Empty;
            var productVariantAttributes = _productAttributeService.GetProductVariantAttributesByProductVariantId(productVariant.Id);
            foreach (var attribute in productVariantAttributes)
            {
                string controlId = string.Format("product_attribute_{0}_{1}_{2}", attribute.ProductVariantId, attribute.ProductAttributeId, attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                        {
                            var ddlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ddlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ddlAttributes);
                                if (selectedAttributeId > 0)
                                    selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.RadioList:
                        {
                            var rblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(rblAttributes))
                            {
                                int selectedAttributeId = int.Parse(rblAttributes);
                                if (selectedAttributeId > 0)
                                    selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                        {
                            var txtAttribute = form[controlId];
                            if (!String.IsNullOrEmpty(txtAttribute))
                            {
                                string enteredText = txtAttribute.Trim();
                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.MultilineTextbox:
                        {
                            var txtAttribute = form[controlId];
                            if (!String.IsNullOrEmpty(txtAttribute))
                            {
                                string enteredText = txtAttribute.Trim();
                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
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
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                        attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            attributes = selectedAttributes;

            #endregion

            #region Gift cards

            if (productVariant.IsGiftCard)
            {
                string recipientName = "";
                string recipientEmail = "";
                string senderName = "";
                string senderEmail = "";
                string giftCardMessage = "";
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientName", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientEmail", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderName", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderEmail", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.Message", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                attributes = _productAttributeParser.AddGiftCardAttribute(attributes,
                    recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
            }

            #endregion

            //save item
            var cartType = (ShoppingCartType)shoppingCartTypeId;
            addToCartWarnings.AddRange(_shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                productVariant, cartType, attributes, customerEnteredPriceConverted, quantity, true));

            #region Return result

            if (addToCartWarnings.Count > 0)
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
                        _customerActivityService.InsertActivity("PublicStore.AddToWishlist", _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), productVariant.FullProductName);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist"),
                            });
                        }
                        else
                        {
                            //display notification message and update appropriate blocks
                            var updatetopwishlistsectionhtml = string.Format("({0})",
                                 _workContext
                                 .CurrentCustomer
                                 .ShoppingCartItems
                                 .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                                 .ToList()
                                 .GetTotalProducts());
                            return Json(new
                            {
                                success = true,
                                message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
                                updatetopwishlistsectionhtml = updatetopwishlistsectionhtml,
                            });
                        }
                    }
                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), productVariant.FullProductName);

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart"),
                            });
                        }
                        else
                        {

                            //display notification message and update appropriate blocks
                            var updatetopcartsectionhtml = string.Format("({0})",
                                 _workContext
                                 .CurrentCustomer
                                 .ShoppingCartItems
                                 .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                                 .ToList()
                                 .GetTotalProducts());
                            var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
                                ? this.RenderPartialViewToString("FlyoutShoppingCart", PrepareMiniShoppingCartModel())
                                : "";

                            return Json(new
                            {
                                success = true,
                                message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
                                updatetopcartsectionhtml = updatetopcartsectionhtml,
                                updateflyoutcartsectionhtml = updateflyoutcartsectionhtml
                            });
                        }
                    }
            }


            #endregion
        }

        [HttpPost]
        public ActionResult UploadFileProductAttribute(int productVariantId, int productAttributeId)
        {
            var productVariant = _productService.GetProductVariantById(productVariantId);
            if (productVariant == null ||
                !productVariant.Published ||
                productVariant.Deleted ||
                productVariant.Product == null ||
                !productVariant.Product.Published ||
                productVariant.Product.Deleted)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty,
                }, "text/plain");
            }
            //ensure that this attribute belong to this product variant and has "file upload" type
            var pva = _productAttributeService
                .GetProductVariantAttributesByProductVariantId(productVariantId)
                .Where(pa => pa.ProductAttributeId == productAttributeId)
                .FirstOrDefault();
            if (pva == null || pva.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty,
                }, "text/plain");
            }

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            int fileMaxSize = _catalogSettings.FileUploadMaximumSizeBytes;
            if (fileBinary.Length > fileMaxSize)
            {
                //when returning JSON the mime-type must be set to text/plain
                //otherwise some browsers will pop-up a "Save As" dialog.
                return Json(new
                {
                    success = false,
                    message = string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), (int)(fileMaxSize / 1024)),
                    downloadGuid = Guid.Empty,
                }, "text/plain");
            }

            var download = new Download()
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
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
                downloadGuid = download.DownloadGuid,
            }, "text/plain");
        }


        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Cart()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = new ShoppingCartModel();
            PrepareShoppingCartModel(model, cart);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult OrderSummary(bool? prepareAndDisplayOrderReviewData)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = new ShoppingCartModel();
            PrepareShoppingCartModel(model, cart, 
                isEditable: false, 
                prepareEstimateShippingIfEnabled: false, 
                prepareAndDisplayOrderReviewData: prepareAndDisplayOrderReviewData.HasValue ? prepareAndDisplayOrderReviewData.Value : false);
            return PartialView(model);
        }

        //update all shopping cart items on the page
        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("updatecart")]
        public ActionResult UpdateCartAll(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            var allIdsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _shoppingCartService.DeleteShoppingCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                else
                {
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int newQuantity = sci.Quantity;
                            if (int.TryParse(form[formKey], out newQuantity))
                            {
                                var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                                    sci.Id, newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }
                            break;
                        }
                }
            }

            //updated cart
            cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = new ShoppingCartModel();
            PrepareShoppingCartModel(model, cart);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.Where(x => x.Id == sciId).FirstOrDefault();
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!sciModel.Warnings.Contains(w))
                            sciModel.Warnings.Add(w);
            }
            return View(model);
        }

        //update a certain shopping cart item on the page
        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "updatecartitem-")]
        public ActionResult UpdateCartItem(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            //get shopping cart item identifier
            int sciId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("updatecartitem-", StringComparison.InvariantCultureIgnoreCase))
                    sciId = Convert.ToInt32(formValue.Substring("updatecartitem-".Length));
            //get shopping cart item
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var sci = cart.Where(x => x.Id == sciId).FirstOrDefault();
            if (sci == null)
            {
                return RedirectToRoute("ShoppingCart");
            }

            //update the cart item
            var warnings = new List<string>();
            foreach (string formKey in form.AllKeys)
                if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                {
                    int newQuantity = sci.Quantity;
                    if (int.TryParse(form[formKey], out newQuantity))
                    {
                        warnings.AddRange(_shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                            sci.Id, newQuantity, true));
                    }
                    break;
                }
                

            //updated cart
            cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = new ShoppingCartModel();
            PrepareShoppingCartModel(model, cart);
            //update current warnings
            //find model
            var sciModel = model.Items.Where(x => x.Id == sciId).FirstOrDefault();
            if (sciModel != null)
                foreach (var w in warnings)
                    if (!sciModel.Warnings.Contains(w))
                        sciModel.Warnings.Add(w);
            return View(model);
        }

        //remove a certain shopping cart item on the page
        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removefromcart-")]
        public ActionResult RemoveCartItem(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            //get shopping cart item identifier
            int sciId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("removefromcart-", StringComparison.InvariantCultureIgnoreCase))
                    sciId = Convert.ToInt32(formValue.Substring("removefromcart-".Length));
            //get shopping cart item
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var sci = cart.Where(x => x.Id == sciId).FirstOrDefault();
            if (sci == null)
            {
                return RedirectToRoute("ShoppingCart");
            }

            //remove the cart item
            _shoppingCartService.DeleteShoppingCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);


            //updated cart
            cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = new ShoppingCartModel();
            PrepareShoppingCartModel(model, cart);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("continueshopping")]
        public ActionResult ContinueShopping()
        {
            string returnUrl = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.LastContinueShoppingPage);
            if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToRoute("HomePage");
            }
        }
        
        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("startcheckout")]
        public ActionResult StartCheckout(FormCollection form)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);

            //validate attributes
            var checkoutAttributeWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, _workContext.CurrentCustomer.CheckoutAttributes, true);
            if (checkoutAttributeWarnings.Count > 0)
            {
                //something wrong, redisplay the page with warnings
                var model = new ShoppingCartModel();
                PrepareShoppingCartModel(model, cart, validateCheckoutAttributes: true);
                return View(model);
            }

            //everything is OK
            if (_workContext.CurrentCustomer.IsGuest())
            {
                if (_orderSettings.AnonymousCheckoutAllowed)
                {
                    return RedirectToRoute("LoginCheckoutAsGuest", new {returnUrl = Url.RouteUrl("ShoppingCart")});
                }
                else
                {
                    return new HttpUnauthorizedResult();
                }
            }
            else
            {
                return RedirectToRoute("Checkout");
            }
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applydiscountcouponcode")]
        public ActionResult ApplyDiscountCoupon(string discountcouponcode, FormCollection form)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            
            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);
            
            var model = new ShoppingCartModel();
            if (!String.IsNullOrWhiteSpace(discountcouponcode))
            {
                var discount = _discountService.GetDiscountByCouponCode(discountcouponcode);
                bool isDiscountValid = discount != null && 
                    discount.RequiresCouponCode &&
                    _discountService.IsDiscountValid(discount, _workContext.CurrentCustomer, discountcouponcode);
                if (isDiscountValid)
                {
                    _workContext.CurrentCustomer.DiscountCouponCode = discountcouponcode;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied");
                }
                else
                {
                    model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
                }
            }
            else
                model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");

            PrepareShoppingCartModel(model, cart);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applygiftcardcouponcode")]
        public ActionResult ApplyGiftCard(string giftcardcouponcode, FormCollection form)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);
            
            var model = new ShoppingCartModel();
            if (!cart.IsRecurring())
            {
                if (!String.IsNullOrWhiteSpace(giftcardcouponcode))
                {
                    var giftCard = _giftCardService.GetAllGiftCards(null, null, 
                        null, null, giftcardcouponcode, 0, int.MaxValue).FirstOrDefault();
                    bool isGiftCardValid = giftCard != null && giftCard.IsGiftCardValid();
                    if (isGiftCardValid)
                    {
                        _workContext.CurrentCustomer.ApplyGiftCardCouponCode(giftcardcouponcode);
                        _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                        model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.Applied");
                    }
                    else
                        model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                }
                else
                    model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
            }
            else
                model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts");

            PrepareShoppingCartModel(model, cart);
            return View(model);
        }
        
        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("estimateshipping")]
        public ActionResult GetEstimateShipping(EstimateShippingModel shippingModel, FormCollection form)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            
            //parse and save checkout attributes
            ParseAndSaveCheckoutAttributes(cart, form);
            
            var model = new ShoppingCartModel();
            model.EstimateShipping.CountryId = shippingModel.CountryId;
            model.EstimateShipping.StateProvinceId = shippingModel.StateProvinceId;
            model.EstimateShipping.ZipPostalCode = shippingModel.ZipPostalCode;
            PrepareShoppingCartModel(model, cart,setEstimateShippingDefaultAddress: false);

            if (cart.RequiresShipping())
            {
                var address = new Address()
                {
                    CountryId = shippingModel.CountryId,
                    Country = shippingModel.CountryId.HasValue ? _countryService.GetCountryById(shippingModel.CountryId.Value) : null,
                    StateProvinceId  = shippingModel.StateProvinceId,
                    StateProvince = shippingModel.StateProvinceId.HasValue ? _stateProvinceService.GetStateProvinceById(shippingModel.StateProvinceId.Value) : null,
                    ZipPostalCode = shippingModel.ZipPostalCode,
                };
                GetShippingOptionResponse getShippingOptionResponse = _shippingService.GetShippingOptions(cart, address);
                if (!getShippingOptionResponse.Success)
                {
                    foreach (var error in getShippingOptionResponse.Errors)
                        model.EstimateShipping.Warnings.Add(error);
                }
                else
                {
                    if (getShippingOptionResponse.ShippingOptions.Count > 0)
                    {
                        foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                        {
                            var soModel = new EstimateShippingModel.ShippingOptionModel()
                            {
                                Name = shippingOption.Name,
                                Description = shippingOption.Description,

                            };
                            //calculate discounted and taxed rate
                            Discount appliedDiscount = null;
                            decimal shippingTotal = _orderTotalCalculationService.AdjustShippingRate(shippingOption.Rate,
                                cart, out appliedDiscount);

                            decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                            decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                            soModel.Price = _priceFormatter.FormatShippingPrice(rate, true);
                            model.EstimateShipping.ShippingOptions.Add(soModel);
                        }
                    }
                    else
                    {
                       model.EstimateShipping.Warnings.Add(_localizationService.GetResource("Checkout.ShippingIsNotAllowed"));
                    }
                }
            }

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult OrderTotals(bool isEditable)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = new OrderTotalsModel();
            model.IsEditable = isEditable;

            if (cart.Count > 0)
            {
                //payment method (if already selected)
                string paymentMethodSystemName = _workContext.CurrentCustomer != null ? _workContext.CurrentCustomer.SelectedPaymentMethodSystemName : null;
                
                //subtotal
                decimal subtotalBase = decimal.Zero;
                decimal orderSubTotalDiscountAmountBase = decimal.Zero;
                Discount orderSubTotalAppliedDiscount = null;
                decimal subTotalWithoutDiscountBase = decimal.Zero;
                decimal subTotalWithDiscountBase = decimal.Zero;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscount,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                subtotalBase = subTotalWithoutDiscountBase;
                    decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                    model.SubTotal = _priceFormatter.FormatPrice(subtotal);
                    if (orderSubTotalDiscountAmountBase > decimal.Zero)
                    {
                        decimal orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, _workContext.WorkingCurrency);
                        model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount);
                        model.AllowRemovingSubTotalDiscount = orderSubTotalAppliedDiscount != null &&
                            orderSubTotalAppliedDiscount.RequiresCouponCode &&
                            !String.IsNullOrEmpty(orderSubTotalAppliedDiscount.CouponCode) &&
                            model.IsEditable;
                    }
                

                //shipping info
                model.RequiresShipping = cart.RequiresShipping();
                if (model.RequiresShipping)
                {
                    decimal? shoppingCartShippingBase = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart);
                    if (shoppingCartShippingBase.HasValue)
                    {
                        decimal shoppingCartShipping = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartShippingBase.Value, _workContext.WorkingCurrency);
                        model.Shipping = _priceFormatter.FormatShippingPrice(shoppingCartShipping, true);

                        //selected shipping method
                        var shippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.LastShippingOption);
                        if (shippingOption != null)
                            model.SelectedShippingMethod = shippingOption.Name;
                    }
                }

                //payment method fee
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                decimal paymentMethodAdditionalFeeWithTaxBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                {
                    decimal paymentMethodAdditionalFeeWithTax = _currencyService.ConvertFromPrimaryStoreCurrency(paymentMethodAdditionalFeeWithTaxBase, _workContext.WorkingCurrency);
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeWithTax, true);
                }

                //tax
                bool displayTax = true;
                bool displayTaxRates = true;
                if (_taxSettings.HideTaxInOrderSummary && _workContext.TaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    SortedDictionary<decimal, decimal> taxRates = null;
                    decimal shoppingCartTaxBase = _orderTotalCalculationService.GetTaxTotal(cart, out taxRates);
                    decimal shoppingCartTax = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTaxBase, _workContext.WorkingCurrency);

                        if (shoppingCartTaxBase == 0 && _taxSettings.HideZeroTax)
                        {
                            displayTax = false;
                            displayTaxRates = false;
                        }
                        else
                        {
                            displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Count > 0;
                            displayTax = !displayTaxRates;

                            model.Tax = _priceFormatter.FormatPrice(shoppingCartTax, true, false);
                            foreach (var tr in taxRates)
                            {
                                model.TaxRates.Add(new OrderTotalsModel.TaxRate()
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
                decimal orderTotalDiscountAmountBase = decimal.Zero;
                Discount orderTotalAppliedDiscount = null;
                List<AppliedGiftCard> appliedGiftCards = null;
                int redeemedRewardPoints = 0;
                decimal redeemedRewardPointsAmount = decimal.Zero;
                decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart,
                    out orderTotalDiscountAmountBase, out orderTotalAppliedDiscount,
                    out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount);
                if (shoppingCartTotalBase.HasValue)
                {
                    decimal shoppingCartTotal = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTotalBase.Value, _workContext.WorkingCurrency);
                    model.OrderTotal = _priceFormatter.FormatPrice(shoppingCartTotal, true, false);
                }

                //discount
                if (orderTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderTotalDiscountAmount, true, false);
                    model.AllowRemovingOrderTotalDiscount = orderTotalAppliedDiscount != null &&
                        orderTotalAppliedDiscount.RequiresCouponCode &&
                        !String.IsNullOrEmpty(orderTotalAppliedDiscount.CouponCode) &&
                        model.IsEditable;
                }

                //gift cards
                if (appliedGiftCards != null && appliedGiftCards.Count > 0)
                {
                    foreach (var appliedGiftCard in appliedGiftCards)
                    {
                        var gcModel = new OrderTotalsModel.GiftCard()
                            {
                                Id = appliedGiftCard.GiftCard.Id,
                                CouponCode =  appliedGiftCard.GiftCard.GiftCardCouponCode,
                            };
                        decimal amountCanBeUsed = _currencyService.ConvertFromPrimaryStoreCurrency(appliedGiftCard.AmountCanBeUsed, _workContext.WorkingCurrency);
                        gcModel.Amount = _priceFormatter.FormatPrice(-amountCanBeUsed, true, false);

                        decimal remainingAmountBase = appliedGiftCard.GiftCard.GetGiftCardRemainingAmount() - appliedGiftCard.AmountCanBeUsed;
                        decimal remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(remainingAmountBase, _workContext.WorkingCurrency);
                        gcModel.Remaining = _priceFormatter.FormatPrice(remainingAmount, true, false);
                        
                        model.GiftCards.Add(gcModel);
                    }
                }

                //reward points
                if (redeemedRewardPointsAmount > decimal.Zero)
                {
                    decimal redeemedRewardPointsAmountInCustomerCurrency = _currencyService.ConvertFromPrimaryStoreCurrency(redeemedRewardPointsAmount, _workContext.WorkingCurrency);
                    model.RedeemedRewardPoints = redeemedRewardPoints;
                    model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
                }
            }


            return PartialView(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("removesubtotaldiscount", "removeordertotaldiscount", "removediscountcouponcode")]
        public ActionResult RemoveDiscountCoupon()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = new ShoppingCartModel();

            _workContext.CurrentCustomer.DiscountCouponCode = "";
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);

            PrepareShoppingCartModel(model, cart);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("removegiftcard")]
        public ActionResult RemoveGiftardCode(int giftCardId)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = new ShoppingCartModel();

            var gc = _giftCardService.GetGiftCardById(giftCardId);
            if (gc != null)
            {
                _workContext.CurrentCustomer.RemoveGiftCardCouponCode(gc.GiftCardCouponCode);
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
            }

            PrepareShoppingCartModel(model, cart);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult FlyoutShoppingCart()
        {
            if (!_shoppingCartSettings.MiniShoppingCartEnabled)
                return Content("");

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return Content("");

            var model = PrepareMiniShoppingCartModel();
            return PartialView(model);
        }

        #endregion

        #region Wishlist

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Wishlist(Guid? customerGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("HomePage");

            Customer customer = customerGuid.HasValue ? 
                _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (customer == null)
                return RedirectToRoute("HomePage");
            var cart = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
            var model = new WishlistModel();
            PrepareWishlistModel(model, cart, !customerGuid.HasValue);
            return View(model);
        }

        //update all wishlist cart items on the page
        [ValidateInput(false)]
        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired("updatecart")]
        public ActionResult UpdateWishlistAll(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();

            var allIdsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _shoppingCartService.DeleteShoppingCartItem(sci);
                else
                {
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int newQuantity = sci.Quantity;
                            if (int.TryParse(form[formKey], out newQuantity))
                            {
                                var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                                    sci.Id, newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }
                            break;
                        }
                }
            }

            //updated wishlist
            cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
            var model = new WishlistModel();
            PrepareWishlistModel(model, cart);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.Where(x => x.Id == sciId).FirstOrDefault();
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!sciModel.Warnings.Contains(w))
                            sciModel.Warnings.Add(w);
            }
            return View(model);
        }

        //update a certain wishlist cart item on the page
        [ValidateInput(false)]
        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired(FormValueRequirement.StartsWith, "updatecartitem-")]
        public ActionResult UpdateWishlistItem(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("HomePage");

            //get wishlist cart item identifier
            int sciId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("updatecartitem-", StringComparison.InvariantCultureIgnoreCase))
                    sciId = Convert.ToInt32(formValue.Substring("updatecartitem-".Length));
            //get shopping cart item
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
            var sci = cart.Where(x => x.Id == sciId).FirstOrDefault();
            if (sci == null)
            {
                return RedirectToRoute("Wishlist");
            }

            //update the wishlist cart item
            var warnings = new List<string>();
            foreach (string formKey in form.AllKeys)
                if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                {
                    int newQuantity = sci.Quantity;
                    if (int.TryParse(form[formKey], out newQuantity))
                    {
                        warnings.AddRange(_shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                            sci.Id, newQuantity, true));
                    }
                    break;
                }


            //updated wishlist
            cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
            var model = new WishlistModel();
            PrepareWishlistModel(model, cart);
            //update current warnings
            //find model
            var sciModel = model.Items.Where(x => x.Id == sciId).FirstOrDefault();
            if (sciModel != null)
                foreach (var w in warnings)
                    if (!sciModel.Warnings.Contains(w))
                        sciModel.Warnings.Add(w);
            return View(model);
        }

        //remove a certain wishlist cart item on the page
        [ValidateInput(false)]
        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removefromcart-")]
        public ActionResult RemoveWishlistItem(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("HomePage");

            //get wishlist cart item identifier
            int sciId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("removefromcart-", StringComparison.InvariantCultureIgnoreCase))
                    sciId = Convert.ToInt32(formValue.Substring("removefromcart-".Length));
            //get wishlist cart item
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
            var sci = cart.Where(x => x.Id == sciId).FirstOrDefault();
            if (sci == null)
            {
                return RedirectToRoute("Wishlist");
            }

            //remove the wishlist cart item
            _shoppingCartService.DeleteShoppingCartItem(sci);


            //updated wishlist
            cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
            var model = new WishlistModel();
            PrepareWishlistModel(model, cart);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired("addtocartbutton")]
        public ActionResult AddItemstoCartFromWishlist(Guid? customerGuid, FormCollection form)
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

            var pageCart = pageCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();

            var allWarnings = new List<string>();
            var numberOfAddedItems = 0;
            var allIdsToAdd = form["addtocart"] != null ? form["addtocart"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();
            foreach (var sci in pageCart)
            {
                if (allIdsToAdd.Contains(sci.Id))
                {
                    var warnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                        sci.ProductVariant, ShoppingCartType.ShoppingCart,
                        sci.AttributesXml, sci.CustomerEnteredPrice, sci.Quantity, true);
                    if (warnings.Count == 0)
                        numberOfAddedItems++;
                    if (_shoppingCartSettings.MoveItemsFromWishlistToCart && //settings enabled
                        !customerGuid.HasValue && //own wishlist
                        warnings.Count == 0) //no warnings ( already in the cart)
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
                return RedirectToRoute("ShoppingCart");
            }
            else
            {
                //no items added. redisplay the wishlist page
                var cart = pageCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
                var model = new WishlistModel();
                PrepareWishlistModel(model, cart, !customerGuid.HasValue);
                return View(model);
            }
        }

        //add a certain wishlist cart item on the page to the shopping cart
        [ValidateInput(false)]
        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired(FormValueRequirement.StartsWith, "addtocart-")]
        public ActionResult AddOneItemtoCartFromWishlist(Guid? customerGuid, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("HomePage");

            //get wishlist cart item identifier
            int sciId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("addtocart-", StringComparison.InvariantCultureIgnoreCase))
                    sciId = Convert.ToInt32(formValue.Substring("addtocart-".Length));
            //get wishlist cart item
            var pageCustomer = customerGuid.HasValue
                ? _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (pageCustomer == null)
                return RedirectToRoute("HomePage");

            var pageCart = pageCustomer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist).ToList();

            var sci = pageCart.Where(x => x.Id == sciId).FirstOrDefault();
            if (sci == null)
            {
                return RedirectToRoute("Wishlist");
            }
            var warnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                                           sci.ProductVariant, ShoppingCartType.ShoppingCart,
                                           sci.AttributesXml, sci.CustomerEnteredPrice, sci.Quantity, true);
            if (_shoppingCartSettings.MoveItemsFromWishlistToCart && //settings enabled
                        !customerGuid.HasValue && //own wishlist
                        warnings.Count == 0) //no warnings ( already in the cart)
            {
                //let's remove the item from wishlist
                _shoppingCartService.DeleteShoppingCartItem(sci);
            }

            if (warnings.Count == 0)
            {
                //redirect to the shopping cart page
                return RedirectToRoute("ShoppingCart");
            }
            else
            {
                //no items added. redisplay the wishlist page
                var cart = pageCustomer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
                var model = new WishlistModel();
                PrepareWishlistModel(model, cart, !customerGuid.HasValue);
                return View(model);
            }
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult EmailWishlist()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) || !_shoppingCartSettings.EmailWishlistEnabled)
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();

            if (cart.Count == 0)
                return RedirectToRoute("HomePage");

            var model = new WishlistEmailAFriendModel()
            {
                YourEmailAddress = _workContext.CurrentCustomer.Email,
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailWishlistToFriendPage
            };
            return View(model);
        }

        [HttpPost, ActionName("EmailWishlist")]
        [FormValueRequired("send-email")]
        [CaptchaValidator]
        public ActionResult EmailWishlistSend(WishlistEmailAFriendModel model, bool captchaValid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) || !_shoppingCartSettings.EmailWishlistEnabled)
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailWishlistToFriendPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptcha"));
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
                        model.FriendEmail, Core.Html.HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("Wishlist.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailWishlistToFriendPage;
            return View(model);
        }

        #endregion
    }
}
