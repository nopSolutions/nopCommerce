using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Shipping.Tracking;

namespace Nop.Services.Shipping;

/// <summary>
/// Shipment service
/// </summary>
public partial class ShipmentService : IShipmentService
{
    #region Fields

    protected readonly IPickupPluginManager _pickupPluginManager;
    protected readonly IRepository<Address> _addressRepository;
    protected readonly IRepository<Order> _orderRepository;
    protected readonly IRepository<OrderItem> _orderItemRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<Shipment> _shipmentRepository;
    protected readonly IRepository<ShipmentItem> _siRepository;
    protected readonly IShippingPluginManager _shippingPluginManager;

    #endregion

    #region Ctor

    public ShipmentService(IPickupPluginManager pickupPluginManager,
        IRepository<Address> addressRepository,
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository,
        IRepository<Product> productRepository,
        IRepository<Shipment> shipmentRepository,
        IRepository<ShipmentItem> siRepository,
        IShippingPluginManager shippingPluginManager)
    {
        _pickupPluginManager = pickupPluginManager;
        _addressRepository = addressRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
        _shipmentRepository = shipmentRepository;
        _siRepository = siRepository;
        _shippingPluginManager = shippingPluginManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a shipment
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteShipmentAsync(Shipment shipment)
    {
        await _shipmentRepository.DeleteAsync(shipment);
    }

    /// <summary>
    /// Search shipments
    /// </summary>
    /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
    /// <param name="warehouseId">Warehouse identifier, only shipments with products from a specified warehouse will be loaded; 0 to load all orders</param>
    /// <param name="shippingCountryId">Shipping country identifier; 0 to load all records</param>
    /// <param name="shippingStateId">Shipping state identifier; 0 to load all records</param>
    /// <param name="shippingCounty">Shipping county; null to load all records</param>
    /// <param name="shippingCity">Shipping city; null to load all records</param>
    /// <param name="trackingNumber">Search by tracking number</param>
    /// <param name="loadNotShipped">A value indicating whether we should load only not shipped shipments</param>
    /// <param name="loadNotReadyForPickup">A value indicating whether we should load only not ready for pickup shipments</param>
    /// <param name="loadNotDelivered">A value indicating whether we should load only not delivered shipments</param>
    /// <param name="orderId">Order identifier; 0 to load all records</param>
    /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipments
    /// </returns>
    public virtual async Task<IPagedList<Shipment>> GetAllShipmentsAsync(int vendorId = 0, int warehouseId = 0,
        int shippingCountryId = 0,
        int shippingStateId = 0,
        string shippingCounty = null,
        string shippingCity = null,
        string trackingNumber = null,
        bool loadNotShipped = false,
        bool loadNotReadyForPickup = false,
        bool loadNotDelivered = false,
        int orderId = 0,
        DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var shipments = await _shipmentRepository.GetAllPagedAsync(query =>
        {
            if (orderId > 0)
                query = query.Where(o => o.OrderId == orderId);

            if (!string.IsNullOrEmpty(trackingNumber))
                query = query.Where(s => s.TrackingNumber.Contains(trackingNumber));

            if (shippingCountryId > 0)
                query = from s in query
                    join o in _orderRepository.Table on s.OrderId equals o.Id
                    where _addressRepository.Table.Any(a =>
                        a.Id == (o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) &&
                        a.CountryId == shippingCountryId)
                    select s;

            if (shippingStateId > 0)
                query = from s in query
                    join o in _orderRepository.Table on s.OrderId equals o.Id
                    where _addressRepository.Table.Any(a =>
                        a.Id == (o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) &&
                        a.StateProvinceId == shippingStateId)
                    select s;

            if (!string.IsNullOrWhiteSpace(shippingCounty))
                query = from s in query
                    join o in _orderRepository.Table on s.OrderId equals o.Id
                    where _addressRepository.Table.Any(a =>
                        a.Id == (o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) &&
                        a.County.Contains(shippingCounty))
                    select s;

            if (!string.IsNullOrWhiteSpace(shippingCity))
                query = from s in query
                    join o in _orderRepository.Table on s.OrderId equals o.Id
                    where _addressRepository.Table.Any(a =>
                        a.Id == (o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) &&
                        a.City.Contains(shippingCity))
                    select s;

            if (loadNotShipped)
                query = from s in query
                    join o in _orderRepository.Table on s.OrderId equals o.Id
                    where !s.ShippedDateUtc.HasValue && !o.PickupInStore
                    select s;

            if (loadNotReadyForPickup)
                query = from s in query
                    join o in _orderRepository.Table on s.OrderId equals o.Id
                    where !s.ReadyForPickupDateUtc.HasValue && o.PickupInStore
                    select s;

            if (loadNotDelivered)
                query = query.Where(s => !s.DeliveryDateUtc.HasValue);

            if (createdFromUtc.HasValue)
                query = query.Where(s => createdFromUtc.Value <= s.CreatedOnUtc);

            if (createdToUtc.HasValue)
                query = query.Where(s => createdToUtc.Value >= s.CreatedOnUtc);

            query = from s in query
                join o in _orderRepository.Table on s.OrderId equals o.Id
                where !o.Deleted
                select s;

            query = query.Distinct();

            if (vendorId > 0)
            {
                var queryVendorOrderItems = from orderItem in _orderItemRepository.Table
                    join p in _productRepository.Table on orderItem.ProductId equals p.Id
                    where p.VendorId == vendorId
                    select orderItem.Id;

                query = from s in query
                    join si in _siRepository.Table on s.Id equals si.ShipmentId
                    where queryVendorOrderItems.Contains(si.OrderItemId)
                    select s;

                query = query.Distinct();
            }

            if (warehouseId > 0)
            {
                query = from s in query
                    join si in _siRepository.Table on s.Id equals si.ShipmentId
                    where si.WarehouseId == warehouseId
                    select s;

                query = query.Distinct();
            }

            query = query.OrderByDescending(s => s.CreatedOnUtc);

            return query;
        }, pageIndex, pageSize);

        return shipments;
    }

    /// <summary>
    /// Get shipment by identifiers
    /// </summary>
    /// <param name="shipmentIds">Shipment identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipments
    /// </returns>
    public virtual async Task<IList<Shipment>> GetShipmentsByIdsAsync(int[] shipmentIds)
    {
        return await _shipmentRepository.GetByIdsAsync(shipmentIds);
    }

    /// <summary>
    /// Gets a shipment
    /// </summary>
    /// <param name="shipmentId">Shipment identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment
    /// </returns>
    public virtual async Task<Shipment> GetShipmentByIdAsync(int shipmentId)
    {
        return await _shipmentRepository.GetByIdAsync(shipmentId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Gets a list of order shipments
    /// </summary>
    /// <param name="orderId">Order identifier</param>
    /// <param name="shipped">A value indicating whether to count only shipped or not shipped shipments; pass null to ignore</param>
    /// <param name="readyForPickup">A value indicating whether to load only ready for pickup shipments; pass null to ignore</param>
    /// <param name="vendorId">Vendor identifier; pass 0 to ignore</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<IList<Shipment>> GetShipmentsByOrderIdAsync(int orderId, bool? shipped = null, bool? readyForPickup = null, int vendorId = 0)
    {
        if (orderId == 0)
            return new List<Shipment>();

        var shipments = _shipmentRepository.Table;

        if (shipped.HasValue)
            shipments = shipments.Where(s => s.ShippedDateUtc.HasValue == shipped);

        if (readyForPickup.HasValue)
            shipments = shipments.Where(s => s.ReadyForPickupDateUtc.HasValue == readyForPickup);

        return await shipments.Where(shipment => shipment.OrderId == orderId).ToListAsync();
    }

    /// <summary>
    /// Inserts a shipment
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertShipmentAsync(Shipment shipment)
    {
        await _shipmentRepository.InsertAsync(shipment);
    }

    /// <summary>
    /// Updates the shipment
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateShipmentAsync(Shipment shipment)
    {
        await _shipmentRepository.UpdateAsync(shipment);
    }

    /// <summary>
    /// Gets a shipment items of shipment
    /// </summary>
    /// <param name="shipmentId">Shipment identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment items
    /// </returns>
    public virtual async Task<IList<ShipmentItem>> GetShipmentItemsByShipmentIdAsync(int shipmentId)
    {
        if (shipmentId == 0)
            return null;

        return await _siRepository.Table.Where(si => si.ShipmentId == shipmentId).ToListAsync();
    }

    /// <summary>
    /// Inserts a shipment item
    /// </summary>
    /// <param name="shipmentItem">Shipment item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertShipmentItemAsync(ShipmentItem shipmentItem)
    {
        await _siRepository.InsertAsync(shipmentItem);
    }

    /// <summary>
    /// Deletes a shipment item
    /// </summary>
    /// <param name="shipmentItem">Shipment Item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteShipmentItemAsync(ShipmentItem shipmentItem)
    {
        await _siRepository.DeleteAsync(shipmentItem);
    }

    /// <summary>
    /// Updates a shipment item
    /// </summary>
    /// <param name="shipmentItem">Shipment item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateShipmentItemAsync(ShipmentItem shipmentItem)
    {
        await _siRepository.UpdateAsync(shipmentItem);
    }

    /// <summary>
    /// Gets a shipment item
    /// </summary>
    /// <param name="shipmentItemId">Shipment item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment item
    /// </returns>
    public virtual async Task<ShipmentItem> GetShipmentItemByIdAsync(int shipmentItemId)
    {
        return await _siRepository.GetByIdAsync(shipmentItemId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Get quantity in shipments. For example, get planned quantity to be shipped
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="warehouseId">Warehouse identifier</param>
    /// <param name="ignoreShipped">Ignore already shipped shipments</param>
    /// <param name="ignoreDelivered">Ignore already delivered shipments</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the quantity
    /// </returns>
    public virtual async Task<int> GetQuantityInShipmentsAsync(Product product, int warehouseId,
        bool ignoreShipped, bool ignoreDelivered)
    {
        ArgumentNullException.ThrowIfNull(product);

        //only products with "use multiple warehouses" are handled this way
        if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock)
            return 0;
        if (!product.UseMultipleWarehouses)
            return 0;

        const int cancelledOrderStatusId = (int)OrderStatus.Cancelled;

        var query = _siRepository.Table;

        query = from si in query
            join s in _shipmentRepository.Table on si.ShipmentId equals s.Id
            join o in _orderRepository.Table on s.OrderId equals o.Id
            where !o.Deleted && o.OrderStatusId != cancelledOrderStatusId
            select si;

        query = query.Distinct();

        if (warehouseId > 0)
            query = query.Where(si => si.WarehouseId == warehouseId);
        if (ignoreShipped)
        {
            query = from si in query
                join s in _shipmentRepository.Table on si.ShipmentId equals s.Id
                where !s.ShippedDateUtc.HasValue
                select si;
        }

        if (ignoreDelivered)
        {
            query = from si in query
                join s in _shipmentRepository.Table on si.ShipmentId equals s.Id
                where !s.DeliveryDateUtc.HasValue
                select si;
        }

        var queryProductOrderItems = from orderItem in _orderItemRepository.Table
            where orderItem.ProductId == product.Id
            select orderItem.Id;
        query = from si in query
            where queryProductOrderItems.Any(orderItemId => orderItemId == si.OrderItemId)
            select si;

        //some null validation
        var result = Convert.ToInt32(await query.SumAsync(si => (int?)si.Quantity));
        return result;
    }

    /// <summary>
    /// Get the tracker of the shipment
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment tracker
    /// </returns>
    public virtual async Task<IShipmentTracker> GetShipmentTrackerAsync(Shipment shipment)
    {
        var order = await _orderRepository.GetByIdAsync(shipment.OrderId, cache => default, useShortTermCache: true);
        IShipmentTracker shipmentTracker = null;

        if (order.PickupInStore)
        {
            var pickupPointProvider = await _pickupPluginManager
                .LoadPluginBySystemNameAsync(order.ShippingRateComputationMethodSystemName);

            if (pickupPointProvider != null)
                shipmentTracker = await pickupPointProvider.GetShipmentTrackerAsync();
        }
        else
        {
            var shippingRateComputationMethod = await _shippingPluginManager
                .LoadPluginBySystemNameAsync(order.ShippingRateComputationMethodSystemName);

            if (shippingRateComputationMethod != null)
                shipmentTracker = await shippingRateComputationMethod.GetShipmentTrackerAsync();
        }

        return shipmentTracker;
    }

    #endregion
}