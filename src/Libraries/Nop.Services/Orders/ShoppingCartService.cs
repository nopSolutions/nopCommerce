using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;

namespace Nop.Services.Orders;

/// <summary>
/// Shopping cart service
/// </summary>
public partial class ShoppingCartService : IShoppingCartService
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAclService _aclService;
    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    protected readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateRangeService _dateRangeService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IRepository<ShoppingCartItem> _sciRepository;
    protected readonly IShippingService _shippingService;
    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly OrderSettings _orderSettings;
    protected readonly ShoppingCartSettings _shoppingCartSettings;

    #endregion

    #region Ctor

    public ShoppingCartService(CatalogSettings catalogSettings,
        IAclService aclService,
        IActionContextAccessor actionContextAccessor,
        IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IDateRangeService dateRangeService,
        IDateTimeHelper dateTimeHelper,
        IEventPublisher eventPublisher,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IPriceCalculationService priceCalculationService,
        IPriceFormatter priceFormatter,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IRepository<ShoppingCartItem> sciRepository,
        IShippingService shippingService,
        IShortTermCacheManager shortTermCacheManager,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreService storeService,
        IStoreMappingService storeMappingService,
        IUrlHelperFactory urlHelperFactory,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        OrderSettings orderSettings,
        ShoppingCartSettings shoppingCartSettings)
    {
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _actionContextAccessor = actionContextAccessor;
        _checkoutAttributeParser = checkoutAttributeParser;
        _checkoutAttributeService = checkoutAttributeService;
        _currencyService = currencyService;
        _customerService = customerService;
        _dateRangeService = dateRangeService;
        _dateTimeHelper = dateTimeHelper;
        _eventPublisher = eventPublisher;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _priceCalculationService = priceCalculationService;
        _priceFormatter = priceFormatter;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _sciRepository = sciRepository;
        _shippingService = shippingService;
        _shortTermCacheManager = shortTermCacheManager;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeService = storeService;
        _storeMappingService = storeMappingService;
        _urlHelperFactory = urlHelperFactory;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _orderSettings = orderSettings;
        _shoppingCartSettings = shoppingCartSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Determine if the shopping cart item is the same as the one being compared
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="customerEnteredPrice">Price entered by a customer</param>
    /// <param name="rentalStartDate">Rental start date</param>
    /// <param name="rentalEndDate">Rental end date</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart item is equal
    /// </returns>
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

    /// <summary>
    /// Gets a value indicating whether customer shopping cart is empty
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>Result</returns>
    protected virtual bool IsCustomerShoppingCartEmpty(Customer customer)
    {
        return !_sciRepository.Table.Any(sci => sci.CustomerId == customer.Id);
    }

    /// <summary>
    /// Validates required products (products which require some other products to be added to the cart)
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="product">Product</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="addRequiredProducts">Whether to add required products</param>
    /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    protected virtual async Task<IList<string>> GetRequiredProductWarningsAsync(Customer customer, ShoppingCartType shoppingCartType, Product product,
        int storeId, int quantity, bool addRequiredProducts, int shoppingCartItemId)
    {
        ArgumentNullException.ThrowIfNull(customer);

        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();

        //at now we ignore quantities of required products and use 1
        var requiredProductQuantity = 1;

        //get customer shopping cart
        var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);

        var productsRequiringProduct = await GetProductsRequiringProductAsync(cart, product);

        //whether other cart items require the passed product
        var passedProductRequiredQuantity = cart.Where(ci => productsRequiringProduct.Any(p => p.Id == ci.ProductId))
            .Sum(item => item.Quantity * requiredProductQuantity);

        if (passedProductRequiredQuantity > quantity)
            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.RequiredProductUpdateWarning"), passedProductRequiredQuantity));

        //whether the passed product requires other products
        if (!product.RequireOtherProducts)
            return warnings;

        //get these required products
        var requiredProducts = await _productService.GetProductsByIdsAsync(_productService.ParseRequiredProductIds(product));
        if (!requiredProducts.Any())
            return warnings;

        var finalRequiredProducts = requiredProducts.GroupBy(p => p.Id)
            .Select(g => new { Product = g.First(), Count = g.Count() });

        //get warnings
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        var warningLocale = await _localizationService.GetResourceAsync("ShoppingCart.RequiredProductWarning");
        foreach (var requiredProduct in finalRequiredProducts)
        {
            var productsRequiringRequiredProduct = await GetProductsRequiringProductAsync(cart, requiredProduct.Product);

            //get the required quantity of the required product
            var requiredProductRequiredQuantity = quantity * requiredProductQuantity +
                                                  cart.Where(ci => productsRequiringRequiredProduct.Any(p => p.Id == ci.ProductId))
                                                      .Where(item => item.Id != shoppingCartItemId)
                                                      .Sum(item => item.Quantity * requiredProductQuantity);

            //whether required product is already in the cart in the required quantity
            var quantityToAdd = requiredProductRequiredQuantity * requiredProduct.Count - (cart.FirstOrDefault(item => item.ProductId == requiredProduct.Product.Id)?.Quantity ?? 0);
            if (quantityToAdd <= 0)
                continue;

            //prepare warning message
            var url = urlHelper.RouteUrl(nameof(Product), new { SeName = await _urlRecordService.GetSeNameAsync(requiredProduct.Product) });
            var requiredProductName = WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(requiredProduct.Product, x => x.Name));
            var requiredProductWarning = _catalogSettings.UseLinksInRequiredProductWarnings
                ? string.Format(warningLocale, $"<a href=\"{url}\">{requiredProductName}</a>", requiredProductRequiredQuantity * requiredProduct.Count)
                : string.Format(warningLocale, requiredProductName, requiredProductRequiredQuantity);

            //add to cart (if possible)
            if (addRequiredProducts && product.AutomaticallyAddRequiredProducts)
            {
                //do not add required products to prevent circular references
                var addToCartWarnings = await GetShoppingCartItemWarningsAsync(
                    customer: customer,
                    product: requiredProduct.Product,
                    attributesXml: null,
                    customerEnteredPrice: decimal.Zero,
                    shoppingCartType: shoppingCartType,
                    storeId: storeId,
                    quantity: quantityToAdd,
                    addRequiredProducts: true);

                //don't display all specific errors only the generic one
                if (addToCartWarnings.Any())
                    warnings.Add(requiredProductWarning);
            }
            else
            {
                warnings.Add(requiredProductWarning);
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
    /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    protected virtual async Task<IList<string>> GetStandardWarningsAsync(Customer customer, ShoppingCartType shoppingCartType, Product product,
        string attributesXml, decimal customerEnteredPrice, int quantity, int shoppingCartItemId, int storeId)
    {
        ArgumentNullException.ThrowIfNull(customer);

        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();

        //deleted
        if (product.Deleted)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductDeleted"));
            return warnings;
        }

        //published
        if (!product.Published)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductUnpublished"));
        }

        //we can add only simple products
        if (product.ProductType != ProductType.SimpleProduct)
        {
            warnings.Add("This is not simple product");
        }

        //ACL
        if (!await _aclService.AuthorizeAsync(product, customer))
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductUnpublished"));
        }

        //Store mapping
        if (!await _storeMappingService.AuthorizeAsync(product, storeId))
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductUnpublished"));
        }

        //disabled "add to cart" button
        if (shoppingCartType == ShoppingCartType.ShoppingCart && product.DisableBuyButton)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.BuyingDisabled"));
        }

        //disabled "add to wishlist" button
        if (shoppingCartType == ShoppingCartType.Wishlist && product.DisableWishlistButton)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.WishlistDisabled"));
        }

        //call for price
        if (shoppingCartType == ShoppingCartType.ShoppingCart && product.CallForPrice &&
            //also check whether the current user is impersonated
            (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
        {
            warnings.Add(await _localizationService.GetResourceAsync("Products.CallForPrice"));
        }

        //customer entered price
        if (product.CustomerEntersPrice)
        {
            if (customerEnteredPrice < product.MinimumCustomerEnteredPrice ||
                customerEnteredPrice > product.MaximumCustomerEnteredPrice)
            {
                var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
                var minimumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MinimumCustomerEnteredPrice, currentCurrency);
                var maximumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MaximumCustomerEnteredPrice, currentCurrency);
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.CustomerEnteredPrice.RangeError"),
                    await _priceFormatter.FormatPriceAsync(minimumCustomerEnteredPrice, false, false),
                    await _priceFormatter.FormatPriceAsync(maximumCustomerEnteredPrice, false, false)));
            }
        }

        //quantity validation
        var hasQtyWarnings = false;
        if (quantity < product.OrderMinimumQuantity)
        {
            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MinimumQuantity"), product.OrderMinimumQuantity));
            hasQtyWarnings = true;
        }

        if (quantity > product.OrderMaximumQuantity)
        {
            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumQuantity"), product.OrderMaximumQuantity));
            hasQtyWarnings = true;
        }

        var allowedQuantities = _productService.ParseAllowedQuantities(product);
        if (allowedQuantities.Length > 0 && !allowedQuantities.Contains(quantity))
        {
            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AllowedQuantities"), string.Join(", ", allowedQuantities)));
        }

        var validateOutOfStock = shoppingCartType == ShoppingCartType.ShoppingCart || !_shoppingCartSettings.AllowOutOfStockItemsToBeAddedToWishlist;
        if (validateOutOfStock && !hasQtyWarnings)
        {
            switch (product.ManageInventoryMethod)
            {
                case ManageInventoryMethod.DontManageStock:
                    //do nothing
                    break;
                case ManageInventoryMethod.ManageStock:
                    if (product.BackorderMode == BackorderMode.NoBackorders)
                    {
                        var maximumQuantityCanBeAdded = await _productService.GetTotalStockQuantityAsync(product);

                        warnings.AddRange(await GetQuantityProductWarningsAsync(product, quantity, maximumQuantityCanBeAdded));

                        if (warnings.Any())
                            return warnings;

                        //validate product quantity with non combinable product attributes
                        var productAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                        if (productAttributeMappings?.Any() == true)
                        {
                            var onlyCombinableAttributes = productAttributeMappings.All(mapping => !mapping.IsNonCombinable());
                            if (!onlyCombinableAttributes)
                            {
                                var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);
                                var totalAddedQuantity = cart
                                    .Where(item => item.ProductId == product.Id && item.Id != shoppingCartItemId)
                                    .Sum(product => product.Quantity);

                                totalAddedQuantity += quantity;

                                //counting a product into bundles
                                foreach (var bundle in cart.Where(x => x.Id != shoppingCartItemId && !string.IsNullOrEmpty(x.AttributesXml)))
                                {
                                    var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(bundle.AttributesXml);
                                    foreach (var attributeValue in attributeValues)
                                    {
                                        if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct && attributeValue.AssociatedProductId == product.Id)
                                            totalAddedQuantity += bundle.Quantity * attributeValue.Quantity;
                                    }
                                }

                                warnings.AddRange(await GetQuantityProductWarningsAsync(product, totalAddedQuantity, maximumQuantityCanBeAdded));
                            }
                        }

                        if (warnings.Any())
                            return warnings;

                        //validate product quantity and product quantity into bundles
                        if (string.IsNullOrEmpty(attributesXml))
                        {
                            var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);
                            var totalQuantityInCart = cart.Where(item => item.ProductId == product.Id && item.Id != shoppingCartItemId && string.IsNullOrEmpty(item.AttributesXml))
                                .Sum(product => product.Quantity);

                            totalQuantityInCart += quantity;

                            foreach (var bundle in cart.Where(x => x.Id != shoppingCartItemId && !string.IsNullOrEmpty(x.AttributesXml)))
                            {
                                var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(bundle.AttributesXml);
                                foreach (var attributeValue in attributeValues)
                                {
                                    if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct && attributeValue.AssociatedProductId == product.Id)
                                        totalQuantityInCart += bundle.Quantity * attributeValue.Quantity;
                                }
                            }

                            warnings.AddRange(await GetQuantityProductWarningsAsync(product, totalQuantityInCart, maximumQuantityCanBeAdded));
                        }
                    }

                    break;
                case ManageInventoryMethod.ManageStockByAttributes:
                    var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
                    if (combination != null)
                    {
                        //combination exists
                        //let's check stock level
                        if (!combination.AllowOutOfStockOrders)
                            warnings.AddRange(await GetQuantityProductWarningsAsync(product, quantity, combination.StockQuantity));
                    }
                    else
                    {
                        //combination doesn't exist
                        if (product.AllowAddingOnlyExistingAttributeCombinations)
                        {
                            //maybe, is it better  to display something like "No such product/combination" message?
                            var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
                            var warning = productAvailabilityRange == null ? await _localizationService.GetResourceAsync("ShoppingCart.OutOfStock")
                                : string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AvailabilityRange"),
                                    await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                            warnings.Add(warning);
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        //availability dates
        var availableStartDateError = false;
        if (product.AvailableStartDateTimeUtc.HasValue)
        {
            var availableStartDateTime = DateTime.SpecifyKind(product.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
            if (availableStartDateTime.CompareTo(DateTime.UtcNow) > 0)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.NotAvailable"));
                availableStartDateError = true;
            }
        }

        if (!product.AvailableEndDateTimeUtc.HasValue || availableStartDateError)
            return warnings;

        var availableEndDateTime = DateTime.SpecifyKind(product.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
        if (availableEndDateTime.CompareTo(DateTime.UtcNow) < 0)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.NotAvailable"));
        }

        return warnings;
    }

    /// <summary>
    /// Validates the maximum quantity a product can be added 
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="maximumQuantityCanBeAdded">The maximum quantity a product can be added</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings 
    /// </returns>
    protected virtual async Task<IList<string>> GetQuantityProductWarningsAsync(Product product, int quantity, int maximumQuantityCanBeAdded)
    {
        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();

        if (maximumQuantityCanBeAdded < quantity)
        {
            if (maximumQuantityCanBeAdded <= 0)
            {
                var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
                var warning = productAvailabilityRange == null ? await _localizationService.GetResourceAsync("ShoppingCart.OutOfStock")
                    : string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AvailabilityRange"),
                        await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                warnings.Add(warning);
            }
            else
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
        }

        return warnings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Delete shopping cart item
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
    /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteShoppingCartItemAsync(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true,
        bool ensureOnlyActiveCheckoutAttributes = false)
    {
        ArgumentNullException.ThrowIfNull(shoppingCartItem);

        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);
        var storeId = shoppingCartItem.StoreId;

        //reset checkout data
        if (resetCheckoutData)
            await _customerService.ResetCheckoutDataAsync(customer, shoppingCartItem.StoreId);

        //delete item
        await _sciRepository.DeleteAsync(shoppingCartItem);

        //reset "HasShoppingCartItems" property used for performance optimization
        var hasShoppingCartItems = !IsCustomerShoppingCartEmpty(customer);
        if (hasShoppingCartItems != customer.HasShoppingCartItems)
        {
            customer.HasShoppingCartItems = hasShoppingCartItems;
            await _customerService.UpdateCustomerAsync(customer);
        }

        //validate checkout attributes
        if (ensureOnlyActiveCheckoutAttributes &&
            //only for shopping cart items (ignore wishlist)
            shoppingCartItem.ShoppingCartType == ShoppingCartType.ShoppingCart)
        {
            var cart = await GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, storeId);

            var checkoutAttributesXml =
                await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CheckoutAttributes,
                    storeId);
            checkoutAttributesXml =
                await _checkoutAttributeParser.EnsureOnlyActiveAttributesAsync(checkoutAttributesXml, cart);
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CheckoutAttributes,
                checkoutAttributesXml, storeId);
        }

        if (!_catalogSettings.RemoveRequiredProducts)
            return;

        var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);
        if (!product?.RequireOtherProducts ?? true)
            return;

        var requiredProductIds = _productService.ParseRequiredProductIds(product);
        var requiredShoppingCartItems =
            (await GetShoppingCartAsync(customer, shoppingCartType: shoppingCartItem.ShoppingCartType))
            .Where(item => requiredProductIds.Any(id => id == item.ProductId))
            .ToList();

        //update quantity of required products in the cart if the main one is removed
        foreach (var cartItem in requiredShoppingCartItems)
        {
            //at now we ignore quantities of required products and use 1
            var requiredProductQuantity = 1;

            await UpdateShoppingCartItemAsync(customer, cartItem.Id, cartItem.AttributesXml, cartItem.CustomerEnteredPrice,
                quantity: cartItem.Quantity - shoppingCartItem.Quantity * requiredProductQuantity,
                resetCheckoutData: false);
        }
    }

    /// <summary>
    /// Clear shopping cart
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Store ID</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ClearShoppingCartAsync(Customer customer, int storeId)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var cart = await GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, storeId);

        //delete items
        await _sciRepository.DeleteAsync(cart, publishEvent: false);
        await _eventPublisher.PublishAsync(new ClearShoppingCartEvent(cart));

        //reset "HasShoppingCartItems" property used for performance optimization
        var hasShoppingCartItems = !IsCustomerShoppingCartEmpty(customer);
        if (hasShoppingCartItems != customer.HasShoppingCartItems)
        {
            customer.HasShoppingCartItems = hasShoppingCartItems;
            await _customerService.UpdateCustomerAsync(customer);
        }
    }

    /// <summary>
    /// Delete shopping cart item
    /// </summary>
    /// <param name="shoppingCartItemId">Shopping cart item ID</param>
    /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
    /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteShoppingCartItemAsync(int shoppingCartItemId, bool resetCheckoutData = true,
        bool ensureOnlyActiveCheckoutAttributes = false)
    {
        var shoppingCartItem = await _sciRepository.Table.FirstOrDefaultAsync(sci => sci.Id == shoppingCartItemId);
        if (shoppingCartItem != null)
            await DeleteShoppingCartItemAsync(shoppingCartItem, resetCheckoutData, ensureOnlyActiveCheckoutAttributes);
    }

    /// <summary>
    /// Deletes expired shopping cart items
    /// </summary>
    /// <param name="olderThanUtc">Older than date and time</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of deleted items
    /// </returns>
    public virtual async Task<int> DeleteExpiredShoppingCartItemsAsync(DateTime olderThanUtc)
    {
        var query = from sci in _sciRepository.Table
            where sci.UpdatedOnUtc < olderThanUtc
            select sci;

        var cartItems = await query.ToListAsync();

        foreach (var cartItem in cartItems)
            await DeleteShoppingCartItemAsync(cartItem);

        return cartItems.Count;
    }

    /// <summary>
    /// Get products from shopping cart whether requiring specific product
    /// </summary>
    /// <param name="cart">Shopping cart </param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<Product>> GetProductsRequiringProductAsync(IList<ShoppingCartItem> cart, Product product)
    {
        ArgumentNullException.ThrowIfNull(cart);

        ArgumentNullException.ThrowIfNull(product);

        if (!cart.Any())
            return new List<Product>();

        var productIds = cart.Select(ci => ci.ProductId).ToArray();

        var cartProducts = await _productService.GetProductsByIdsAsync(productIds);

        return cartProducts.Where(cartProduct =>
            cartProduct.RequireOtherProducts &&
            _productService.ParseRequiredProductIds(cartProduct).Contains(product.Id)).ToList();
    }

    /// <summary>
    /// Gets shopping cart
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="shoppingCartType">Shopping cart type; pass null to load all records</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="productId">Product identifier; pass null to load all records</param>
    /// <param name="createdFromUtc">Created date from (UTC); pass null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); pass null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping Cart
    /// </returns>
    public virtual async Task<IList<ShoppingCartItem>> GetShoppingCartAsync(Customer customer, ShoppingCartType? shoppingCartType = null,
        int storeId = 0, int? productId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null)
    {
        ArgumentNullException.ThrowIfNull(customer);

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

        return await _shortTermCacheManager.GetAsync(async () => await items.ToListAsync(), NopOrderDefaults.ShoppingCartItemsAllCacheKey, customer, shoppingCartType, storeId, productId, createdFromUtc, createdToUtc);
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
    /// <param name="ignoreConditionMet">A value indicating whether we should ignore filtering by "is condition met" property</param>
    /// <param name="ignoreBundledProducts">A value indicating whether we should ignore bundled (associated) products</param>
    /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> GetShoppingCartItemAttributeWarningsAsync(Customer customer,
        ShoppingCartType shoppingCartType,
        Product product,
        int quantity = 1,
        string attributesXml = "",
        bool ignoreNonCombinableAttributes = false,
        bool ignoreConditionMet = false,
        bool ignoreBundledProducts = false,
        int shoppingCartItemId = 0)
    {
        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();

        //ensure it's our attributes
        var attributes1 = await _productAttributeParser.ParseProductAttributeMappingsAsync(attributesXml);
        if (ignoreNonCombinableAttributes)
        {
            attributes1 = attributes1.Where(x => !x.IsNonCombinable()).ToList();
        }

        foreach (var attribute in attributes1)
        {
            if (attribute.ProductId == 0)
            {
                warnings.Add("Attribute error");
                return warnings;
            }

            if (attribute.ProductId != product.Id)
            {
                warnings.Add("Attribute error");
            }
        }

        //validate required product attributes (whether they're chosen/selected/entered)
        var attributes2 = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
        if (ignoreNonCombinableAttributes)
        {
            attributes2 = attributes2.Where(x => !x.IsNonCombinable()).ToList();
        }

        //validate conditional attributes only (if specified)
        if (!ignoreConditionMet)
        {
            attributes2 = await attributes2.WhereAwait(async x =>
            {
                var conditionMet = await _productAttributeParser.IsConditionMetAsync(x, attributesXml);
                return !conditionMet.HasValue || conditionMet.Value;
            }).ToListAsync();
        }

        foreach (var a2 in attributes2)
        {
            var productAttributeValues = await _productAttributeService.GetProductAttributeValuesAsync(a2.Id);

            if (a2.IsRequired)
            {
                var found = false;
                //selected product attributes
                foreach (var a1 in attributes1)
                {
                    if (a1.Id != a2.Id)
                        continue;

                    var attributeValuesStr = _productAttributeParser.ParseValues(attributesXml, a1.Id);

                    if (a2.ShouldHaveValues() && productAttributeValues.Any() && !productAttributeValues.Any(x => attributeValuesStr.Contains(x.Id.ToString())))
                        break;

                    foreach (var str1 in attributeValuesStr)
                    {
                        if (string.IsNullOrEmpty(str1.Trim()))
                            continue;

                        found = true;
                        break;
                    }
                }

                //if not found
                if (!found)
                {
                    var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(a2.ProductAttributeId);

                    var textPrompt = await _localizationService.GetLocalizedAsync(a2, x => x.TextPrompt);
                    var notFoundWarning = !string.IsNullOrEmpty(textPrompt) ?
                        textPrompt :
                        string.Format(await _localizationService.GetResourceAsync("ShoppingCart.SelectAttribute"), await _localizationService.GetLocalizedAsync(productAttribute, a => a.Name));

                    warnings.Add(notFoundWarning);
                }
            }

            if (a2.AttributeControlType != AttributeControlType.ReadonlyCheckboxes)
                continue;

            //customers cannot edit read-only attributes
            var allowedReadOnlyValueIds = productAttributeValues
                .Where(x => x.IsPreSelected)
                .Select(x => x.Id)
                .ToArray();

            var selectedReadOnlyValueIds = (await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml))
                .Where(x => x.ProductAttributeMappingId == a2.Id)
                .Select(x => x.Id)
                .ToArray();

            if (!CommonHelper.ArraysEqual(allowedReadOnlyValueIds, selectedReadOnlyValueIds))
            {
                warnings.Add("You cannot change read-only values");
            }
        }

        //validation rules
        foreach (var pam in attributes2)
        {
            if (!pam.ValidationRulesAllowed())
                continue;

            string enteredText;
            int enteredTextLength;

            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);

            //minimum length
            if (pam.ValidationMinLength.HasValue)
            {
                if (pam.AttributeControlType == AttributeControlType.TextBox ||
                    pam.AttributeControlType == AttributeControlType.MultilineTextbox)
                {
                    enteredText = _productAttributeParser.ParseValues(attributesXml, pam.Id).FirstOrDefault();
                    enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                    if (pam.ValidationMinLength.Value > enteredTextLength)
                    {
                        warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.TextboxMinimumLength"), await _localizationService.GetLocalizedAsync(productAttribute, a => a.Name), pam.ValidationMinLength.Value));
                    }
                }
            }

            //maximum length
            if (!pam.ValidationMaxLength.HasValue)
                continue;

            if (pam.AttributeControlType != AttributeControlType.TextBox && pam.AttributeControlType != AttributeControlType.MultilineTextbox)
                continue;

            enteredText = _productAttributeParser.ParseValues(attributesXml, pam.Id).FirstOrDefault();
            enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

            if (pam.ValidationMaxLength.Value < enteredTextLength)
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.TextboxMaximumLength"), await _localizationService.GetLocalizedAsync(productAttribute, a => a.Name), pam.ValidationMaxLength.Value));
            }
        }

        if (warnings.Any() || ignoreBundledProducts)
            return warnings;

        //validate bundled products
        var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
        foreach (var attributeValue in attributeValues)
        {
            if (attributeValue.AttributeValueType != AttributeValueType.AssociatedToProduct)
                continue;

            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(attributeValue.ProductAttributeMappingId);

            if (productAttributeMapping == null)
                continue;

            if (ignoreNonCombinableAttributes && productAttributeMapping.IsNonCombinable())
                continue;

            //associated product (bundle)
            var associatedProduct = await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId);
            
            if (associatedProduct != null)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var totalQty = quantity * attributeValue.Quantity;
                var associatedProductWarnings = await GetShoppingCartItemWarningsAsync(customer,
                    shoppingCartType, associatedProduct, store.Id,
                    string.Empty, decimal.Zero, null, null, totalQty, false, shoppingCartItemId);

                var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId);

                foreach (var associatedProductWarning in associatedProductWarnings)
                {
                    var attributeName = await _localizationService.GetLocalizedAsync(productAttribute, a => a.Name);
                    var attributeValueName = await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name);
                    warnings.Add(string.Format(
                        await _localizationService.GetResourceAsync("ShoppingCart.AssociatedAttributeWarning"),
                        attributeName, attributeValueName, associatedProductWarning));
                }
            }
            else
                warnings.Add($"Associated product cannot be loaded - {attributeValue.AssociatedProductId}");
        }

        return warnings;
    }

    /// <summary>
    /// Validates shopping cart item (gift card)
    /// </summary>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> GetShoppingCartItemGiftCardWarningsAsync(ShoppingCartType shoppingCartType,
        Product product, string attributesXml)
    {
        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();

        //gift cards
        if (!product.IsGiftCard)
            return warnings;

        _productAttributeParser.GetGiftCardAttribute(attributesXml, out var giftCardRecipientName, out var giftCardRecipientEmail, out var giftCardSenderName, out var giftCardSenderEmail, out var _);

        if (string.IsNullOrEmpty(giftCardRecipientName))
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.RecipientNameError"));

        if (product.GiftCardType == GiftCardType.Virtual)
        {
            //validate for virtual gift cards only
            if (string.IsNullOrEmpty(giftCardRecipientEmail) || !CommonHelper.IsValidEmail(giftCardRecipientEmail))
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.RecipientEmailError"));
        }

        if (string.IsNullOrEmpty(giftCardSenderName))
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.SenderNameError"));

        if (product.GiftCardType != GiftCardType.Virtual)
            return warnings;

        //validate for virtual gift cards only
        if (string.IsNullOrEmpty(giftCardSenderEmail) || !CommonHelper.IsValidEmail(giftCardSenderEmail))
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.SenderEmailError"));

        return warnings;
    }

    /// <summary>
    /// Validates shopping cart item for rental products
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="rentalStartDate">Rental start date</param>
    /// <param name="rentalEndDate">Rental end date</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> GetRentalProductWarningsAsync(Product product,
        DateTime? rentalStartDate = null, DateTime? rentalEndDate = null)
    {
        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();

        if (!product.IsRental)
            return warnings;

        if (!rentalStartDate.HasValue)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.Rental.EnterStartDate"));
            return warnings;
        }

        if (!rentalEndDate.HasValue)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.Rental.EnterEndDate"));
            return warnings;
        }

        if (rentalStartDate.Value.CompareTo(rentalEndDate.Value) > 0)
        {
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.Rental.StartDateLessEndDate"));
            return warnings;
        }

        //allowed start date should be the future date
        //we should compare rental start date with a store local time
        //but we what if a store works in distinct timezones? how we should handle it? skip it for now
        //we also ignore hours (anyway not supported yet)
        //today
        var nowDtInStoreTimeZone = _dateTimeHelper.ConvertToUserTime(DateTime.Now, TimeZoneInfo.Local, _dateTimeHelper.DefaultStoreTimeZone);
        var todayDt = new DateTime(nowDtInStoreTimeZone.Year, nowDtInStoreTimeZone.Month, nowDtInStoreTimeZone.Day);
        var todayDtUtc = _dateTimeHelper.ConvertToUtcTime(todayDt, _dateTimeHelper.DefaultStoreTimeZone);
        //dates are entered in store timezone (e.g. like in hotels)
        var startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.DefaultStoreTimeZone);
        //but we what if dates should be entered in a customer timezone?
        //DateTime startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.CurrentTimeZone);
        if (todayDtUtc.CompareTo(startDateUtc) <= 0)
            return warnings;

        warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.Rental.StartDateShouldBeFuture"));
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
    /// <param name="addRequiredProducts">Whether to add required products</param>
    /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
    /// <param name="getStandardWarnings">A value indicating whether we should validate a product for standard properties</param>
    /// <param name="getAttributesWarnings">A value indicating whether we should validate product attributes</param>
    /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
    /// <param name="getRequiredProductWarnings">A value indicating whether we should validate required products (products which require other products to be added to the cart)</param>
    /// <param name="getRentalWarnings">A value indicating whether we should validate rental properties</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> GetShoppingCartItemWarningsAsync(Customer customer, ShoppingCartType shoppingCartType,
        Product product, int storeId,
        string attributesXml, decimal customerEnteredPrice,
        DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
        int quantity = 1, bool addRequiredProducts = true, int shoppingCartItemId = 0,
        bool getStandardWarnings = true, bool getAttributesWarnings = true,
        bool getGiftCardWarnings = true, bool getRequiredProductWarnings = true,
        bool getRentalWarnings = true)
    {
        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();

        //standard properties
        if (getStandardWarnings)
            warnings.AddRange(await GetStandardWarningsAsync(customer, shoppingCartType, product, attributesXml, customerEnteredPrice, quantity, shoppingCartItemId, storeId));

        //selected attributes
        if (getAttributesWarnings)
            warnings.AddRange(await GetShoppingCartItemAttributeWarningsAsync(customer, shoppingCartType, product, quantity, attributesXml, false, false, false, shoppingCartItemId));

        //gift cards
        if (getGiftCardWarnings)
            warnings.AddRange(await GetShoppingCartItemGiftCardWarningsAsync(shoppingCartType, product, attributesXml));

        //required products
        if (getRequiredProductWarnings)
            warnings.AddRange(await GetRequiredProductWarningsAsync(customer, shoppingCartType, product, storeId, quantity, addRequiredProducts, shoppingCartItemId));

        //rental products
        if (getRentalWarnings)
            warnings.AddRange(await GetRentalProductWarningsAsync(product, rentalStartDate, rentalEndDate));

        return warnings;
    }

    /// <summary>
    /// Validates whether this shopping cart is valid
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <param name="checkoutAttributesXml">Checkout attributes in XML format</param>
    /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> GetShoppingCartWarningsAsync(IList<ShoppingCartItem> shoppingCart,
        string checkoutAttributesXml, bool validateCheckoutAttributes)
    {
        var warnings = new List<string>();

        if (shoppingCart.Count > _shoppingCartSettings.MaximumShoppingCartItems)
            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumShoppingCartItems"), _shoppingCartSettings.MaximumShoppingCartItems));

        var hasStandardProducts = false;
        var hasRecurringProducts = false;

        foreach (var sci in shoppingCart)
        {
            var product = await _productService.GetProductByIdAsync(sci.ProductId);
            if (product == null)
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.CannotLoadProduct"), sci.ProductId));
                return warnings;
            }

            if (product.IsRecurring)
                hasRecurringProducts = true;
            else
                hasStandardProducts = true;
        }

        //don't mix standard and recurring products
        if (hasStandardProducts && hasRecurringProducts)
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.CannotMixStandardAndAutoshipProducts"));

        //recurring cart validation
        if (hasRecurringProducts)
        {
            var cyclesError = (await GetRecurringCycleInfoAsync(shoppingCart)).error;
            if (!string.IsNullOrEmpty(cyclesError))
            {
                warnings.Add(cyclesError);
                return warnings;
            }
        }

        //validate checkout attributes
        if (!validateCheckoutAttributes)
            return warnings;

        //selected attributes
        var attributes1 = await _checkoutAttributeParser.ParseAttributesAsync(checkoutAttributesXml);

        //existing checkout attributes
        var excludeShippableAttributes = !await ShoppingCartRequiresShippingAsync(shoppingCart);
        var store = await _storeContext.GetCurrentStoreAsync();
        var attributes2 = await _checkoutAttributeService.GetAllAttributesAsync(_staticCacheManager, _storeMappingService, store.Id, excludeShippableAttributes);

        //validate conditional attributes only (if specified)
        attributes2 = await attributes2.WhereAwait(async x =>
        {
            var conditionMet = await _checkoutAttributeParser.IsConditionMetAsync(x.ConditionAttributeXml, checkoutAttributesXml);
            return !conditionMet.HasValue || conditionMet.Value;
        }).ToListAsync();

        foreach (var a2 in attributes2)
        {
            if (!a2.IsRequired)
                continue;

            var found = false;
            //selected checkout attributes
            foreach (var a1 in attributes1)
            {
                if (a1.Id != a2.Id)
                    continue;

                var attributeValuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, a1.Id);
                foreach (var str1 in attributeValuesStr)
                    if (!string.IsNullOrEmpty(str1.Trim()))
                    {
                        found = true;
                        break;
                    }
            }

            if (found)
                continue;

            //if not found
            warnings.Add(!string.IsNullOrEmpty(await _localizationService.GetLocalizedAsync(a2, a => a.TextPrompt))
                ? await _localizationService.GetLocalizedAsync(a2, a => a.TextPrompt)
                : string.Format(await _localizationService.GetResourceAsync("ShoppingCart.SelectAttribute"),
                    await _localizationService.GetLocalizedAsync(a2, a => a.Name)));
        }

        //now validation rules

        //minimum length
        foreach (var ca in attributes2)
        {
            string enteredText;
            int enteredTextLength;

            if (ca.ValidationMinLength.HasValue)
            {
                if (ca.AttributeControlType == AttributeControlType.TextBox ||
                    ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                {
                    enteredText = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id).FirstOrDefault();
                    enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                    if (ca.ValidationMinLength.Value > enteredTextLength)
                    {
                        warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.TextboxMinimumLength"), await _localizationService.GetLocalizedAsync(ca, a => a.Name), ca.ValidationMinLength.Value));
                    }
                }
            }

            //maximum length
            if (!ca.ValidationMaxLength.HasValue)
                continue;

            if (ca.AttributeControlType != AttributeControlType.TextBox && ca.AttributeControlType != AttributeControlType.MultilineTextbox)
                continue;

            enteredText = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id).FirstOrDefault();
            enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

            if (ca.ValidationMaxLength.Value < enteredTextLength)
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.TextboxMaximumLength"), await _localizationService.GetLocalizedAsync(ca, a => a.Name), ca.ValidationMaxLength.Value));
            }
        }

        return warnings;
    }

    /// <summary>
    /// Gets the shopping cart item sub total
    /// </summary>
    /// <param name="shoppingCartItem">The shopping cart item</param>
    /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
    /// <returns>Shopping cart item sub total. Applied discount amount. Applied discounts. Maximum discounted qty. Return not nullable value if discount cannot be applied to ALL items</returns>
    public virtual async Task<(decimal subTotal, decimal discountAmount, List<Discount> appliedDiscounts, int? maximumDiscountQty)> GetSubTotalAsync(ShoppingCartItem shoppingCartItem,
        bool includeDiscounts)
    {
        ArgumentNullException.ThrowIfNull(shoppingCartItem);

        decimal subTotal;
        int? maximumDiscountQty = null;

        //unit price
        var (unitPrice, discountAmount, appliedDiscounts) = await GetUnitPriceAsync(shoppingCartItem, includeDiscounts);

        //discount
        if (appliedDiscounts.Any())
        {
            //we can properly use "MaximumDiscountedQuantity" property only for one discount (not cumulative ones)
            Discount oneAndOnlyDiscount = null;
            if (appliedDiscounts.Count == 1)
                oneAndOnlyDiscount = appliedDiscounts.First();

            if ((oneAndOnlyDiscount?.MaximumDiscountedQuantity.HasValue ?? false) &&
                shoppingCartItem.Quantity > oneAndOnlyDiscount.MaximumDiscountedQuantity.Value)
            {
                maximumDiscountQty = oneAndOnlyDiscount.MaximumDiscountedQuantity.Value;
                //we cannot apply discount for all shopping cart items
                var discountedQuantity = oneAndOnlyDiscount.MaximumDiscountedQuantity.Value;
                var discountedSubTotal = unitPrice * discountedQuantity;
                discountAmount *= discountedQuantity;

                var notDiscountedQuantity = shoppingCartItem.Quantity - discountedQuantity;
                var notDiscountedUnitPrice = (await GetUnitPriceAsync(shoppingCartItem, false)).unitPrice;
                var notDiscountedSubTotal = notDiscountedUnitPrice * notDiscountedQuantity;

                subTotal = discountedSubTotal + notDiscountedSubTotal;
            }
            else
            {
                //discount is applied to all items (quantity)
                //calculate discount amount for all items
                discountAmount *= shoppingCartItem.Quantity;

                subTotal = unitPrice * shoppingCartItem.Quantity;
            }
        }
        else
        {
            subTotal = unitPrice * shoppingCartItem.Quantity;
        }

        return (subTotal, discountAmount, appliedDiscounts, maximumDiscountQty);
    }

    /// <summary>
    /// Gets the shopping cart unit price (one item)
    /// </summary>
    /// <param name="shoppingCartItem">The shopping cart item</param>
    /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart unit price (one item). Applied discount amount. Applied discounts
    /// </returns>
    public virtual async Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(ShoppingCartItem shoppingCartItem,
        bool includeDiscounts)
    {
        ArgumentNullException.ThrowIfNull(shoppingCartItem);

        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);
        var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);
        var store = await _storeService.GetStoreByIdAsync(shoppingCartItem.StoreId);

        return await GetUnitPriceAsync(product,
            customer,
            store,
            shoppingCartItem.ShoppingCartType,
            shoppingCartItem.Quantity,
            shoppingCartItem.AttributesXml,
            shoppingCartItem.CustomerEnteredPrice,
            shoppingCartItem.RentalStartDateUtc,
            shoppingCartItem.RentalEndDateUtc,
            includeDiscounts);
    }

    /// <summary>
    /// Gets the shopping cart unit price (one item)
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="customer">Customer</param>
    /// <param name="store">Store</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="attributesXml">Product attributes (XML format)</param>
    /// <param name="customerEnteredPrice">Customer entered price (if specified)</param>
    /// <param name="rentalStartDate">Rental start date (null for not rental products)</param>
    /// <param name="rentalEndDate">Rental end date (null for not rental products)</param>
    /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart unit price (one item). Applied discount amount. Applied discounts
    /// </returns>
    public virtual async Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(Product product,
        Customer customer,
        Store store,
        ShoppingCartType shoppingCartType,
        int quantity,
        string attributesXml,
        decimal customerEnteredPrice,
        DateTime? rentalStartDate, DateTime? rentalEndDate,
        bool includeDiscounts)
    {
        ArgumentNullException.ThrowIfNull(product);

        ArgumentNullException.ThrowIfNull(customer);

        var discountAmount = decimal.Zero;
        var appliedDiscounts = new List<Discount>();

        decimal finalPrice;

        var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
        if (combination?.OverriddenPrice.HasValue ?? false)
        {
            (_, finalPrice, discountAmount, appliedDiscounts) = await _priceCalculationService.GetFinalPriceAsync(product,
                customer,
                store,
                combination.OverriddenPrice.Value,
                decimal.Zero,
                includeDiscounts,
                quantity,
                product.IsRental ? rentalStartDate : null,
                product.IsRental ? rentalEndDate : null);
        }
        else
        {
            //summarize price of all attributes
            var attributesTotalPrice = decimal.Zero;
            var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
            if (attributeValues != null)
            {
                foreach (var attributeValue in attributeValues)
                {
                    attributesTotalPrice += await _priceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(product,
                        attributeValue,
                        customer,
                        store,
                        product.CustomerEntersPrice ? (decimal?)customerEnteredPrice : null,
                        quantity);
                }
            }

            //get price of a product (with previously calculated price of all attributes)
            if (product.CustomerEntersPrice)
            {
                finalPrice = customerEnteredPrice;
            }
            else
            {
                int qty;
                if (_shoppingCartSettings.GroupTierPricesForDistinctShoppingCartItems)
                {
                    //the same products with distinct product attributes could be stored as distinct "ShoppingCartItem" records
                    //so let's find how many of the current products are in the cart                        
                    qty = (await GetShoppingCartAsync(customer, shoppingCartType: shoppingCartType, productId: product.Id))
                        .Sum(x => x.Quantity);

                    if (qty == 0)
                    {
                        qty = quantity;
                    }
                }
                else
                {
                    qty = quantity;
                }

                (_, finalPrice, discountAmount, appliedDiscounts) = await _priceCalculationService.GetFinalPriceAsync(product,
                    customer,
                    store,
                    attributesTotalPrice,
                    includeDiscounts,
                    qty,
                    product.IsRental ? rentalStartDate : null,
                    product.IsRental ? rentalEndDate : null);
            }
        }

        //rounding
        if (_shoppingCartSettings.RoundPricesDuringCalculation)
            finalPrice = await _priceCalculationService.RoundPriceAsync(finalPrice);

        return (finalPrice, discountAmount, appliedDiscounts);
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found shopping cart item
    /// </returns>
    public virtual async Task<ShoppingCartItem> FindShoppingCartItemInTheCartAsync(IList<ShoppingCartItem> shoppingCart,
        ShoppingCartType shoppingCartType,
        Product product,
        string attributesXml = "",
        decimal customerEnteredPrice = decimal.Zero,
        DateTime? rentalStartDate = null,
        DateTime? rentalEndDate = null)
    {
        ArgumentNullException.ThrowIfNull(shoppingCart);

        ArgumentNullException.ThrowIfNull(product);

        return await shoppingCart.Where(sci => sci.ShoppingCartType == shoppingCartType)
            .FirstOrDefaultAwaitAsync(async sci => await ShoppingCartItemIsEqualAsync(sci, product, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate));
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
    /// <param name="addRequiredProducts">Whether to add required products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> AddToCartAsync(Customer customer, Product product,
        ShoppingCartType shoppingCartType, int storeId, string attributesXml = null,
        decimal customerEnteredPrice = decimal.Zero,
        DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
        int quantity = 1, bool addRequiredProducts = true)
    {
        ArgumentNullException.ThrowIfNull(customer);

        ArgumentNullException.ThrowIfNull(product);

        var warnings = new List<string>();
        if (shoppingCartType == ShoppingCartType.ShoppingCart && !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART, customer))
        {
            warnings.Add("Shopping cart is disabled");
            return warnings;
        }

        if (shoppingCartType == ShoppingCartType.Wishlist && !await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_WISHLIST, customer))
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
            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.QuantityShouldPositive"));
            return warnings;
        }

        //reset checkout info
        await _customerService.ResetCheckoutDataAsync(customer, storeId);

        var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);

        var shoppingCartItem = await FindShoppingCartItemInTheCartAsync(cart,
            shoppingCartType, product, attributesXml, customerEnteredPrice,
            rentalStartDate, rentalEndDate);

        if (shoppingCartItem != null)
        {
            //update existing shopping cart item
            var newQuantity = shoppingCartItem.Quantity + quantity;

            await addRequiredProductsToCartAsync(newQuantity);

            if (warnings.Any())
                return warnings;

            warnings.AddRange(await GetShoppingCartItemWarningsAsync(customer, shoppingCartType, product,
                storeId, attributesXml,
                customerEnteredPrice, rentalStartDate, rentalEndDate,
                newQuantity, addRequiredProducts, shoppingCartItem.Id));

            if (warnings.Any())
                return warnings;

            shoppingCartItem.AttributesXml = attributesXml;
            shoppingCartItem.Quantity = newQuantity;
            shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

            await _sciRepository.UpdateAsync(shoppingCartItem);
        }
        else
        {
            //new shopping cart item
            warnings.AddRange(await GetShoppingCartItemWarningsAsync(customer, shoppingCartType, product,
                storeId, attributesXml, customerEnteredPrice,
                rentalStartDate, rentalEndDate,
                quantity, addRequiredProducts));

            if (warnings.Any())
                return warnings;

            await addRequiredProductsToCartAsync();

            if (warnings.Any())
                return warnings;

            //maximum items validation
            switch (shoppingCartType)
            {
                case ShoppingCartType.ShoppingCart:
                    if (cart.Count >= _shoppingCartSettings.MaximumShoppingCartItems)
                    {
                        warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumShoppingCartItems"), _shoppingCartSettings.MaximumShoppingCartItems));
                        return warnings;
                    }

                    break;
                case ShoppingCartType.Wishlist:
                    if (cart.Count >= _shoppingCartSettings.MaximumWishlistItems)
                    {
                        warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumWishlistItems"), _shoppingCartSettings.MaximumWishlistItems));
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

            await _sciRepository.InsertAsync(shoppingCartItem);

            //updated "HasShoppingCartItems" property used for performance optimization
            var hasShoppingCartItems = !IsCustomerShoppingCartEmpty(customer);
            if (hasShoppingCartItems != customer.HasShoppingCartItems)
            {
                customer.HasShoppingCartItems = hasShoppingCartItems;
                await _customerService.UpdateCustomerAsync(customer);
            }
        }

        return warnings;

        async Task addRequiredProductsToCartAsync(int qty = 0)
        {
            if (!product.RequireOtherProducts)
                return;

            //get these required products
            var requiredProducts = await _productService.GetProductsByIdsAsync(_productService.ParseRequiredProductIds(product));
            if (!requiredProducts.Any())
                return;

            var finalRequiredProducts = requiredProducts.GroupBy(p => p.Id)
                .Select(g => new { Product = g.First(), Count = g.Count() });

            foreach (var requiredProduct in finalRequiredProducts)
            {
                var productsRequiringRequiredProduct = await GetProductsRequiringProductAsync(cart, requiredProduct.Product);

                //get the required quantity of the required product
                var requiredProductRequiredQuantity = (qty > 0 ? qty : quantity) +
                                                      cart.Where(ci => productsRequiringRequiredProduct.Any(p => p.Id == ci.ProductId))
                                                          .Where(item => item.Id != (shoppingCartItem?.Id ?? 0))
                                                          .Sum(item => item.Quantity);

                //whether required product is already in the cart in the required quantity
                var quantityToAdd = requiredProductRequiredQuantity * requiredProduct.Count - (cart.FirstOrDefault(item => item.ProductId == requiredProduct.Product.Id)?.Quantity ?? 0);
                if (quantityToAdd <= 0)
                    continue;

                if (addRequiredProducts && product.AutomaticallyAddRequiredProducts)
                {
                    //do not add required products to prevent circular references
                    var addToCartWarnings = await AddToCartAsync(customer, requiredProduct.Product, shoppingCartType, storeId,
                        quantity: quantityToAdd, addRequiredProducts: requiredProduct.Product.AutomaticallyAddRequiredProducts);

                    if (addToCartWarnings.Any())
                    {
                        warnings.AddRange(addToCartWarnings);
                        return;
                    }
                }
            }
        }
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> UpdateShoppingCartItemAsync(Customer customer,
        int shoppingCartItemId, string attributesXml,
        decimal customerEnteredPrice,
        DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
        int quantity = 1, bool resetCheckoutData = true)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var warnings = new List<string>();

        var shoppingCartItem = await _sciRepository.GetByIdAsync(shoppingCartItemId, cache => default);

        if (shoppingCartItem == null || shoppingCartItem.CustomerId != customer.Id)
            return warnings;

        if (resetCheckoutData)
        {
            //reset checkout data
            await _customerService.ResetCheckoutDataAsync(customer, shoppingCartItem.StoreId);
        }

        var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);

        if (quantity > 0)
        {
            //check warnings
            warnings.AddRange(await GetShoppingCartItemWarningsAsync(customer, shoppingCartItem.ShoppingCartType,
                product, shoppingCartItem.StoreId,
                attributesXml, customerEnteredPrice,
                rentalStartDate, rentalEndDate, quantity, false, shoppingCartItemId));
            if (warnings.Any())
                return warnings;

            //if everything is OK, then update a shopping cart item
            shoppingCartItem.Quantity = quantity;
            shoppingCartItem.AttributesXml = attributesXml;
            shoppingCartItem.CustomerEnteredPrice = customerEnteredPrice;
            shoppingCartItem.RentalStartDateUtc = rentalStartDate;
            shoppingCartItem.RentalEndDateUtc = rentalEndDate;
            shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

            await _sciRepository.UpdateAsync(shoppingCartItem);
        }
        else
        {
            //check warnings for required products
            warnings.AddRange(await GetRequiredProductWarningsAsync(customer, shoppingCartItem.ShoppingCartType,
                product, shoppingCartItem.StoreId, quantity, false, shoppingCartItemId));
            if (warnings.Any())
                return warnings;

            //delete a shopping cart item
            await DeleteShoppingCartItemAsync(shoppingCartItem, resetCheckoutData, true);
        }

        return warnings;
    }

    /// <summary>
    /// Migrate shopping cart
    /// </summary>
    /// <param name="fromCustomer">From customer</param>
    /// <param name="toCustomer">To customer</param>
    /// <param name="includeCouponCodes">A value indicating whether to coupon codes (discount and gift card) should be also re-applied</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task MigrateShoppingCartAsync(Customer fromCustomer, Customer toCustomer, bool includeCouponCodes)
    {
        ArgumentNullException.ThrowIfNull(fromCustomer);
        ArgumentNullException.ThrowIfNull(toCustomer);

        if (fromCustomer.Id == toCustomer.Id)
            return; //the same customer

        //shopping cart items
        var fromCart = await GetShoppingCartAsync(fromCustomer);

        for (var i = 0; i < fromCart.Count; i++)
        {
            var sci = fromCart[i];
            var product = await _productService.GetProductByIdAsync(sci.ProductId);

            await AddToCartAsync(toCustomer, product, sci.ShoppingCartType, sci.StoreId,
                sci.AttributesXml, sci.CustomerEnteredPrice,
                sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false);
        }

        for (var i = 0; i < fromCart.Count; i++)
        {
            var sci = fromCart[i];
            await DeleteShoppingCartItemAsync(sci);
        }

        //copy discount and gift card coupon codes
        if (includeCouponCodes)
        {
            //discount
            foreach (var code in await _customerService.ParseAppliedDiscountCouponCodesAsync(fromCustomer))
                await _customerService.ApplyDiscountCouponCodeAsync(toCustomer, code);

            //gift card
            foreach (var code in await _customerService.ParseAppliedGiftCardCouponCodesAsync(fromCustomer))
                await _customerService.ApplyGiftCardCouponCodeAsync(toCustomer, code);
        }

        //move selected checkout attributes
        var store = await _storeContext.GetCurrentStoreAsync();
        var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(fromCustomer, NopCustomerDefaults.CheckoutAttributes, store.Id);
        await _genericAttributeService.SaveAttributeAsync(toCustomer, NopCustomerDefaults.CheckoutAttributes, checkoutAttributesXml, store.Id);
    }

    /// <summary>
    /// Indicates whether the shopping cart requires shipping
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if the shopping cart requires shipping; otherwise, false.
    /// </returns>
    public virtual async Task<bool> ShoppingCartRequiresShippingAsync(IList<ShoppingCartItem> shoppingCart)
    {
        return await shoppingCart.AnyAwaitAsync(async shoppingCartItem => await _shippingService.IsShipEnabledAsync(shoppingCartItem));
    }

    /// <summary>
    /// Gets a value indicating whether shopping cart is recurring
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> ShoppingCartIsRecurringAsync(IList<ShoppingCartItem> shoppingCart)
    {
        ArgumentNullException.ThrowIfNull(shoppingCart);

        if (!shoppingCart.Any())
            return false;

        return await _productService.HasAnyRecurringProductAsync(shoppingCart.Select(sci => sci.ProductId).ToArray());
    }

    /// <summary>
    /// Get a recurring cycle information
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the error (if exists); otherwise, empty string. Cycle length. Cycle period. Total cycles
    /// </returns>
    public virtual async Task<(string error, int cycleLength, RecurringProductCyclePeriod cyclePeriod, int totalCycles)> GetRecurringCycleInfoAsync(IList<ShoppingCartItem> shoppingCart)
    {
        var rezCycleLength = 0;
        RecurringProductCyclePeriod rezCyclePeriod = 0;
        var rezTotalCycles = 0;

        int? cycleLength = null;
        RecurringProductCyclePeriod? cyclePeriod = null;
        int? totalCycles = null;

        var conflictError = await _localizationService.GetResourceAsync("ShoppingCart.ConflictingShipmentSchedules");

        foreach (var sci in shoppingCart)
        {
            var product = await _productService.GetProductByIdAsync(sci.ProductId) ?? throw new NopException($"Product (Id={sci.ProductId}) cannot be loaded");

            if (!product.IsRecurring)
                continue;

            //cycle length
            if (cycleLength.HasValue && cycleLength.Value != product.RecurringCycleLength)
                return (conflictError, rezCycleLength, rezCyclePeriod, rezTotalCycles);
            cycleLength = product.RecurringCycleLength;

            //cycle period
            if (cyclePeriod.HasValue && cyclePeriod.Value != product.RecurringCyclePeriod)
                return (conflictError, rezCycleLength, rezCyclePeriod, rezTotalCycles);
            cyclePeriod = product.RecurringCyclePeriod;

            //total cycles
            if (totalCycles.HasValue && totalCycles.Value != product.RecurringTotalCycles)
                return (conflictError, rezCycleLength, rezCyclePeriod, rezTotalCycles);
            totalCycles = product.RecurringTotalCycles;
        }

        if (!cycleLength.HasValue)
            return (string.Empty, rezCycleLength, rezCyclePeriod, rezTotalCycles);

        rezCycleLength = cycleLength.Value;
        rezCyclePeriod = cyclePeriod.Value;
        rezTotalCycles = totalCycles.Value;

        return (string.Empty, rezCycleLength, rezCyclePeriod, rezTotalCycles);
    }

    #endregion
}