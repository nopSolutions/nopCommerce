using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping.Pickup;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipping service interface
    /// </summary>
    public partial interface IShippingService
    {
        #region Shipping methods

        /// <summary>
        /// Deletes a shipping method
        /// </summary>
        /// <param name="shippingMethod">The shipping method</param>
        Task DeleteShippingMethodAsync(ShippingMethod shippingMethod);

        /// <summary>
        /// Gets a shipping method
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <returns>Shipping method</returns>
        Task<ShippingMethod> GetShippingMethodByIdAsync(int shippingMethodId);

        /// <summary>
        /// Gets all shipping methods
        /// </summary>
        /// <param name="filterByCountryId">The country identifier to filter by</param>
        /// <returns>Shipping methods</returns>
        Task<IList<ShippingMethod>> GetAllShippingMethodsAsync(int? filterByCountryId = null);

        /// <summary>
        /// Inserts a shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        Task InsertShippingMethodAsync(ShippingMethod shippingMethod);

        /// <summary>
        /// Updates the shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        Task UpdateShippingMethodAsync(ShippingMethod shippingMethod);

        /// <summary>
        /// Does country restriction exist
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Result</returns>
        Task<bool> CountryRestrictionExistsAsync(ShippingMethod shippingMethod, int countryId);

        /// <summary>
        /// Gets shipping country mappings
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Shipping country mappings</returns>
        Task<IList<ShippingMethodCountryMapping>> GetShippingMethodCountryMappingAsync(int shippingMethodId, int countryId);

        /// <summary>
        /// Inserts a shipping country mapping
        /// </summary>
        /// <param name="shippingMethodCountryMapping">Shipping country mapping</param>
        Task InsertShippingMethodCountryMappingAsync(ShippingMethodCountryMapping shippingMethodCountryMapping);

        /// <summary>
        /// Delete the shipping country mapping
        /// </summary>
        /// <param name="shippingMethodCountryMapping">Shipping country mapping</param>
        Task DeleteShippingMethodCountryMappingAsync(ShippingMethodCountryMapping shippingMethodCountryMapping);

        #endregion

        #region Warehouses

        /// <summary>
        /// Deletes a warehouse
        /// </summary>
        /// <param name="warehouse">The warehouse</param>
        Task DeleteWarehouseAsync(Warehouse warehouse);

        /// <summary>
        /// Gets a warehouse
        /// </summary>
        /// <param name="warehouseId">The warehouse identifier</param>
        /// <returns>Warehouse</returns>
        Task<Warehouse> GetWarehouseByIdAsync(int warehouseId);

        /// <summary>
        /// Gets all warehouses
        /// </summary>
        /// <param name="name">Warehouse name</param>
        /// <returns>Warehouses</returns>
        Task<IList<Warehouse>> GetAllWarehousesAsync(string name = null);

        /// <summary>
        /// Inserts a warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        Task InsertWarehouseAsync(Warehouse warehouse);

        /// <summary>
        /// Updates the warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        Task UpdateWarehouseAsync(Warehouse warehouse);

        #endregion

        #region Workflow

        /// <summary>
        /// Gets shopping cart item weight (of one item)
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        /// <returns>Shopping cart item weight</returns>
        Task<decimal> GetShoppingCartItemWeightAsync(ShoppingCartItem shoppingCartItem, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Gets product item weight (of one item)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Selected product attributes in XML</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        /// <returns>Item weight</returns>
        Task<decimal> GetShoppingCartItemWeightAsync(Product product, string attributesXml, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Gets shopping cart weight
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="includeCheckoutAttributes">A value indicating whether we should calculate weights of selected checkout attributes</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        /// <returns>Total weight</returns>
        Task<decimal> GetTotalWeightAsync(GetShippingOptionRequest request, bool includeCheckoutAttributes = true, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Get total dimensions
        /// </summary>
        /// <param name="packageItems">Package items</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        /// <returns>Width. Length. Height</returns>
        Task<(decimal width, decimal length, decimal height)> GetDimensionsAsync(IList<GetShippingOptionRequest.PackageItem> packageItems, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Create shipment packages (requests) from shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Shipment packages (requests). Value indicating whether shipping is done from multiple locations (warehouses)</returns>
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
        /// <returns>Shipping options</returns>
        Task<GetShippingOptionResponse> GetShippingOptionsAsync(IList<ShoppingCartItem> cart, Address shippingAddress,
            Customer customer = null, string allowedShippingRateComputationMethodSystemName = "", int storeId = 0);

        /// <summary>
        /// Gets available pickup points
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="providerSystemName">Filter by provider identifier; null to load pickup points of all providers</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Pickup points</returns>
        Task<GetPickupPointsResponse> GetPickupPointsAsync(int addressId, Customer customer = null, string providerSystemName = null, int storeId = 0);

        /// <summary>
        /// Whether the shopping cart item is ship enabled
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>True if the shopping cart item requires shipping; otherwise false</returns>
        Task<bool> IsShipEnabledAsync(ShoppingCartItem shoppingCartItem);

        /// <summary>
        /// Whether the shopping cart item is free shipping
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>True if the shopping cart item is free shipping; otherwise false</returns>
        Task<bool> IsFreeShippingAsync(ShoppingCartItem shoppingCartItem);

        /// <summary>
        /// Get the additional shipping charge
        /// </summary> 
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>The additional shipping charge of the shopping cart item</returns>
        Task<decimal> GetAdditionalShippingChargeAsync(ShoppingCartItem shoppingCartItem);

        #endregion
    }
}