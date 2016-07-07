using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial class ShoppingCartService : IShoppingCartService
    {
        #region Fields

        private readonly IRepository<ShoppingCartItem> _sciRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICustomerService _customerService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="sciRepository">Shopping cart repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="productService">Product settings</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="checkoutAttributeService">Checkout attribute service</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="shoppingCartSettings">Shopping cart settings</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="permissionService">Permission service</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="productAttributeService">Product attribute service</param>
        /// <param name="dateTimeHelper">Datetime helper</param>
        public ShoppingCartService(IRepository<ShoppingCartItem> sciRepository,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ICurrencyService currencyService,
            IProductService productService,
            ILocalizationService localizationService,
            IProductAttributeParser productAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IPriceFormatter priceFormatter,
            ICustomerService customerService,
            ShoppingCartSettings shoppingCartSettings,
            IEventPublisher eventPublisher,
            IPermissionService permissionService, 
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IGenericAttributeService genericAttributeService,
            IProductAttributeService productAttributeService,
            IDateTimeHelper dateTimeHelper)
        {
            this._sciRepository = sciRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._productService = productService;
            this._localizationService = localizationService;
            this._productAttributeParser = productAttributeParser;
            this._checkoutAttributeService = checkoutAttributeService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._priceFormatter = priceFormatter;
            this._customerService = customerService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._eventPublisher = eventPublisher;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._genericAttributeService = genericAttributeService;
            this._productAttributeService = productAttributeService;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
        public virtual void DeleteShoppingCartItem(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true, 
            bool ensureOnlyActiveCheckoutAttributes = false)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException("shoppingCartItem");

            var customer = shoppingCartItem.Customer;
            var storeId = shoppingCartItem.StoreId;

            //reset checkout data
            if (resetCheckoutData)
            {
                _customerService.ResetCheckoutData(shoppingCartItem.Customer, shoppingCartItem.StoreId);
            }

            //delete item
            _sciRepository.Delete(shoppingCartItem);

            //reset "HasShoppingCartItems" property used for performance optimization
            customer.HasShoppingCartItems = customer.ShoppingCartItems.Any();
            _customerService.UpdateCustomer(customer);

            //validate checkout attributes
            if (ensureOnlyActiveCheckoutAttributes &&
                //only for shopping cart items (ignore wishlist)
                shoppingCartItem.ShoppingCartType == ShoppingCartType.ShoppingCart)
            {
                var cart = customer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(storeId)
                    .ToList();

                var checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, storeId);
                checkoutAttributesXml = _checkoutAttributeParser.EnsureOnlyActiveAttributes(checkoutAttributesXml, cart);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CheckoutAttributes, checkoutAttributesXml, storeId);
            }

            //event notification
            _eventPublisher.EntityDeleted(shoppingCartItem);
        }

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        public virtual int DeleteExpiredShoppingCartItems(DateTime olderThanUtc)
        {
            var query = from sci in _sciRepository.Table
                           where sci.UpdatedOnUtc < olderThanUtc
                           select sci;

            var cartItems = query.ToList();
            foreach (var cartItem in cartItems)
                _sciRepository.Delete(cartItem);
            return cartItems.Count;
        }

        /// <summary>
        /// Validates required products (products which require some other products to be added to the cart)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetRequiredProductWarnings(Customer customer,
            ShoppingCartType shoppingCartType, Product product,
            int storeId, bool automaticallyAddRequiredProductsIfEnabled)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (product == null)
                throw new ArgumentNullException("product");

            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == shoppingCartType)
                .LimitPerStore(storeId)
                .ToList();

            var warnings = new List<string>();

            if (product.RequireOtherProducts)
            {
                var requiredProducts = new List<Product>();
                foreach (var id in product.ParseRequiredProductIds())
                {
                    var rp = _productService.GetProductById(id);
                    if (rp != null)
                        requiredProducts.Add(rp);
                }
                
                foreach (var rp in requiredProducts)
                {
                    //ensure that product is in the cart
                    bool alreadyInTheCart = false;
                    foreach (var sci in cart)
                    {
                        if (sci.ProductId == rp.Id)
                        {
                            alreadyInTheCart = true;
                            break;
                        }
                    }
                    //not in the cart
                    if (!alreadyInTheCart)
                    {

                        if (product.AutomaticallyAddRequiredProducts)
                        {
                            //add to cart (if possible)
                            if (automaticallyAddRequiredProductsIfEnabled)
                            {
                                //pass 'false' for 'automaticallyAddRequiredProductsIfEnabled' to prevent circular references
                                var addToCartWarnings = AddToCart(customer: customer,
                                    product: rp, 
                                    shoppingCartType: shoppingCartType,
                                    storeId: storeId,
                                    automaticallyAddRequiredProductsIfEnabled: false);
                                if (addToCartWarnings.Any())
                                {
                                    //a product wasn't atomatically added for some reasons

                                    //don't display specific errors from 'addToCartWarnings' variable
                                    //display only generic error
                                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredProductWarning"), rp.GetLocalized(x => x.Name)));
                                }
                            }
                            else
                            {
                                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredProductWarning"), rp.GetLocalized(x => x.Name)));
                            }
                        }
                        else
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredProductWarning"), rp.GetLocalized(x => x.Name)));
                        }
                    }
                }
            }

            return warnings;
        }
        
        /// <summary>
        /// Validates a product for standard properties
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetStandardWarnings(Customer customer, ShoppingCartType shoppingCartType,
            Product product, string attributesXml, decimal customerEnteredPrice,
            int quantity)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            //deleted
            if (product.Deleted)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ProductDeleted"));
                return warnings;
            }

            //published
            if (!product.Published)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ProductUnpublished"));
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                warnings.Add("This is not simple product");
            }
            
            //ACL
            if (!_aclService.Authorize(product, customer))
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ProductUnpublished"));
            }

            //Store mapping
            if (!_storeMappingService.Authorize(product, _storeContext.CurrentStore.Id))
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ProductUnpublished"));
            }

            //disabled "add to cart" button
            if (shoppingCartType == ShoppingCartType.ShoppingCart && product.DisableBuyButton)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.BuyingDisabled"));
            }

            //disabled "add to wishlist" button
            if (shoppingCartType == ShoppingCartType.Wishlist && product.DisableWishlistButton)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.WishlistDisabled"));
            }

            //call for price
            if (shoppingCartType == ShoppingCartType.ShoppingCart && product.CallForPrice)
            {
                warnings.Add(_localizationService.GetResource("Products.CallForPrice"));
            }

            //customer entered price
            if (product.CustomerEntersPrice)
            {
                if (customerEnteredPrice < product.MinimumCustomerEnteredPrice ||
                    customerEnteredPrice > product.MaximumCustomerEnteredPrice)
                {
                    decimal minimumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MinimumCustomerEnteredPrice, _workContext.WorkingCurrency);
                    decimal maximumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MaximumCustomerEnteredPrice, _workContext.WorkingCurrency);
                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.CustomerEnteredPrice.RangeError"),
                        _priceFormatter.FormatPrice(minimumCustomerEnteredPrice, false, false),
                        _priceFormatter.FormatPrice(maximumCustomerEnteredPrice, false, false)));
                }
            }

            //quantity validation
            var hasQtyWarnings = false;
            if (quantity < product.OrderMinimumQuantity)
            {
                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MinimumQuantity"), product.OrderMinimumQuantity));
                hasQtyWarnings = true;
            }
            if (quantity > product.OrderMaximumQuantity)
            {
                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumQuantity"), product.OrderMaximumQuantity));
                hasQtyWarnings = true;
            }
            var allowedQuantities = product.ParseAllowedQuantities();
            if (allowedQuantities.Length > 0 && !allowedQuantities.Contains(quantity))
            {
                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.AllowedQuantities"), string.Join(", ", allowedQuantities)));
            }

            var validateOutOfStock = shoppingCartType == ShoppingCartType.ShoppingCart || !_shoppingCartSettings.AllowOutOfStockItemsToBeAddedToWishlist;
            if (validateOutOfStock && !hasQtyWarnings)
            {
                switch (product.ManageInventoryMethod)
                {
                    case ManageInventoryMethod.DontManageStock:
                        {
                            //do nothing
                        }
                        break;
                    case ManageInventoryMethod.ManageStock:
                        {
                            if (product.BackorderMode == BackorderMode.NoBackorders)
                            {
                                int maximumQuantityCanBeAdded = product.GetTotalStockQuantity();
                                if (maximumQuantityCanBeAdded < quantity)
                                {
                                    if (maximumQuantityCanBeAdded <= 0)
                                        warnings.Add(_localizationService.GetResource("ShoppingCart.OutOfStock"));
                                    else
                                        warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
                                }
                            }
                        }
                        break;
                    case ManageInventoryMethod.ManageStockByAttributes:
                        {
                            var combination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
                            if (combination != null)
                            {
                                //combination exists
                                //let's check stock level
                                if (!combination.AllowOutOfStockOrders && combination.StockQuantity < quantity)
                                {
                                    int maximumQuantityCanBeAdded = combination.StockQuantity;
                                    if (maximumQuantityCanBeAdded <= 0)
                                    {
                                        warnings.Add(_localizationService.GetResource("ShoppingCart.OutOfStock"));
                                    }
                                    else
                                    {
                                        warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
                                    }
                                }
                            }
                            else
                            {
                                //combination doesn't exist
                                if (product.AllowAddingOnlyExistingAttributeCombinations)
                                {
                                    //maybe, is it better  to display something like "No such product/combination" message?
                                    warnings.Add(_localizationService.GetResource("ShoppingCart.OutOfStock"));
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //availability dates
            bool availableStartDateError = false;
            if (product.AvailableStartDateTimeUtc.HasValue)
            {
                DateTime now = DateTime.UtcNow;
                DateTime availableStartDateTime = DateTime.SpecifyKind(product.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableStartDateTime.CompareTo(now) > 0)
                {
                    warnings.Add(_localizationService.GetResource("ShoppingCart.NotAvailable"));
                    availableStartDateError = true;
                }
            }
            if (product.AvailableEndDateTimeUtc.HasValue && !availableStartDateError)
            {
                DateTime now = DateTime.UtcNow;
                DateTime availableEndDateTime = DateTime.SpecifyKind(product.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableEndDateTime.CompareTo(now) < 0)
                {
                    warnings.Add(_localizationService.GetResource("ShoppingCart.NotAvailable"));
                }
            }
            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemAttributeWarnings(Customer customer, 
            ShoppingCartType shoppingCartType,
            Product product, 
            int quantity = 1,
            string attributesXml = "",
            bool ignoreNonCombinableAttributes = false)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            //ensure it's our attributes
            var attributes1 = _productAttributeParser.ParseProductAttributeMappings(attributesXml);
            if (ignoreNonCombinableAttributes)
            {
                attributes1 = attributes1.Where(x => !x.IsNonCombinable()).ToList();
            }
            foreach (var attribute in attributes1)
            {
                if (attribute.Product != null)
                {
                    if (attribute.Product.Id != product.Id)
                    {
                        warnings.Add("Attribute error");
                    }
                }
                else
                {
                    warnings.Add("Attribute error");
                    return warnings;
                }
            }

            //validate required product attributes (whether they're chosen/selected/entered)
            var attributes2 = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            if (ignoreNonCombinableAttributes)
            {
                attributes2 = attributes2.Where(x => !x.IsNonCombinable()).ToList();
            }
            //validate conditional attributes only (if specified)
            attributes2 = attributes2.Where(x =>
            {
                var conditionMet = _productAttributeParser.IsConditionMet(x, attributesXml);
                return !conditionMet.HasValue || conditionMet.Value;
            }).ToList();
            foreach (var a2 in attributes2)
            {
                if (a2.IsRequired)
                {
                    bool found = false;
                    //selected product attributes
                    foreach (var a1 in attributes1)
                    {
                        if (a1.Id == a2.Id)
                        {
                            var attributeValuesStr = _productAttributeParser.ParseValues(attributesXml, a1.Id);
                            foreach (string str1 in attributeValuesStr)
                            {
                                if (!String.IsNullOrEmpty(str1.Trim()))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

                    //if not found
                    if (!found)
                    {
                        var notFoundWarning = !string.IsNullOrEmpty(a2.TextPrompt) ?
                            a2.TextPrompt : 
                            string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"), a2.ProductAttribute.GetLocalized(a => a.Name));
                        
                        warnings.Add(notFoundWarning);
                    }
                }

                if (a2.AttributeControlType == AttributeControlType.ReadonlyCheckboxes)
                {
                    //customers cannot edit read-only attributes
                    var allowedReadOnlyValueIds = _productAttributeService.GetProductAttributeValues(a2.Id)
                        .Where(x => x.IsPreSelected)
                        .Select(x => x.Id)
                        .ToArray();

                    var selectedReadOnlyValueIds = _productAttributeParser.ParseProductAttributeValues(attributesXml)
                        .Where(x => x.ProductAttributeMappingId == a2.Id)
                        .Select(x => x.Id)
                        .ToArray();

                    if (!CommonHelper.ArraysEqual(allowedReadOnlyValueIds, selectedReadOnlyValueIds))
                    {
                        warnings.Add("You cannot change read-only values");
                    }
                }
            }

            //validation rules
            foreach (var pam in attributes2)
            {
                if (!pam.ValidationRulesAllowed())
                    continue;
                
                //minimum length
                if (pam.ValidationMinLength.HasValue)
                {
                    if (pam.AttributeControlType == AttributeControlType.TextBox ||
                        pam.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        var valuesStr = _productAttributeParser.ParseValues(attributesXml, pam.Id);
                        var enteredText = valuesStr.FirstOrDefault();
                        int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                        if (pam.ValidationMinLength.Value > enteredTextLength)
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.TextboxMinimumLength"), pam.ProductAttribute.GetLocalized(a => a.Name), pam.ValidationMinLength.Value));
                        }
                    }
                }

                //maximum length
                if (pam.ValidationMaxLength.HasValue)
                {
                    if (pam.AttributeControlType == AttributeControlType.TextBox ||
                        pam.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        var valuesStr = _productAttributeParser.ParseValues(attributesXml, pam.Id);
                        var enteredText = valuesStr.FirstOrDefault();
                        int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                        if (pam.ValidationMaxLength.Value < enteredTextLength)
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.TextboxMaximumLength"), pam.ProductAttribute.GetLocalized(a => a.Name), pam.ValidationMaxLength.Value));
                        }
                    }
                }
            }

            if (warnings.Any())
                return warnings;

            //validate bundled products
            var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                {
                    if (ignoreNonCombinableAttributes && attributeValue.ProductAttributeMapping.IsNonCombinable())
                        continue;

                    //associated product (bundle)
                    var associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
                    if (associatedProduct != null)
                    {
                        var totalQty = quantity * attributeValue.Quantity;
                        var associatedProductWarnings = GetShoppingCartItemWarnings(customer,
                            shoppingCartType, associatedProduct, _storeContext.CurrentStore.Id,
                            "", decimal.Zero, null, null, totalQty, false);
                        foreach (var associatedProductWarning in associatedProductWarnings)
                        {
                            var attributeName = attributeValue.ProductAttributeMapping.ProductAttribute.GetLocalized(a => a.Name);
                            var attributeValueName = attributeValue.GetLocalized(a => a.Name);
                            warnings.Add(string.Format(
                                _localizationService.GetResource("ShoppingCart.AssociatedAttributeWarning"), 
                                attributeName, attributeValueName, associatedProductWarning));
                        }
                    }
                    else
                    {
                        warnings.Add(string.Format("Associated product cannot be loaded - {0}", attributeValue.AssociatedProductId));
                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemGiftCardWarnings(ShoppingCartType shoppingCartType,
            Product product, string attributesXml)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            //gift cards
            if (product.IsGiftCard)
            {
                string giftCardRecipientName;
                string giftCardRecipientEmail;
                string giftCardSenderName;
                string giftCardSenderEmail;
                string giftCardMessage;
                _productAttributeParser.GetGiftCardAttribute(attributesXml,
                    out giftCardRecipientName, out giftCardRecipientEmail,
                    out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                if (String.IsNullOrEmpty(giftCardRecipientName))
                    warnings.Add(_localizationService.GetResource("ShoppingCart.RecipientNameError"));

                if (product.GiftCardType == GiftCardType.Virtual)
                {
                    //validate for virtual gift cards only
                    if (String.IsNullOrEmpty(giftCardRecipientEmail) || !CommonHelper.IsValidEmail(giftCardRecipientEmail))
                        warnings.Add(_localizationService.GetResource("ShoppingCart.RecipientEmailError"));
                }

                if (String.IsNullOrEmpty(giftCardSenderName))
                    warnings.Add(_localizationService.GetResource("ShoppingCart.SenderNameError"));

                if (product.GiftCardType == GiftCardType.Virtual)
                {
                    //validate for virtual gift cards only
                    if (String.IsNullOrEmpty(giftCardSenderEmail) || !CommonHelper.IsValidEmail(giftCardSenderEmail))
                        warnings.Add(_localizationService.GetResource("ShoppingCart.SenderEmailError"));
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item for rental products
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetRentalProductWarnings(Product product,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null)
        {
            if (product == null)
                throw new ArgumentNullException("product");
            
            var warnings = new List<string>();

            if (!product.IsRental)
                return warnings;

            if (!rentalStartDate.HasValue)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.Rental.EnterStartDate"));
                return warnings;
            }
            if (!rentalEndDate.HasValue)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.Rental.EnterEndDate"));
                return warnings;
            }
            if (rentalStartDate.Value.CompareTo(rentalEndDate.Value) > 0)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.Rental.StartDateLessEndDate"));
                return warnings;
            }

            //allowed start date should be the future date
            //we should compare rental start date with a store local time
            //but we what if a store works in distinct timezones? how we should handle it? skip it for now
            //we also ignore hours (anyway not supported yet)
            //today
            DateTime nowDtInStoreTimeZone = _dateTimeHelper.ConvertToUserTime(DateTime.Now, TimeZoneInfo.Local, _dateTimeHelper.DefaultStoreTimeZone);
            var todayDt = new DateTime(nowDtInStoreTimeZone.Year, nowDtInStoreTimeZone.Month, nowDtInStoreTimeZone.Day);
            DateTime todayDtUtc = _dateTimeHelper.ConvertToUtcTime(todayDt, _dateTimeHelper.DefaultStoreTimeZone);
            //dates are entered in store timezone (e.g. like in hotels)
            DateTime startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.DefaultStoreTimeZone);
            //but we what if dates should be entered in a customer timezone?
            //DateTime startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.CurrentTimeZone);
            if (todayDtUtc.CompareTo(startDateUtc) > 0)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.Rental.StartDateShouldBeFuture"));
                return warnings;
            }

            return warnings;
        }


        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <param name="getStandardWarnings">A value indicating whether we should validate a product for standard properties</param>
        /// <param name="getAttributesWarnings">A value indicating whether we should validate product attributes</param>
        /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
        /// <param name="getRequiredProductWarnings">A value indicating whether we should validate required products (products which require other products to be added to the cart)</param>
        /// <param name="getRentalWarnings">A value indicating whether we should validate rental properties</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemWarnings(Customer customer, ShoppingCartType shoppingCartType,
            Product product, int storeId,
            string attributesXml, decimal customerEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool automaticallyAddRequiredProductsIfEnabled = true,
            bool getStandardWarnings = true, bool getAttributesWarnings = true,
            bool getGiftCardWarnings = true, bool getRequiredProductWarnings = true,
            bool getRentalWarnings = true)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();
            
            //standard properties
            if (getStandardWarnings)
                warnings.AddRange(GetStandardWarnings(customer, shoppingCartType, product, attributesXml, customerEnteredPrice, quantity));

            //selected attributes
            if (getAttributesWarnings)
                warnings.AddRange(GetShoppingCartItemAttributeWarnings(customer, shoppingCartType, product, quantity, attributesXml));

            //gift cards
            if (getGiftCardWarnings)
                warnings.AddRange(GetShoppingCartItemGiftCardWarnings(shoppingCartType, product, attributesXml));

            //required products
            if (getRequiredProductWarnings)
                warnings.AddRange(GetRequiredProductWarnings(customer, shoppingCartType, product, storeId, automaticallyAddRequiredProductsIfEnabled));

            //rental products
            if (getRentalWarnings)
                warnings.AddRange(GetRentalProductWarnings(product, rentalStartDate, rentalEndDate));
            
            return warnings;
        }

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributesXml">Checkout attributes in XML format</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartWarnings(IList<ShoppingCartItem> shoppingCart, 
            string checkoutAttributesXml, bool validateCheckoutAttributes)
        {
            var warnings = new List<string>();

            bool hasStandartProducts = false;
            bool hasRecurringProducts = false;

            foreach (var sci in shoppingCart)
            {
                var product = sci.Product;
                if (product == null)
                {
                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.CannotLoadProduct"), sci.ProductId));
                    return warnings;
                }

                if (product.IsRecurring)
                    hasRecurringProducts = true;
                else
                    hasStandartProducts = true;
            }

            //don't mix standard and recurring products
            if (hasStandartProducts && hasRecurringProducts)
                warnings.Add(_localizationService.GetResource("ShoppingCart.CannotMixStandardAndAutoshipProducts"));

            //recurring cart validation
            if (hasRecurringProducts)
            {
                int cycleLength;
                RecurringProductCyclePeriod cyclePeriod;
                int totalCycles;
                string cyclesError = shoppingCart.GetRecurringCycleInfo(_localizationService,
                    out cycleLength, out cyclePeriod, out totalCycles);
                if (!string.IsNullOrEmpty(cyclesError))
                {
                    warnings.Add(cyclesError);
                    return warnings;
                }
            }

            //validate checkout attributes
            if (validateCheckoutAttributes)
            {
                //selected attributes
                var attributes1 = _checkoutAttributeParser.ParseCheckoutAttributes(checkoutAttributesXml);

                //existing checkout attributes
                var attributes2 = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !shoppingCart.RequiresShipping());
                foreach (var a2 in attributes2)
                {
                    if (a2.IsRequired)
                    {
                        bool found = false;
                        //selected checkout attributes
                        foreach (var a1 in attributes1)
                        {
                            if (a1.Id == a2.Id)
                            {
                                var attributeValuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, a1.Id);
                                foreach (string str1 in attributeValuesStr)
                                    if (!String.IsNullOrEmpty(str1.Trim()))
                                    {
                                        found = true;
                                        break;
                                    }
                            }
                        }

                        //if not found
                        if (!found)
                        {
                            if (!string.IsNullOrEmpty(a2.GetLocalized(a => a.TextPrompt)))
                                warnings.Add(a2.GetLocalized(a => a.TextPrompt));
                            else
                                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"), a2.GetLocalized(a => a.Name)));
                        }
                    }
                }

                //now validation rules

                //minimum length
                foreach (var ca in attributes2)
                {
                    if (ca.ValidationMinLength.HasValue)
                    {
                        if (ca.AttributeControlType == AttributeControlType.TextBox ||
                            ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            var valuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id);
                            var enteredText = valuesStr.FirstOrDefault();
                            int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                            if (ca.ValidationMinLength.Value > enteredTextLength)
                            {
                                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.TextboxMinimumLength"), ca.GetLocalized(a => a.Name), ca.ValidationMinLength.Value));
                            }
                        }
                    }

                    //maximum length
                    if (ca.ValidationMaxLength.HasValue)
                    {
                        if (ca.AttributeControlType == AttributeControlType.TextBox ||
                            ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            var valuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id);
                            var enteredText = valuesStr.FirstOrDefault();
                            int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                            if (ca.ValidationMaxLength.Value < enteredTextLength)
                            {
                                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.TextboxMaximumLength"), ca.GetLocalized(a => a.Name), ca.ValidationMaxLength.Value));
                            }
                        }
                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Price entered by a customer</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>Found shopping cart item</returns>
        public virtual ShoppingCartItem FindShoppingCartItemInTheCart(IList<ShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
            Product product,
            string attributesXml = "",
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null, 
            DateTime? rentalEndDate = null)
        {
            if (shoppingCart == null)
                throw new ArgumentNullException("shoppingCart");

            if (product == null)
                throw new ArgumentNullException("product");

            foreach (var sci in shoppingCart.Where(a => a.ShoppingCartType == shoppingCartType))
            {
                if (sci.ProductId == product.Id)
                {
                    //attributes
                    bool attributesEqual = _productAttributeParser.AreProductAttributesEqual(sci.AttributesXml, attributesXml, false);

                    //gift cards
                    bool giftCardInfoSame = true;
                    if (sci.Product.IsGiftCard)
                    {
                        string giftCardRecipientName1;
                        string giftCardRecipientEmail1;
                        string giftCardSenderName1;
                        string giftCardSenderEmail1;
                        string giftCardMessage1;
                        _productAttributeParser.GetGiftCardAttribute(attributesXml,
                            out giftCardRecipientName1, out giftCardRecipientEmail1,
                            out giftCardSenderName1, out giftCardSenderEmail1, out giftCardMessage1);

                        string giftCardRecipientName2;
                        string giftCardRecipientEmail2;
                        string giftCardSenderName2;
                        string giftCardSenderEmail2;
                        string giftCardMessage2;
                        _productAttributeParser.GetGiftCardAttribute(sci.AttributesXml,
                            out giftCardRecipientName2, out giftCardRecipientEmail2,
                            out giftCardSenderName2, out giftCardSenderEmail2, out giftCardMessage2);


                        if (giftCardRecipientName1.ToLowerInvariant() != giftCardRecipientName2.ToLowerInvariant() ||
                            giftCardSenderName1.ToLowerInvariant() != giftCardSenderName2.ToLowerInvariant())
                            giftCardInfoSame = false;
                    }

                    //price is the same (for products which require customers to enter a price)
                    bool customerEnteredPricesEqual = true;
                    if (sci.Product.CustomerEntersPrice)
                        //TODO should we use RoundingHelper.RoundPrice here?
                        customerEnteredPricesEqual = Math.Round(sci.CustomerEnteredPrice, 2) == Math.Round(customerEnteredPrice, 2);

                    //rental products
                    bool rentalInfoEqual = true;
                    if (sci.Product.IsRental)
                    {
                        rentalInfoEqual = sci.RentalStartDateUtc == rentalStartDate && sci.RentalEndDateUtc == rentalEndDate;
                    }

                    //found?
                    if (attributesEqual && giftCardInfoSame && customerEnteredPricesEqual && rentalInfoEqual)
                        return sci;
                }
            }

            return null;
        }

        /// <summary>
        /// Add a product to shopping cart
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="product">Product</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">The price enter by a customer</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> AddToCart(Customer customer, Product product,
            ShoppingCartType shoppingCartType, int storeId, string attributesXml = null,
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool automaticallyAddRequiredProductsIfEnabled = true)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();
            if (shoppingCartType == ShoppingCartType.ShoppingCart && !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart, customer))
            {
                warnings.Add("Shopping cart is disabled");
                return warnings;
            }
            if (shoppingCartType == ShoppingCartType.Wishlist && !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist, customer))
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
                warnings.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));
                return warnings;
            }

            //reset checkout info
            _customerService.ResetCheckoutData(customer, storeId);

            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == shoppingCartType)
                .LimitPerStore(storeId)
                .ToList();

            var shoppingCartItem = FindShoppingCartItemInTheCart(cart,
                shoppingCartType, product, attributesXml, customerEnteredPrice,
                rentalStartDate, rentalEndDate);

            if (shoppingCartItem != null)
            {
                //update existing shopping cart item
                int newQuantity = shoppingCartItem.Quantity + quantity;
                warnings.AddRange(GetShoppingCartItemWarnings(customer, shoppingCartType, product,
                    storeId, attributesXml, 
                    customerEnteredPrice, rentalStartDate, rentalEndDate,
                    newQuantity, automaticallyAddRequiredProductsIfEnabled));

                if (!warnings.Any())
                {
                    shoppingCartItem.AttributesXml = attributesXml;
                    shoppingCartItem.Quantity = newQuantity;
                    shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;
                    _customerService.UpdateCustomer(customer);

                    //event notification
                    _eventPublisher.EntityUpdated(shoppingCartItem);
                }
            }
            else
            {
                //new shopping cart item
                warnings.AddRange(GetShoppingCartItemWarnings(customer, shoppingCartType, product,
                    storeId, attributesXml, customerEnteredPrice,
                    rentalStartDate, rentalEndDate, 
                    quantity, automaticallyAddRequiredProductsIfEnabled));
                if (!warnings.Any())
                {
                    //maximum items validation
                    switch (shoppingCartType)
                    {
                        case ShoppingCartType.ShoppingCart:
                            {
                                if (cart.Count >= _shoppingCartSettings.MaximumShoppingCartItems)
                                {
                                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumShoppingCartItems"), _shoppingCartSettings.MaximumShoppingCartItems));
                                    return warnings;
                                }
                            }
                            break;
                        case ShoppingCartType.Wishlist:
                            {
                                if (cart.Count >= _shoppingCartSettings.MaximumWishlistItems)
                                {
                                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumWishlistItems"), _shoppingCartSettings.MaximumWishlistItems));
                                    return warnings;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    DateTime now = DateTime.UtcNow;
                    shoppingCartItem = new ShoppingCartItem
                    {
                        ShoppingCartType = shoppingCartType,
                        StoreId = storeId,
                        Product = product,
                        AttributesXml = attributesXml,
                        CustomerEnteredPrice = customerEnteredPrice,
                        Quantity = quantity,
                        RentalStartDateUtc = rentalStartDate,
                        RentalEndDateUtc = rentalEndDate,
                        CreatedOnUtc = now,
                        UpdatedOnUtc = now
                    };
                    customer.ShoppingCartItems.Add(shoppingCartItem);
                    _customerService.UpdateCustomer(customer);


                    //updated "HasShoppingCartItems" property used for performance optimization
                    customer.HasShoppingCartItems = customer.ShoppingCartItems.Any();
                    _customerService.UpdateCustomer(customer);

                    //event notification
                    _eventPublisher.EntityInserted(shoppingCartItem);
                }
            }

            return warnings;
        }

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">New customer entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> UpdateShoppingCartItem(Customer customer,
            int shoppingCartItemId, string attributesXml,
            decimal customerEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null, 
            int quantity = 1, bool resetCheckoutData = true)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var warnings = new List<string>();

            var shoppingCartItem = customer.ShoppingCartItems.FirstOrDefault(sci => sci.Id == shoppingCartItemId);
            if (shoppingCartItem != null)
            {
                if (resetCheckoutData)
                {
                    //reset checkout data
                    _customerService.ResetCheckoutData(customer, shoppingCartItem.StoreId);
                }
                if (quantity > 0)
                {
                    //check warnings
                    warnings.AddRange(GetShoppingCartItemWarnings(customer, shoppingCartItem.ShoppingCartType,
                        shoppingCartItem.Product, shoppingCartItem.StoreId,
                        attributesXml, customerEnteredPrice, 
                        rentalStartDate, rentalEndDate, quantity, false));
                    if (!warnings.Any())
                    {
                        //if everything is OK, then update a shopping cart item
                        shoppingCartItem.Quantity = quantity;
                        shoppingCartItem.AttributesXml = attributesXml;
                        shoppingCartItem.CustomerEnteredPrice = customerEnteredPrice;
                        shoppingCartItem.RentalStartDateUtc = rentalStartDate;
                        shoppingCartItem.RentalEndDateUtc = rentalEndDate;
                        shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;
                        _customerService.UpdateCustomer(customer);

                        //event notification
                        _eventPublisher.EntityUpdated(shoppingCartItem);
                    }
                }
                else
                {
                    //delete a shopping cart item
                    DeleteShoppingCartItem(shoppingCartItem, resetCheckoutData, true);
                }
            }

            return warnings;
        }
        
        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromCustomer">From customer</param>
        /// <param name="toCustomer">To customer</param>
        /// <param name="includeCouponCodes">A value indicating whether to coupon codes (discount and gift card) should be also re-applied</param>
        public virtual void MigrateShoppingCart(Customer fromCustomer, Customer toCustomer, bool includeCouponCodes)
        {
            if (fromCustomer == null)
                throw new ArgumentNullException("fromCustomer");
            if (toCustomer == null)
                throw new ArgumentNullException("toCustomer");

            if (fromCustomer.Id == toCustomer.Id)
                return; //the same customer

            //shopping cart items
            var fromCart = fromCustomer.ShoppingCartItems.ToList();
            for (int i = 0; i < fromCart.Count; i++)
            {
                var sci = fromCart[i];
                AddToCart(toCustomer, sci.Product, sci.ShoppingCartType, sci.StoreId, 
                    sci.AttributesXml, sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false);
            }
            for (int i = 0; i < fromCart.Count; i++)
            {
                var sci = fromCart[i];
                DeleteShoppingCartItem(sci);
            }
            
            //migrate gift card and discount coupon codes
            if (includeCouponCodes)
            {
                //discount
                var discountCouponCode  = fromCustomer.GetAttribute<string>(SystemCustomerAttributeNames.DiscountCouponCode);
                if (!String.IsNullOrEmpty(discountCouponCode))
                    _genericAttributeService.SaveAttribute(toCustomer, SystemCustomerAttributeNames.DiscountCouponCode, discountCouponCode);
                
                //gift card
                foreach (var gcCode in fromCustomer.ParseAppliedGiftCardCouponCodes())
                    toCustomer.ApplyGiftCardCouponCode(gcCode);

                //save customer
                _customerService.UpdateCustomer(toCustomer);
                 
            }
        }

        #endregion
    }
}
