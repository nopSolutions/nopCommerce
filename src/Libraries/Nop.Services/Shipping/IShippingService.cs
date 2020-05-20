using System.Collections.Generic;
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
        void DeleteShippingMethod(ShippingMethod shippingMethod);

        /// <summary>
        /// Gets a shipping method
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <returns>Shipping method</returns>
        ShippingMethod GetShippingMethodById(int shippingMethodId);

        /// <summary>
        /// Gets all shipping methods
        /// </summary>
        /// <param name="filterByCountryId">The country identifier to filter by</param>
        /// <returns>Shipping methods</returns>
        IList<ShippingMethod> GetAllShippingMethods(int? filterByCountryId = null);

        /// <summary>
        /// Inserts a shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        void InsertShippingMethod(ShippingMethod shippingMethod);

        /// <summary>
        /// Updates the shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        void UpdateShippingMethod(ShippingMethod shippingMethod);

        /// <summary>
        /// Does country restriction exist
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Result</returns>
        bool CountryRestrictionExists(ShippingMethod shippingMethod, int countryId);

        /// <summary>
        /// Gets shipping country mappings
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Shipping country mappings</returns>
        IList<ShippingMethodCountryMapping> GetShippingMethodCountryMapping(int shippingMethodId, int countryId);

        /// <summary>
        /// Inserts a shipping country mapping
        /// </summary>
        /// <param name="shippingMethodCountryMapping">Shipping country mapping</param>
        void InsertShippingMethodCountryMapping(ShippingMethodCountryMapping shippingMethodCountryMapping);

        /// <summary>
        /// Delete the shipping country mapping
        /// </summary>
        /// <param name="shippingMethodCountryMapping">Shipping country mapping</param>
        void DeleteShippingMethodCountryMapping(ShippingMethodCountryMapping shippingMethodCountryMapping);

        #endregion

        #region Warehouses

        /// <summary>
        /// Deletes a warehouse
        /// </summary>
        /// <param name="warehouse">The warehouse</param>
        void DeleteWarehouse(Warehouse warehouse);

        /// <summary>
        /// Gets a warehouse
        /// </summary>
        /// <param name="warehouseId">The warehouse identifier</param>
        /// <returns>Warehouse</returns>
        Warehouse GetWarehouseById(int warehouseId);

        /// <summary>
        /// Gets all warehouses
        /// </summary>
        /// <param name="name">Warehouse name</param>
        /// <returns>Warehouses</returns>
        IList<Warehouse> GetAllWarehouses(string name = null);

        /// <summary>
        /// Inserts a warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        void InsertWarehouse(Warehouse warehouse);

        /// <summary>
        /// Updates the warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        void UpdateWarehouse(Warehouse warehouse);

        #endregion
        
        #region Workflow

        /// <summary>
        /// Gets shopping cart item weight (of one item)
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        /// <returns>Shopping cart item weight</returns>
        decimal GetShoppingCartItemWeight(ShoppingCartItem shoppingCartItem, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Gets product item weight (of one item)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Selected product attributes in XML</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        /// <returns>Item weight</returns>
        decimal GetShoppingCartItemWeight(Product product, string attributesXml, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Gets shopping cart weight
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="includeCheckoutAttributes">A value indicating whether we should calculate weights of selected checkout attributes</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        /// <returns>Total weight</returns>
        decimal GetTotalWeight(GetShippingOptionRequest request, bool includeCheckoutAttributes = true, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Get dimensions of associated products (for quantity 1)
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="width">Width</param>
        /// <param name="length">Length</param>
        /// <param name="height">Height</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        void GetAssociatedProductDimensions(ShoppingCartItem shoppingCartItem,
            out decimal width, out decimal length, out decimal height, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Get total dimensions
        /// </summary>
        /// <param name="packageItems">Package items</param>
        /// <param name="width">Width</param>
        /// <param name="length">Length</param>
        /// <param name="height">Height</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the products marked as "Free shipping"</param>
        void GetDimensions(IList<GetShippingOptionRequest.PackageItem> packageItems,
            out decimal width, out decimal length, out decimal height, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Get the nearest warehouse for the specified address
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="warehouses">List of warehouses, if null all warehouses are used.</param>
        /// <returns></returns>
        Warehouse GetNearestWarehouse(Address address, IList<Warehouse> warehouses = null);

        /// <summary>
        /// Create shipment packages (requests) from shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Shipment packages (requests)</returns>
        /// <param name="shippingFromMultipleLocations">Value indicating whether shipping is done from multiple locations (warehouses)</param>
        IList<GetShippingOptionRequest> CreateShippingOptionRequests(IList<ShoppingCartItem> cart,
            Address shippingAddress, int storeId, out bool shippingFromMultipleLocations);

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="allowedShippingRateComputationMethodSystemName">Filter by shipping rate computation method identifier; null to load shipping options of all shipping rate computation methods</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Shipping options</returns>
        GetShippingOptionResponse GetShippingOptions(IList<ShoppingCartItem> cart, Address shippingAddress,
            Customer customer = null, string allowedShippingRateComputationMethodSystemName = "", int storeId = 0);

        /// <summary>
        /// Gets available pickup points
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="providerSystemName">Filter by provider identifier; null to load pickup points of all providers</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Pickup points</returns>
        GetPickupPointsResponse GetPickupPoints(int addressId, Customer customer = null, string providerSystemName = null, int storeId = 0);

        /// <summary>
        /// Whether the shopping cart item is ship enabled
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>True if the shopping cart item requires shipping; otherwise false</returns>
        bool IsShipEnabled(ShoppingCartItem shoppingCartItem);

        /// <summary>
        /// Whether the shopping cart item is free shipping
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>True if the shopping cart item is free shipping; otherwise false</returns>
        bool IsFreeShipping(ShoppingCartItem shoppingCartItem);

        /// <summary>
        /// Get the additional shipping charge
        /// </summary> 
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>The additional shipping charge of the shopping cart item</returns>
        decimal GetAdditionalShippingCharge(ShoppingCartItem shoppingCartItem);

        #endregion
    }
}