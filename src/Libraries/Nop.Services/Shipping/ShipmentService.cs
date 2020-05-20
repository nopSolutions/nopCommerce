using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Shipping.Tracking;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipment service
    /// </summary>
    public partial class ShipmentService : IShipmentService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IPickupPluginManager _pickupPluginManager;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IRepository<ShipmentItem> _siRepository;
        private readonly IShippingPluginManager _shippingPluginManager;

        #endregion

        #region Ctor

        public ShipmentService(IEventPublisher eventPublisher,
            IPickupPluginManager pickupPluginManager,
            IRepository<Address> addressRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product> productRepository,
            IRepository<Shipment> shipmentRepository,
            IRepository<ShipmentItem> siRepository,
            IShippingPluginManager shippingPluginManager)
        {
            _eventPublisher = eventPublisher;
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
        public virtual void DeleteShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            _shipmentRepository.Delete(shipment);

            //event notification
            _eventPublisher.EntityDeleted(shipment);
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
        /// <param name="loadNotDelivered">A value indicating whether we should load only not delivered shipments</param>
        /// <param name="orderId">Order identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Shipments</returns>
        public virtual IPagedList<Shipment> GetAllShipments(int vendorId = 0, int warehouseId = 0,
            int shippingCountryId = 0,
            int shippingStateId = 0,
            string shippingCounty = null,
            string shippingCity = null,
            string trackingNumber = null,
            bool loadNotShipped = false,
            bool loadNotDelivered = false,
            int orderId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _shipmentRepository.Table;

            if(orderId > 0)
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
                query = query.Where(s => !s.ShippedDateUtc.HasValue);

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

            var shipments = new PagedList<Shipment>(query, pageIndex, pageSize);
            return shipments;
        }

        /// <summary>
        /// Get shipment by identifiers
        /// </summary>
        /// <param name="shipmentIds">Shipment identifiers</param>
        /// <returns>Shipments</returns>
        public virtual IList<Shipment> GetShipmentsByIds(int[] shipmentIds)
        {
            if (shipmentIds == null || shipmentIds.Length == 0)
                return new List<Shipment>();

            var query = from o in _shipmentRepository.Table
                        where shipmentIds.Contains(o.Id)
                        select o;
            var shipments = query.ToList();
            //sort by passed identifiers
            var sortedOrders = new List<Shipment>();
            foreach (var id in shipmentIds)
            {
                var shipment = shipments.Find(x => x.Id == id);
                if (shipment != null)
                    sortedOrders.Add(shipment);
            }

            return sortedOrders;
        }

        /// <summary>
        /// Gets a shipment
        /// </summary>
        /// <param name="shipmentId">Shipment identifier</param>
        /// <returns>Shipment</returns>
        public virtual Shipment GetShipmentById(int shipmentId)
        {
            if (shipmentId == 0)
                return null;

            return _shipmentRepository.GetById(shipmentId);
        }

        /// <summary>
        /// Gets a list of order shipments
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="shipped">A value indicating whether to count only shipped or not shipped shipments; pass null to ignore</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore</param>
        /// <returns>Result</returns>
        public virtual IList<Shipment> GetShipmentsByOrderId(int orderId, bool? shipped = null, int vendorId = 0)
        {
            if (orderId == 0)
                return new List<Shipment>();

            var shipments = _shipmentRepository.Table;

            if (shipped.HasValue)
            {
                shipments = shipments.Where(s => s.ShippedDateUtc.HasValue == shipped);
            }

            return shipments.Where(shipment => shipment.OrderId == orderId).ToList();
        }

        /// <summary>
        /// Inserts a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        public virtual void InsertShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            _shipmentRepository.Insert(shipment);

            //event notification
            _eventPublisher.EntityInserted(shipment);
        }

        /// <summary>
        /// Updates the shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        public virtual void UpdateShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            _shipmentRepository.Update(shipment);

            //event notification
            _eventPublisher.EntityUpdated(shipment);
        }

        /// <summary>
        /// Deletes a shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        public virtual void DeleteShipmentItem(ShipmentItem shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem));

            _siRepository.Delete(shipmentItem);

            //event notification
            _eventPublisher.EntityDeleted(shipmentItem);
        }

        /// <summary>
        /// Gets a shipment items of shipment
        /// </summary>
        /// <param name="shipmentId">Shipment identifier</param>
        /// <returns>Shipment items</returns>
        public virtual IList<ShipmentItem> GetShipmentItemsByShipmentId(int shipmentId)
        {
            if (shipmentId == 0)
                return null;

            return _siRepository.Table.Where(si => si.ShipmentId == shipmentId).ToList();
        }

        /// <summary>
        /// Gets a shipment item
        /// </summary>
        /// <param name="shipmentItemId">Shipment item identifier</param>
        /// <returns>Shipment item</returns>
        public virtual ShipmentItem GetShipmentItemById(int shipmentItemId)
        {
            if (shipmentItemId == 0)
                return null;

            return _siRepository.GetById(shipmentItemId);
        }

        /// <summary>
        /// Inserts a shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        public virtual void InsertShipmentItem(ShipmentItem shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem));

            _siRepository.Insert(shipmentItem);

            //event notification
            _eventPublisher.EntityInserted(shipmentItem);
        }

        /// <summary>
        /// Updates the shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        public virtual void UpdateShipmentItem(ShipmentItem shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem));

            _siRepository.Update(shipmentItem);

            //event notification
            _eventPublisher.EntityUpdated(shipmentItem);
        }

        /// <summary>
        /// Get quantity in shipments. For example, get planned quantity to be shipped
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="ignoreShipped">Ignore already shipped shipments</param>
        /// <param name="ignoreDelivered">Ignore already delivered shipments</param>
        /// <returns>Quantity</returns>
        public virtual int GetQuantityInShipments(Product product, int warehouseId,
            bool ignoreShipped, bool ignoreDelivered)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

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
            var result = Convert.ToInt32(query.Sum(si => (int?)si.Quantity));
            return result;
        }

        /// <summary>
        /// Get the tracker of the shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>Shipment tracker</returns>
        public virtual IShipmentTracker GetShipmentTracker(Shipment shipment)
        {
            var order = _orderRepository.ToCachedGetById(shipment.OrderId);

            if (!order.PickupInStore)
            {
                var shippingRateComputationMethod = _shippingPluginManager
                    .LoadPluginBySystemName(order.ShippingRateComputationMethodSystemName);

                return shippingRateComputationMethod?.ShipmentTracker;
            }

            var pickupPointProvider = _pickupPluginManager
                .LoadPluginBySystemName(order.ShippingRateComputationMethodSystemName);
            return pickupPointProvider?.ShipmentTracker;
        }

        #endregion
    }
}