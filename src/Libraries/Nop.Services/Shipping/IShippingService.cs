using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Shipping.Pickup;

namespace Nop.Services.Shipping;

/// <summary>
/// Shipping service interface
/// </summary>
public partial interface IShippingService
{
    /// <summary>
    /// Gets shopping cart item weight (of one item)
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart item weight
    /// </returns>
    Task<decimal> GetShoppingCartItemWeightAsync(ShoppingCartItem shoppingCartItem, bool ignoreFreeShippedItems = false);

    /// <summary>
    /// Gets product item weight (of one item)
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Selected product attributes in XML</param>
    /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the item weight
    /// </returns>
    Task<decimal> GetShoppingCartItemWeightAsync(Product product, string attributesXml, bool ignoreFreeShippedItems = false);

    /// <summary>
    /// Gets shopping cart weight
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="includeCheckoutAttributes">A value indicating whether we should calculate weights of selected checkout attributes</param>
    /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the otal weight
    /// </returns>
    Task<decimal> GetTotalWeightAsync(GetShippingOptionRequest request, bool includeCheckoutAttributes = true, bool ignoreFreeShippedItems = false);

    /// <summary>
    /// Get total dimensions
    /// </summary>
    /// <param name="packageItems">Package items</param>
    /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the width. Length. Height
    /// </returns>
    Task<(decimal width, decimal length, decimal height)> GetDimensionsAsync(IList<GetShippingOptionRequest.PackageItem> packageItems, bool ignoreFreeShippedItems = false);

    /// <summary>
    /// Create shipment packages (requests) from shopping cart
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <param name="shippingAddress">Shipping address</param>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment packages (requests). Value indicating whether shipping is done from multiple locations (warehouses)
    /// </returns>
    Task<(IList<GetShippingOptionRequest> shipmentPackages, bool shippingFromMultipleLocations)> CreateShippingOptionRequestsAsync(IList<ShoppingCartItem> cart,
        Address shippingAddress, int storeId);

    /// <summary>
    ///  Gets available shipping options
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <param name="shippingAddress">Shipping address</param>
    /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
    /// <param name="allowedShippingRateComputationMethodSystemName">Filter by shipping rate computation method identifier; null to load shipping options of all shipping rate computation methods</param>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping options
    /// </returns>
    Task<GetShippingOptionResponse> GetShippingOptionsAsync(IList<ShoppingCartItem> cart, Address shippingAddress,
        Customer customer = null, string allowedShippingRateComputationMethodSystemName = "", int storeId = 0);

    /// <summary>
    /// Gets available pickup points
    /// </summary>
    /// <param name="cart">Shopping Cart</param>
    /// <param name="address">Address</param>
    /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
    /// <param name="providerSystemName">Filter by provider identifier; null to load pickup points of all providers</param>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pickup points
    /// </returns>
    Task<GetPickupPointsResponse> GetPickupPointsAsync(IList<ShoppingCartItem> cart, Address address,
        Customer customer = null, string providerSystemName = null, int storeId = 0);

    /// <summary>
    /// Whether the shopping cart item is ship enabled
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if the shopping cart item requires shipping; otherwise false
    /// </returns>
    Task<bool> IsShipEnabledAsync(ShoppingCartItem shoppingCartItem);

    /// <summary>
    /// Whether the shopping cart item is free shipping
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if the shopping cart item is free shipping; otherwise false
    /// </returns>
    Task<bool> IsFreeShippingAsync(ShoppingCartItem shoppingCartItem);

    /// <summary>
    /// Get the additional shipping charge
    /// </summary> 
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the additional shipping charge of the shopping cart item
    /// </returns>
    Task<decimal> GetAdditionalShippingChargeAsync(ShoppingCartItem shoppingCartItem);
}