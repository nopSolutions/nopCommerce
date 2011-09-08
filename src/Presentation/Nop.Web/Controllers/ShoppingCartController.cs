using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
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
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Checkout;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;
using System.Web.Routing;

namespace Nop.Web.Controllers
{
    public class ShoppingCartController : BaseNopController
    {
		#region Fields

        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
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
        

        private readonly MediaSettings _mediaSetting;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

		#region Constructors

        public ShoppingCartController(IProductService productService, IWorkContext workContext,
            IShoppingCartService shoppingCartService, IPictureService pictureService,
            ILocalizationService localizationService, IProductAttributeFormatter productAttributeFormatter,
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
            MediaSettings mediaSetting, ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings, OrderSettings orderSettings,
            ShippingSettings shippingSettings,TaxSettings taxSettings)
        {
            this._productService = productService;
            this._workContext = workContext;
            this._shoppingCartService = shoppingCartService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._productAttributeFormatter = productAttributeFormatter;
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

            this._mediaSetting = mediaSetting;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._shippingSettings = shippingSettings;
            this._taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        [NonAction]
        private ShoppingCartModel PrepareShoppingCartModel(ShoppingCartModel model, 
            IList<ShoppingCartItem> cart, bool isEditable, bool setEstimateShippingDefaultAddress = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            if (cart.Count == 0)
                return model;
            
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
            model.ShowDiscountBox = _shoppingCartSettings.ShowDiscountBox;
            model.ShowGiftCardBox = _shoppingCartSettings.ShowGiftCardBox;

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, "", false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);
            
            #endregion

            #region Checkout attributes

            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(!cart.RequiresShipping());
            foreach (var attribute in checkoutAttributes)
            {
                var caModel = new ShoppingCartModel.CheckoutAttributeModel()
                {
                    Id = attribute.Id,
                    Name = attribute.GetLocalized(x => x.Name),
                    TextPrompt = attribute.TextPrompt,
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
                        if (!_catalogSettings.HidePricesForNonRegistered ||
                            !_workContext.CurrentCustomer.IsGuest())
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
                    default:
                        break;
                }

                model.CheckoutAttributes.Add(caModel);
            }

            #endregion 

            #region Estimate shipping

            model.EstimateShipping.Enabled = cart.Count > 0 && cart.RequiresShipping() && _shippingSettings.EstimateShippingEnabled;
            if (model.EstimateShipping.Enabled)
            {
                //countries
                int? defaultEstimateCountryId = (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null) ? _workContext.CurrentCustomer.ShippingAddress.CountryId : model.EstimateShipping.CountryId;
                model.EstimateShipping.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                    model.EstimateShipping.AvailableCountries.Add(new SelectListItem() { 
                        Text = c.Name, 
                        Value = c.Id.ToString(),
                        Selected = c.Id == defaultEstimateCountryId
                    });
                //states
                int? defaultEstimateStateId = (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null) ? _workContext.CurrentCustomer.ShippingAddress.StateProvinceId : model.EstimateShipping.StateProvinceId;
                var states = defaultEstimateCountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(defaultEstimateCountryId.Value).ToList() : new List<StateProvince>();
                if (states.Count > 0)
                    foreach (var s in states)
                        model.EstimateShipping.AvailableStates.Add(new SelectListItem() { 
                            Text = s.Name,
                            Value = s.Id.ToString(),
                            Selected = s.Id == defaultEstimateStateId
                        });
                else
                    model.EstimateShipping.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "0" });

                if (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null)
                    model.EstimateShipping.ZipPostalCode = _workContext.CurrentCustomer.ShippingAddress.ZipPostalCode;
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
                    var picture = _pictureService.GetPictureById(sci.ProductVariant.PictureId);
                    if (picture == null)
                    {
                        picture = _pictureService.GetPicturesByProductId(sci.ProductVariant.Product.Id, 1).FirstOrDefault();
                    }
                    cartItemModel.Picture = new PictureModel()
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSetting.CartThumbPictureSize, true),
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), cartItemModel.ProductName),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), cartItemModel.ProductName),

                    };
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
                .LoadActivePaymentMethods()
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

            return model;
        }

        [NonAction]
        private WishlistModel PrepareWishlistModel(WishlistModel model, IList<ShoppingCartItem> cart, bool isEditable)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.EmailWishlistEnabled = _shoppingCartSettings.EmailWishlistEnabled;
            model.IsEditable = isEditable;

            if (cart.Count == 0)
                return model;

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
                    var picture = _pictureService.GetPictureById(sci.ProductVariant.PictureId);
                    if (picture == null)
                    {
                        picture = _pictureService.GetPicturesByProductId(sci.ProductVariant.Product.Id, 1).FirstOrDefault();
                    }
                    cartItemModel.Picture = new PictureModel()
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSetting.CartThumbPictureSize, true),
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), cartItemModel.ProductName),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), cartItemModel.ProductName),

                    };
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

            return model;
        }

        #endregion

        #region Shopping cart

        public ActionResult AddProductToCart(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return RedirectToAction("Index", "Home");

            int productVariantId = 0;
            if (_shoppingCartService.DirectAddToCartAllowed(productId, out productVariantId))
            {
                var productVariant = _productService.GetProductVariantById(productVariantId);
                var addToCartWarnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                    productVariant, ShoppingCartType.ShoppingCart,
                    string.Empty, decimal.Zero, 1, true);
                if (addToCartWarnings.Count == 0)
                    return RedirectToRoute("ShoppingCart");
                else
                    return RedirectToRoute("Product", new { productId = product.Id, SeName = product.GetSeName() });
            }
            else
                return RedirectToRoute("Product", new { productId = product.Id, SeName = product.GetSeName() });
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Cart()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = PrepareShoppingCartModel(new ShoppingCartModel(), cart, true);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult OrderSummary(bool isEditable)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = PrepareShoppingCartModel(new ShoppingCartModel(), cart, isEditable);
            return PartialView(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("updatecart")]
        public ActionResult UpdateCart(FormCollection form)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            var allIdsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _shoppingCartService.DeleteShoppingCartItem(sci, true);
                else
                {
                    int newQuantity = sci.Quantity;
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int.TryParse(form[formKey], out newQuantity);
                            break;
                        }
                    _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                        sci.Id, newQuantity, true);
                }
            }

            //updated cart
            cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = PrepareShoppingCartModel(new ShoppingCartModel(), cart, true);
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
                return RedirectToAction("Index", "Home");
            }
        }
        
        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("startcheckout")]
        public ActionResult StartCheckout(FormCollection form)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            //apply attributes
            string selectedAttributes = "";

            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(!cart.RequiresShipping());
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
                    default:
                        break;
                }
            }

            _workContext.CurrentCustomer.CheckoutAttributes = selectedAttributes;
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);

            if (_workContext.CurrentCustomer.IsGuest())
            {
                if (_orderSettings.AnonymousCheckoutAllowed)
                {
                    return RedirectToRoute("LoginCheckoutAsGuest");
                }
                else
                {
                    return RedirectToRoute("Login");
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
        public ActionResult ApplyDiscountCoupon(string discountcouponcode)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = new ShoppingCartModel();

            if (!String.IsNullOrWhiteSpace(discountcouponcode))
            {
                var discounts = _discountService.GetAllDiscounts(null);
                var discount = discounts.Where(d => !String.IsNullOrEmpty(d.CouponCode) && d.CouponCode.Equals(discountcouponcode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                bool isDiscountValid = discount != null;
                if (isDiscountValid)
                {
                    _workContext.CurrentCustomer.DiscountCouponCode = discountcouponcode;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    model.DiscountMessage = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied");
                }
                else
                {
                    model.DiscountMessage = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
                }
            }

            model = PrepareShoppingCartModel(model, cart, true);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applygiftcardcouponcode")]
        public ActionResult ApplyGiftCard(string giftcardcouponcode)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = new ShoppingCartModel();

            if (!cart.IsRecurring())
            {
                if (!String.IsNullOrWhiteSpace(giftcardcouponcode))
                {
                    var giftCard = _giftCardService.GetAllGiftCards(null, null, null, null, giftcardcouponcode).FirstOrDefault();
                    bool isGiftCardValid = giftCard != null && giftCard.IsGiftCardValid();
                    if (isGiftCardValid)
                    {
                        _workContext.CurrentCustomer.ApplyGiftCardCouponCode(giftcardcouponcode);
                        _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                        model.GiftCardMessage = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.Applied");
                    }
                    else
                        model.GiftCardMessage = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                }
            }
            else
                model.GiftCardMessage = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts");

            model = PrepareShoppingCartModel(model, cart, true);
            return View(model);
        }
        
        [ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("estimateshipping")]
        public ActionResult GetEstimateShipping(EstimateShippingModel shippingModel)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = new ShoppingCartModel();
            model.EstimateShipping.CountryId = shippingModel.CountryId;
            model.EstimateShipping.StateProvinceId = shippingModel.StateProvinceId;
            model.EstimateShipping.ZipPostalCode = shippingModel.ZipPostalCode;
            model = PrepareShoppingCartModel(model, cart, true, false);

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
                            decimal shippingTotalWithoutDiscount = shippingOption.Rate;
                            decimal discountAmount = _orderTotalCalculationService.GetShippingDiscount(_workContext.CurrentCustomer,
                                shippingTotalWithoutDiscount, out appliedDiscount);
                            decimal shippingTotalWithDiscount = shippingTotalWithoutDiscount - discountAmount;
                            if (shippingTotalWithDiscount < decimal.Zero)
                                shippingTotalWithDiscount = decimal.Zero;
                            shippingTotalWithDiscount = Math.Round(shippingTotalWithDiscount, 2);

                            decimal rateBase = _taxService.GetShippingPrice(shippingTotalWithDiscount, _workContext.CurrentCustomer);
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
                    }
                }

                //payment method fee
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(paymentMethodSystemName);
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
        [FormValueRequired("removesubtotaldiscount", "removeordertotaldiscount")]
        public ActionResult RemoveDiscountCoupon()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = new ShoppingCartModel();

            _workContext.CurrentCustomer.DiscountCouponCode = "";
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);

            model = PrepareShoppingCartModel(model, cart, true);
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

            model = PrepareShoppingCartModel(model, cart, true);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult MiniShoppingCart()
        {
            if (!_shoppingCartSettings.MiniShoppingCartEnabled)
                return Content("");

            var model = new MiniShoppingCartModel()
            {
                DisplayProducts = _shoppingCartSettings.MiniShoppingCartDisplayProducts
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
            }
            if (_shoppingCartSettings.MiniShoppingCartDisplayProducts)
            {
                foreach (var sci in cart)
                {
                    var cartItemModel = new MiniShoppingCartModel.ShoppingCartItemModel()
                    {
                        Id = sci.Id,
                        ProductId = sci.ProductVariant.ProductId,
                        ProductSeName = sci.ProductVariant.Product.GetSeName(),
                        Quantity = sci.Quantity,
                    };

                    //product name
                    if (!String.IsNullOrEmpty(sci.ProductVariant.GetLocalized(x => x.Name)))
                        cartItemModel.ProductName = string.Format("{0} ({1})", sci.ProductVariant.Product.GetLocalized(x => x.Name), sci.ProductVariant.GetLocalized(x => x.Name));
                    else
                        cartItemModel.ProductName = sci.ProductVariant.Product.GetLocalized(x => x.Name);

                    model.Items.Add(cartItemModel);
                }
            }
            return PartialView(model);
        }

        #endregion

        #region Wishlist

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Wishlist(Guid? customerGuid)
        {
            if (!_shoppingCartSettings.WishlistEnabled)
                return RedirectToAction("Index", "Home");

            Customer customer = customerGuid.HasValue ? 
                _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (customer == null)
                return RedirectToAction("Index", "Home");
            var cart = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
            var model = PrepareWishlistModel(new WishlistModel(), cart, !customerGuid.HasValue);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired("updatecart")]
        public ActionResult UpdateWishlist(FormCollection form)
        {
            if (!_shoppingCartSettings.WishlistEnabled)
                return RedirectToAction("Index", "Home");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();

            var allIdsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _shoppingCartService.DeleteShoppingCartItem(sci, true);
                else
                {
                    int newQuantity = sci.Quantity;
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int.TryParse(form[formKey], out newQuantity);
                            break;
                        }
                    _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                        sci.Id, newQuantity, true);
                }
            }

            //updated cart
            cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
            var model = PrepareWishlistModel(new WishlistModel(), cart, true);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired("addtocartbutton")]
        public ActionResult AddtoCartFromWishlist(FormCollection form)
        {
            if (!_shoppingCartSettings.WishlistEnabled)
                return RedirectToAction("Index", "Home");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();

            var allIdsToAdd = form["addtocart"] != null ? form["addtocart"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();
            foreach (var sci in cart)
            {
                if (allIdsToAdd.Contains(sci.Id))
                {
                    _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                        sci.ProductVariant, ShoppingCartType.ShoppingCart,
                        sci.AttributesXml, sci.CustomerEnteredPrice, sci.Quantity, true);
                }
            }

            //updated cart
            cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();
            var model = PrepareWishlistModel(new WishlistModel(), cart, true);
            return View(model);
        }

        public ActionResult EmailWishlist()
        {
            if (!_shoppingCartSettings.WishlistEnabled || !_shoppingCartSettings.EmailWishlistEnabled)
                return RedirectToAction("Index", "Home");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();

            if (cart.Count == 0)
                return RedirectToAction("Index", "Home");

            var model = new WishlistEmailAFriendModel()
            {
                YourEmailAddress = _workContext.CurrentCustomer.Email
            };
            return View(model);
        }

        [HttpPost, ActionName("EmailWishlist")]
        [FormValueRequired("send-email")]
        public ActionResult EmailWishlistSend(WishlistEmailAFriendModel model)
        {
            if (!_shoppingCartSettings.WishlistEnabled || !_shoppingCartSettings.EmailWishlistEnabled)
                return RedirectToAction("Index", "Home");
            
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList();

            if (cart.Count == 0)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                if (_workContext.CurrentCustomer.IsGuest())
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Wishlist.EmailAFriend.OnlyRegisteredUsers"));
                }
                else
                {
                    //email
                    _workflowMessageService.SendWishlistEmailAFriendMessage(_workContext.CurrentCustomer,
                            _workContext.WorkingLanguage.Id, model.YourEmailAddress,
                            model.FriendEmail, Core.Html.HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                    model.SuccessfullySent = true;
                    model.Result = _localizationService.GetResource("Wishlist.EmailAFriend.SuccessfullySent");

                    return View(model);
                }
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion
    }
}
