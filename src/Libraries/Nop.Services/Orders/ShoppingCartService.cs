using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;

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

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="sciRepository">Shopping cart repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="productService">Product settings</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="checkoutAttributeService">Checkout attribute service</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="shoppingCartSettings"></param>
        /// <param name="eventPublisher"></param>
        public ShoppingCartService(IRepository<ShoppingCartItem> sciRepository,
            IWorkContext workContext, ICurrencyService currencyService,
            IProductService productService, ILocalizationService localizationService,
            IProductAttributeParser productAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IPriceFormatter priceFormatter,
            ICustomerService customerService,
            ShoppingCartSettings shoppingCartSettings,
            IEventPublisher eventPublisher)
        {
            _sciRepository = sciRepository;
            _workContext = workContext;
            _currencyService = currencyService;
            _productService = productService;
            _localizationService = localizationService;
            _productAttributeParser = productAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _priceFormatter = priceFormatter;
            _customerService = customerService;
            _shoppingCartSettings = shoppingCartSettings;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get a list of required product variants
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <returns>Required product variants</returns>
        protected virtual IList<ProductVariant> GetRequiredProductVariants(ProductVariant productVariant)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            var requiredProductVariants = new List<ProductVariant>();

            var ids = productVariant.ParseRequiredProductVariantIds();
            foreach (var id in ids)
            {
                var pv = _productService.GetProductVariantById(id);
                var nowUtc = DateTime.UtcNow;
                //ensure that product and product variant are published, not deleted, etc
                if (pv != null &&
                    !pv.Deleted &&
                    pv.Published &&
                    !pv.Product.Deleted &&
                    pv.Product.Published &&
                    (!pv.AvailableStartDateTimeUtc.HasValue || pv.AvailableStartDateTimeUtc.Value < nowUtc) &&
                    (!pv.AvailableEndDateTimeUtc.HasValue || pv.AvailableEndDateTimeUtc.Value > nowUtc))
                {
                    requiredProductVariants.Add(pv);
                }
            }

            return requiredProductVariants;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        public virtual void DeleteShoppingCartItem(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException("shoppingCartItem");


            if (resetCheckoutData)
            {
                //reset checkout data
                _customerService.ResetCheckoutData(shoppingCartItem.Customer, false);
            }

            _sciRepository.Delete(shoppingCartItem);

            //event notification
            _eventPublisher.EntityDeleted(shoppingCartItem);
        }

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        public virtual void DeleteExpiredShoppingCartItems(DateTime olderThanUtc)
        {
            var query = from sci in _sciRepository.Table
                           where sci.UpdatedOnUtc < olderThanUtc
                           select sci;

            var cartItems = query.ToList();
            foreach (var cartItem in cartItems)
                _sciRepository.Delete(cartItem);
        }
        
        /// <summary>
        /// Validates required product variants
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="automaticallyAddRequiredProductVariantsIfEnabled">Automatically add required product variants if enabled</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetRequiredProductVariantWarnings(Customer customer, ShoppingCartType shoppingCartType,
            ProductVariant productVariant, bool automaticallyAddRequiredProductVariantsIfEnabled)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            var cart = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == shoppingCartType).ToList();

            var warnings = new List<string>();

            if (productVariant.RequireOtherProducts)
            {
                var requiredProductVariants = GetRequiredProductVariants(productVariant);
                foreach (var rpv in requiredProductVariants)
                {
                    //ensure that product is in the cart
                    bool alreadyInTheCart = false;
                    foreach (var sci in cart)
                    {
                        if (sci.ProductVariantId == rpv.Id)
                        {
                            alreadyInTheCart = true;
                            break;
                        }
                    }
                    //not in the cart
                    if (!alreadyInTheCart)
                    {

                        string fullProductName = "";
                        if (!String.IsNullOrEmpty(rpv.GetLocalized(x => x.Name)))
                            fullProductName = string.Format("{0} ({1})", rpv.Product.GetLocalized(x => x.Name), rpv.GetLocalized(x => x.Name));
                        else
                            fullProductName = rpv.Product.GetLocalized(x => x.Name);

                        if (productVariant.AutomaticallyAddRequiredProductVariants)
                        {
                            //add to cart (if possible)
                            if (automaticallyAddRequiredProductVariantsIfEnabled)
                            {
                                //pass 'false' for 'automaticallyAddRequiredProductVariantsIfEnabled' to prevent circular references
                                var addToCartWarnings = AddToCart(customer, rpv, shoppingCartType, "", decimal.Zero, 1, false);
                                if (addToCartWarnings.Count > 0)
                                {
                                    //a product wasn't atomatically added for some reasons

                                    //don't display specific errors from 'addToCartWarnings' variable
                                    //display only geenric error
                                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredProductWarning"), fullProductName));
                                }
                            }
                            else
                            {
                                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredProductWarning"), fullProductName));
                            }
                        }
                        else
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredProductWarning"), fullProductName));
                        }
                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemAttributeWarnings(ShoppingCartType shoppingCartType,
            ProductVariant productVariant, string selectedAttributes)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            var warnings = new List<string>();

            //selected attributes
            var pva1Collection = _productAttributeParser.ParseProductVariantAttributes(selectedAttributes);
            foreach (var pva1 in pva1Collection)
            {
                var pv1 = pva1.ProductVariant;
                if (pv1 != null)
                {
                    if (pv1.Id != productVariant.Id)
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

            //existing product attributes
            var pva2Collection = productVariant.ProductVariantAttributes;
            foreach (var pva2 in pva2Collection)
            {
                if (pva2.IsRequired)
                {
                    bool found = false;
                    //selected product attributes
                    foreach (var pva1 in pva1Collection)
                    {
                        if (pva1.Id == pva2.Id)
                        {
                            var pvaValuesStr = _productAttributeParser.ParseValues(selectedAttributes, pva1.Id);
                            foreach (string str1 in pvaValuesStr)
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
                        if (!string.IsNullOrEmpty(pva2.TextPrompt))
                        {
                            warnings.Add(pva2.TextPrompt);
                        }
                        else
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"), pva2.ProductAttribute.GetLocalized(a => a.Name)));
                        }
                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemGiftCardWarnings(ShoppingCartType shoppingCartType,
            ProductVariant productVariant, string selectedAttributes)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            var warnings = new List<string>();

            //gift cards
            if (productVariant.IsGiftCard)
            {
                string giftCardRecipientName = string.Empty;
                string giftCardRecipientEmail = string.Empty;
                string giftCardSenderName = string.Empty;
                string giftCardSenderEmail = string.Empty;
                string giftCardMessage = string.Empty;
                _productAttributeParser.GetGiftCardAttribute(selectedAttributes,
                    out giftCardRecipientName, out giftCardRecipientEmail,
                    out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                if (String.IsNullOrEmpty(giftCardRecipientName))
                    warnings.Add(_localizationService.GetResource("ShoppingCartWarning.RecipientNameError"));

                if (productVariant.GiftCardType == GiftCardType.Virtual)
                {
                    //validate for virtual gift cards only
                    if (String.IsNullOrEmpty(giftCardRecipientEmail) || !CommonHelper.IsValidEmail(giftCardRecipientEmail))
                        warnings.Add(_localizationService.GetResource("ShoppingCartWarning.RecipientEmailError"));
                }

                if (String.IsNullOrEmpty(giftCardSenderName))
                    warnings.Add(_localizationService.GetResource("ShoppingCartWarning.SenderNameError"));

                if (productVariant.GiftCardType == GiftCardType.Virtual)
                {
                    //validate for virtual gift cards only
                    if (String.IsNullOrEmpty(giftCardSenderEmail) || !CommonHelper.IsValidEmail(giftCardSenderEmail))
                        warnings.Add(_localizationService.GetResource("ShoppingCartWarning.SenderEmailError"));
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredProductVariantsIfEnabled">Automatically add required product variants if enabled</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemWarnings(Customer customer, ShoppingCartType shoppingCartType,
            ProductVariant productVariant, string selectedAttributes, decimal customerEnteredPrice,
            int quantity, bool automaticallyAddRequiredProductVariantsIfEnabled)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            var warnings = new List<string>();

            var product = productVariant.Product;
            if (product == null)
            {
                warnings.Add(string.Format("Product (Id={0}) can not be loaded", productVariant.ProductId));
                return warnings;
            }

            if (product.Deleted || productVariant.Deleted)
            {
                warnings.Add("Product is deleted");
                return warnings;
            }

            if (!product.Published || !productVariant.Published)
            {
                warnings.Add("Product is not published");
            }

            if (shoppingCartType == ShoppingCartType.ShoppingCart && productVariant.DisableBuyButton)
            {
                warnings.Add("Buying is disabled for this product");
            }

            if (shoppingCartType == ShoppingCartType.Wishlist && productVariant.DisableWishlistButton)
            {
                warnings.Add("Wishlist is disabled for this product");
            }
            
            if (shoppingCartType == ShoppingCartType.ShoppingCart &&
                productVariant.CallForPrice)
            {
                warnings.Add(_localizationService.GetResource("Products.CallForPrice"));
            }

            if (productVariant.CustomerEntersPrice)
            {
                if (customerEnteredPrice < productVariant.MinimumCustomerEnteredPrice ||
                    customerEnteredPrice > productVariant.MaximumCustomerEnteredPrice)
                {
                    decimal minimumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(productVariant.MinimumCustomerEnteredPrice, _workContext.WorkingCurrency);
                    decimal maximumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(productVariant.MaximumCustomerEnteredPrice, _workContext.WorkingCurrency);
                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.CustomerEnteredPrice.RangeError"),
                        _priceFormatter.FormatPrice(minimumCustomerEnteredPrice, false, false),
                        _priceFormatter.FormatPrice(maximumCustomerEnteredPrice, false, false)));
                }
            }

            if (quantity < productVariant.OrderMinimumQuantity)
            {
                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MinimumQuantity"), productVariant.OrderMinimumQuantity));
            }

            if (quantity > productVariant.OrderMaximumQuantity)
            {
                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumQuantity"), productVariant.OrderMaximumQuantity));
            }

            switch (productVariant.ManageInventoryMethod)
            {
                case ManageInventoryMethod.DontManageStock:
                    {
                    }
                    break;
                case ManageInventoryMethod.ManageStock:
                    {
                        if ((BackorderMode)productVariant.BackorderMode == BackorderMode.NoBackorders)
                        {
                            if (productVariant.StockQuantity < quantity)
                            {
                                int maximumQuantityCanBeAdded = productVariant.StockQuantity;
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
                        var combinations = productVariant.ProductVariantAttributeCombinations;
                        ProductVariantAttributeCombination combination = null;
                        foreach (var comb1 in combinations)
                            if (_productAttributeParser.AreProductAttributesEqual(comb1.AttributesXml, selectedAttributes))
                                combination = comb1;
                        if (combination != null)
                        {
                            if (!combination.AllowOutOfStockOrders)
                            {
                                if (combination.StockQuantity < quantity)
                                {
                                    int maximumQuantityCanBeAdded = combination.StockQuantity;
                                    if (maximumQuantityCanBeAdded <= 0)
                                        warnings.Add(_localizationService.GetResource("ShoppingCart.OutOfStock"));
                                    else
                                        warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            //availability dates
            bool availableStartDateError = false;
            if (productVariant.AvailableStartDateTimeUtc.HasValue)
            {
                DateTime now = DateTime.UtcNow;
                DateTime availableStartDateTime = DateTime.SpecifyKind(productVariant.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableStartDateTime.CompareTo(now) > 0)
                {
                    warnings.Add("Product is not available");
                    availableStartDateError = true;
                }
            }
            if (productVariant.AvailableEndDateTimeUtc.HasValue && !availableStartDateError)
            {
                DateTime now = DateTime.UtcNow;
                DateTime availableEndDateTime = DateTime.SpecifyKind(productVariant.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableEndDateTime.CompareTo(now) < 0)
                {
                    warnings.Add("Product is not available");
                }
            }

            //selected attributes
            warnings.AddRange(GetShoppingCartItemAttributeWarnings(shoppingCartType, productVariant, selectedAttributes));

            //gift cards
            warnings.AddRange(GetShoppingCartItemGiftCardWarnings(shoppingCartType, productVariant, selectedAttributes));

            //required product variants
            warnings.AddRange(GetRequiredProductVariantWarnings(customer, shoppingCartType, productVariant, automaticallyAddRequiredProductVariantsIfEnabled));

            return warnings;
        }

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartWarnings(IList<ShoppingCartItem> shoppingCart, 
            string checkoutAttributes, bool validateCheckoutAttributes)
        {
            var warnings = new List<string>();

            bool hasStandartProducts = false;
            bool hasRecurringProducts = false;

            foreach (var sci in shoppingCart)
            {
                var productVariant = sci.ProductVariant;
                if (productVariant == null)
                {
                    warnings.Add(string.Format("Product variant (Id={0}) can not be loaded", sci.ProductVariantId));
                    return warnings;
                }

                if (productVariant.IsRecurring)
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
                int cycleLength = 0;
                RecurringProductCyclePeriod cyclePeriod =  RecurringProductCyclePeriod.Days;
                int totalCycles = 0;
                string cyclesError = shoppingCart.GetReccuringCycleInfo(out cycleLength, out cyclePeriod, out totalCycles);
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
                var ca1Collection = _checkoutAttributeParser.ParseCheckoutAttributes(checkoutAttributes);

                //existing checkout attributes
                bool shoppingCartRequiresShipping = shoppingCart.RequiresShipping();
                var ca2Collection = _checkoutAttributeService.GetAllCheckoutAttributes(!shoppingCartRequiresShipping);
                foreach (var ca2 in ca2Collection)
                {
                    if (ca2.IsRequired)
                    {
                        bool found = false;
                        //selected checkout attributes
                        foreach (var ca1 in ca1Collection)
                        {
                            if (ca1.Id == ca2.Id)
                            {
                                var caValuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributes, ca1.Id);
                                foreach (string str1 in caValuesStr)
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
                            if (!string.IsNullOrEmpty(ca2.GetLocalized(a => a.TextPrompt)))
                                warnings.Add(ca2.GetLocalized(a => a.TextPrompt));
                            else
                                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"), ca2.GetLocalized(a => a.Name)));
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
        /// <param name="productVariant">Product variant</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">Price entered by a customer</param>
        /// <returns>Found shopping cart item</returns>
        public virtual ShoppingCartItem FindShoppingCartItemInTheCart(IList<ShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
            ProductVariant productVariant,
            string selectedAttributes = "",
            decimal customerEnteredPrice = decimal.Zero)
        {
            if (shoppingCart == null)
                throw new ArgumentNullException("shoppingCart");

            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            foreach (var sci in shoppingCart.Where(a => a.ShoppingCartType == shoppingCartType))
            {
                if (sci.ProductVariantId == productVariant.Id)
                {
                    //attributes
                    bool attributesEqual = _productAttributeParser.AreProductAttributesEqual(sci.AttributesXml, selectedAttributes);

                    //gift cards
                    bool giftCardInfoSame = true;
                    if (sci.ProductVariant.IsGiftCard)
                    {
                        string giftCardRecipientName1 = string.Empty;
                        string giftCardRecipientEmail1 = string.Empty;
                        string giftCardSenderName1 = string.Empty;
                        string giftCardSenderEmail1 = string.Empty;
                        string giftCardMessage1 = string.Empty;
                        _productAttributeParser.GetGiftCardAttribute(selectedAttributes,
                            out giftCardRecipientName1, out giftCardRecipientEmail1,
                            out giftCardSenderName1, out giftCardSenderEmail1, out giftCardMessage1);

                        string giftCardRecipientName2 = string.Empty;
                        string giftCardRecipientEmail2 = string.Empty;
                        string giftCardSenderName2 = string.Empty;
                        string giftCardSenderEmail2 = string.Empty;
                        string giftCardMessage2 = string.Empty;
                        _productAttributeParser.GetGiftCardAttribute(sci.AttributesXml,
                            out giftCardRecipientName2, out giftCardRecipientEmail2,
                            out giftCardSenderName2, out giftCardSenderEmail2, out giftCardMessage2);


                        if (giftCardRecipientName1.ToLowerInvariant() != giftCardRecipientName2.ToLowerInvariant() ||
                            giftCardSenderName1.ToLowerInvariant() != giftCardSenderName2.ToLowerInvariant())
                            giftCardInfoSame = false;
                    }

                    //price is the same (for products which require customers to enter a price)
                    bool customerEnteredPricesEqual = true;
                    if (sci.ProductVariant.CustomerEntersPrice)
                        customerEnteredPricesEqual = Math.Round(sci.CustomerEnteredPrice, 2) == Math.Round(customerEnteredPrice, 2);

                    //found?
                    if (attributesEqual && giftCardInfoSame && customerEnteredPricesEqual)
                        return sci;
                }
            }

            return null;
        }

        /// <summary>
        /// Add a product variant to shopping cart
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">The price enter by a customer</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredProductVariantsIfEnabled">Automatically add required product variants if enabled</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> AddToCart(Customer customer, ProductVariant productVariant, 
            ShoppingCartType shoppingCartType, string selectedAttributes,
            decimal customerEnteredPrice, int quantity, bool automaticallyAddRequiredProductVariantsIfEnabled)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            var warnings = new List<string>();
            if (shoppingCartType == ShoppingCartType.Wishlist && !_shoppingCartSettings.WishlistEnabled)
            {
                warnings.Add("Wishlits is disabled");
                return warnings;
            }


            //reset checkout info
            _customerService.ResetCheckoutData(customer, false);

            var cart = customer.ShoppingCartItems.Where(sci=>sci.ShoppingCartType == shoppingCartType).ToList();

            ShoppingCartItem shoppingCartItem = FindShoppingCartItemInTheCart(cart,
                shoppingCartType, productVariant, selectedAttributes, customerEnteredPrice);

            if (shoppingCartItem != null)
            {
                //update existing shopping cart item
                int newQuantity = shoppingCartItem.Quantity + quantity;
                warnings.AddRange(GetShoppingCartItemWarnings(customer, shoppingCartType, productVariant,
                    selectedAttributes, customerEnteredPrice, newQuantity, automaticallyAddRequiredProductVariantsIfEnabled));

                if (warnings.Count == 0)
                {
                    shoppingCartItem.AttributesXml = selectedAttributes;
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
                warnings.AddRange(GetShoppingCartItemWarnings(customer, shoppingCartType, productVariant,
                    selectedAttributes, customerEnteredPrice, quantity, automaticallyAddRequiredProductVariantsIfEnabled));
                if (warnings.Count == 0)
                {
                    //maximum items validation
                    switch (shoppingCartType)
                    {
                        case ShoppingCartType.ShoppingCart:
                            {
                                if (cart.Count >= _shoppingCartSettings.MaximumShoppingCartItems)
                                    return warnings;
                            }
                            break;
                        case ShoppingCartType.Wishlist:
                            {
                                if (cart.Count >= _shoppingCartSettings.MaximumWishlistItems)
                                    return warnings;
                            }
                            break;
                        default:
                            break;
                    }

                    DateTime now = DateTime.UtcNow;
                    customer.ShoppingCartItems.Add(new ShoppingCartItem()
                    {
                        ShoppingCartType = shoppingCartType,
                        ProductVariant = productVariant,
                        AttributesXml = selectedAttributes,
                        CustomerEnteredPrice = customerEnteredPrice,
                        Quantity = quantity,
                        CreatedOnUtc = now,
                        UpdatedOnUtc = now
                    });
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
        /// <param name="newQuantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> UpdateShoppingCartItem(Customer customer, int shoppingCartItemId, 
            int newQuantity, bool resetCheckoutData)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var warnings = new List<string>();

            var shoppingCartItem = customer.ShoppingCartItems.Where(sci => sci.Id == shoppingCartItemId).FirstOrDefault();
            if (shoppingCartItem != null)
            {
                if (resetCheckoutData)
                {
                    //reset checkout data
                    _customerService.ResetCheckoutData(customer, false);
                }
                if (newQuantity > 0)
                {
                    //check warnings
                    warnings.AddRange(GetShoppingCartItemWarnings(customer, shoppingCartItem.ShoppingCartType,
                        shoppingCartItem.ProductVariant, shoppingCartItem.AttributesXml,
                        shoppingCartItem.CustomerEnteredPrice, newQuantity, false));
                    if (warnings.Count == 0)
                    {
                        //if everything is OK, then update a shopping cart item
                        shoppingCartItem.Quantity = newQuantity;
                        shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;
                        _customerService.UpdateCustomer(customer);

                        //event notification
                        _eventPublisher.EntityUpdated(shoppingCartItem);
                    }
                }
                else
                {
                    //delete a shopping cart item
                    //customer.ShoppingCartItems.Remove(shoppingCartItem);
                    //_customerService.UpdateCustomer(customer);
                    //_sciRepository.Delete(shoppingCartItem);
                    //if (resetCheckoutData)
                    //    _customerService.ResetCheckoutData(customer, false);
                    DeleteShoppingCartItem(shoppingCartItem, resetCheckoutData);
                }
            }

            return warnings;
        }

        /// <summary>
        /// Direct add to cart allowed
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="productVariantId">Default product variant identifier for adding to cart</param>
        /// <returns>A value indicating whether direct add to cart is allowed</returns>
        public virtual bool DirectAddToCartAllowed(int productId, out int productVariantId)
        {
            bool result = false;
            productVariantId = 0;
            var productVariants = _productService.GetProductVariantsByProductId(productId);
            if (productVariants.Count == 1)
            {
                var defaultProductVariant = productVariants[0];
                if (!defaultProductVariant.CustomerEntersPrice)
                {
                    var addToCartWarnings = GetShoppingCartItemWarnings(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart,
                        defaultProductVariant, string.Empty, decimal.Zero, 1, false);

                    if (addToCartWarnings.Count == 0)
                    {
                        productVariantId = defaultProductVariant.Id;
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromCustomer">From customer</param>
        /// <param name="toCustomer">To customer</param>
        public virtual void MigrateShoppingCart(Customer fromCustomer, Customer toCustomer)
        {
            if (fromCustomer == null)
                throw new ArgumentNullException("fromCustomer");
            if (toCustomer == null)
                throw new ArgumentNullException("toCustomer");

            if (fromCustomer.Id == toCustomer.Id)
                return; //the same customer

            var fromCart = fromCustomer.ShoppingCartItems.ToList();
            for (int i = 0; i < fromCart.Count; i++)
            {
                var sci = fromCart[i];
                AddToCart(toCustomer, sci.ProductVariant, sci.ShoppingCartType,
                    sci.AttributesXml, sci.CustomerEnteredPrice, sci.Quantity, false);
            }
            for (int i = 0; i < fromCart.Count; i++)
            {
                var sci = fromCart[i];
                DeleteShoppingCartItem(sci);
            }

            //TODO apply current discount & gift card codes
        }

        #endregion
    }
}
