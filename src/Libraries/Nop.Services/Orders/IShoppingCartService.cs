using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;

namespace Nop.Services.Orders;

/// <summary>
/// Shopping cart service
/// </summary>
public partial interface IShoppingCartService
{
    /// <summary>
    /// Delete shopping cart item
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
    /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteShoppingCartItemAsync(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true,
        bool ensureOnlyActiveCheckoutAttributes = false);

    /// <summary>
    /// Clear shopping cart
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Store ID</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task ClearShoppingCartAsync(Customer customer, int storeId);

    /// <summary>
    /// Delete shopping cart item
    /// </summary>
    /// <param name="shoppingCartItemId">Shopping cart item ID</param>
    /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
    /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteShoppingCartItemAsync(int shoppingCartItemId, bool resetCheckoutData = true,
        bool ensureOnlyActiveCheckoutAttributes = false);

    /// <summary>
    /// Deletes expired shopping cart items
    /// </summary>
    /// <param name="olderThanUtc">Older than date and time</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of deleted items
    /// </returns>
    Task<int> DeleteExpiredShoppingCartItemsAsync(DateTime olderThanUtc);

    /// <summary>
    /// Get products from shopping cart whether requiring specific product
    /// </summary>
    /// <param name="cart">Shopping cart </param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task<IList<Product>> GetProductsRequiringProductAsync(IList<ShoppingCartItem> cart, Product product);

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
    Task<IList<ShoppingCartItem>> GetShoppingCartAsync(Customer customer, ShoppingCartType? shoppingCartType = null,
        int storeId = 0, int? productId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null);

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
    Task<IList<string>> GetShoppingCartItemAttributeWarningsAsync(Customer customer,
        ShoppingCartType shoppingCartType,
        Product product,
        int quantity = 1,
        string attributesXml = "",
        bool ignoreNonCombinableAttributes = false,
        bool ignoreConditionMet = false,
        bool ignoreBundledProducts = false,
        int shoppingCartItemId = 0);

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
    Task<IList<string>> GetShoppingCartItemGiftCardWarningsAsync(ShoppingCartType shoppingCartType,
        Product product, string attributesXml);

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
    Task<IList<string>> GetRentalProductWarningsAsync(Product product,
        DateTime? rentalStartDate = null, DateTime? rentalEndDate = null);

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
    Task<IList<string>> GetShoppingCartItemWarningsAsync(Customer customer, ShoppingCartType shoppingCartType,
        Product product, int storeId,
        string attributesXml, decimal customerEnteredPrice,
        DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
        int quantity = 1, bool addRequiredProducts = true, int shoppingCartItemId = 0,
        bool getStandardWarnings = true, bool getAttributesWarnings = true,
        bool getGiftCardWarnings = true, bool getRequiredProductWarnings = true,
        bool getRentalWarnings = true);

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
    Task<IList<string>> GetShoppingCartWarningsAsync(IList<ShoppingCartItem> shoppingCart,
        string checkoutAttributesXml, bool validateCheckoutAttributes);

    /// <summary>
    /// Gets the shopping cart unit price (one item)
    /// </summary>
    /// <param name="shoppingCartItem">The shopping cart item</param>
    /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart unit price (one item). Applied discount amount. Applied discounts
    /// </returns>
    Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(ShoppingCartItem shoppingCartItem,
        bool includeDiscounts);

    /// <summary>
    /// Gets the shopping cart unit price (one item)
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="customer">Customer</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="attributesXml">Product attributes (XML format)</param>
    /// <param name="customerEnteredPrice">Customer entered price (if specified)</param>
    /// <param name="rentalStartDate">Rental start date (null for not rental products)</param>
    /// <param name="rentalEndDate">Rental end date (null for not rental products)</param>
    /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart unit price (one item)
    /// </returns>
    Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(Product product,
        Customer customer,
        Store store,
        ShoppingCartType shoppingCartType,
        int quantity,
        string attributesXml,
        decimal customerEnteredPrice,
        DateTime? rentalStartDate, DateTime? rentalEndDate,
        bool includeDiscounts);

    /// <summary>
    /// Gets the shopping cart item sub total
    /// </summary>
    /// <param name="shoppingCartItem">The shopping cart item</param>
    /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart item sub total. Applied discount amount.Applied discounts. Maximum discounted qty. Return not nullable value if discount cannot be applied to ALL items
    /// </returns>
    Task<(decimal subTotal, decimal discountAmount, List<Discount> appliedDiscounts, int? maximumDiscountQty)> GetSubTotalAsync(ShoppingCartItem shoppingCartItem,
        bool includeDiscounts);

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
    Task<ShoppingCartItem> FindShoppingCartItemInTheCartAsync(IList<ShoppingCartItem> shoppingCart,
        ShoppingCartType shoppingCartType,
        Product product,
        string attributesXml = "",
        decimal customerEnteredPrice = decimal.Zero,
        DateTime? rentalStartDate = null,
        DateTime? rentalEndDate = null);

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
    Task<IList<string>> AddToCartAsync(Customer customer, Product product,
        ShoppingCartType shoppingCartType, int storeId, string attributesXml = null,
        decimal customerEnteredPrice = decimal.Zero,
        DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
        int quantity = 1, bool addRequiredProducts = true);

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
    Task<IList<string>> UpdateShoppingCartItemAsync(Customer customer,
        int shoppingCartItemId, string attributesXml,
        decimal customerEnteredPrice,
        DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
        int quantity = 1, bool resetCheckoutData = true);

    /// <summary>
    /// Migrate shopping cart
    /// </summary>
    /// <param name="fromCustomer">From customer</param>
    /// <param name="toCustomer">To customer</param>
    /// <param name="includeCouponCodes">A value indicating whether to coupon codes (discount and gift card) should be also re-applied</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task MigrateShoppingCartAsync(Customer fromCustomer, Customer toCustomer, bool includeCouponCodes);

    /// <summary>
    /// Indicates whether the shopping cart requires shipping
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if the shopping cart requires shipping; otherwise, false.
    /// </returns>
    Task<bool> ShoppingCartRequiresShippingAsync(IList<ShoppingCartItem> shoppingCart);

    /// <summary>
    /// Gets a value indicating whether shopping cart is recurring
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task<bool> ShoppingCartIsRecurringAsync(IList<ShoppingCartItem> shoppingCart);

    /// <summary>
    /// Get a recurring cycle information
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the error (if exists); otherwise, empty string
    /// </returns>
    Task<(string error, int cycleLength, RecurringProductCyclePeriod cyclePeriod, int totalCycles)> GetRecurringCycleInfoAsync(IList<ShoppingCartItem> shoppingCart);
}