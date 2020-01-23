using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Html;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Shipping;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order service
    /// </summary>
    public partial class OrderService : IOrderService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IProductService _productService;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductWarehouseInventory> _productWarehouseInventoryRepository;
        private readonly IRepository<RecurringPayment> _recurringPaymentRepository;
        private readonly IRepository<RecurringPaymentHistory> _recurringPaymentHistoryRepository;
        private readonly IShipmentService _shipmentService;

        #endregion

        #region Ctor

        public OrderService(IEventPublisher eventPublisher,
            IProductService productService,
            IRepository<Address> addressRepository,
            IRepository<Customer> customerRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<OrderNote> orderNoteRepository,
            IRepository<Product> productRepository,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
            IRepository<RecurringPayment> recurringPaymentRepository,
            IRepository<RecurringPaymentHistory> recurringPaymentHistoryRepository,
            IShipmentService shipmentService)
        {
            _eventPublisher = eventPublisher;
            _productService = productService;
            _addressRepository = addressRepository;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _orderNoteRepository = orderNoteRepository;
            _productRepository = productRepository;
            _productWarehouseInventoryRepository = productWarehouseInventoryRepository;
            _recurringPaymentRepository = recurringPaymentRepository;
            _recurringPaymentHistoryRepository = recurringPaymentHistoryRepository;
            _shipmentService = shipmentService;
        }

        #endregion

        #region Methods

        #region Orders

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderById(int orderId)
        {
            if (orderId == 0)
                return null;

            return _orderRepository.ToCachedGetById(orderId);
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="customOrderNumber">The custom order number</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderByCustomOrderNumber(string customOrderNumber)
        {
            if (string.IsNullOrEmpty(customOrderNumber))
                return null;

            return _orderRepository.Table.FirstOrDefault(o => o.CustomOrderNumber == customOrderNumber);
        }

        /// <summary>
        /// Gets an order by order item identifier
        /// </summary>
        /// <param name="orderItemId">The order item identifier</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderByOrderItem(int orderItemId)
        {
            if (orderItemId == 0)
                return null;

            return (from o in _orderRepository.Table
                    join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                    where oi.Id == orderItemId
                    select o).FirstOrDefault();
        }

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">Order identifiers</param>
        /// <returns>Order</returns>
        public virtual IList<Order> GetOrdersByIds(int[] orderIds)
        {
            if (orderIds == null || orderIds.Length == 0)
                return new List<Order>();

            var query = from o in _orderRepository.Table
                        where orderIds.Contains(o.Id) && !o.Deleted
                        select o;
            var orders = query.ToList();
            //sort by passed identifiers
            var sortedOrders = new List<Order>();
            foreach (var id in orderIds)
            {
                var order = orders.Find(x => x.Id == id);
                if (order != null)
                    sortedOrders.Add(order);
            }

            return sortedOrders;
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderByGuid(Guid orderGuid)
        {
            if (orderGuid == Guid.Empty)
                return null;

            var query = from o in _orderRepository.Table
                        where o.OrderGuid == orderGuid
                        select o;
            var order = query.FirstOrDefault();
            return order;
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void DeleteOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            order.Deleted = true;
            UpdateOrder(order);

            //event notification
            _eventPublisher.EntityDeleted(order);
        }

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all orders</param>
        /// <param name="vendorId">Vendor identifier; null to load all orders</param>
        /// <param name="customerId">Customer identifier; 0 to load all orders</param>
        /// <param name="productId">Product identifier which was purchased in an order; 0 to load all orders</param>
        /// <param name="affiliateId">Affiliate identifier; 0 to load all orders</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="warehouseId">Warehouse identifier, only orders with products from a specified warehouse will be loaded; 0 to load all orders</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="osIds">Order status identifiers; null to load all orders</param>
        /// <param name="psIds">Payment status identifiers; null to load all orders</param>
        /// <param name="ssIds">Shipping status identifiers; null to load all orders</param>
        /// <param name="billingPhone">Billing phone. Leave empty to load all records.</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>Orders</returns>
        public virtual IPagedList<Order> SearchOrders(int storeId = 0,
            int vendorId = 0, int customerId = 0,
            int productId = 0, int affiliateId = 0, int warehouseId = 0,
            int billingCountryId = 0, string paymentMethodSystemName = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            string billingPhone = null, string billingEmail = null, string billingLastName = "",
            string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _orderRepository.Table;

            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);

            if (vendorId > 0)
                query = from o in query
                    join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                    join p in _productRepository.Table on oi.ProductId equals p.Id
                    where p.VendorId == vendorId
                    select o;

            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);

            if (productId > 0)
                query = from o in query
                    join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                    where oi.ProductId == productId
                    select o;

            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;

                query = from o in query
                    join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                    join p in _productRepository.Table on oi.ProductId equals p.Id
                    join pwi in _productWarehouseInventoryRepository.Table on p.Id equals pwi.ProductId
                    where
                        //"Use multiple warehouses" enabled
                        //we search in each warehouse
                        (p.ManageInventoryMethodId == manageStockInventoryMethodId && p.UseMultipleWarehouses && pwi.WarehouseId == warehouseId) ||
                        //"Use multiple warehouses" disabled
                        //we use standard "warehouse" property
                        ((p.ManageInventoryMethodId != manageStockInventoryMethodId || !p.UseMultipleWarehouses) && p.WarehouseId == warehouseId)
                    select o;
            }

            if (!string.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);

            if (affiliateId > 0)
                query = query.Where(o => o.AffiliateId == affiliateId);

            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);

            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);

            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));

            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));

            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));

            if (!string.IsNullOrEmpty(orderNotes))
                query = query.Where(o => _orderNoteRepository.Table.Any(oNote => oNote.OrderId == o.Id && oNote.Note.Contains(orderNotes)));

            query = from o in query
                join oba in _addressRepository.Table on o.BillingAddressId equals oba.Id
                where
                    (billingCountryId <= 0 || (oba.CountryId == billingCountryId)) &&
                    (string.IsNullOrEmpty(billingPhone) || (!string.IsNullOrEmpty(oba.PhoneNumber) && oba.PhoneNumber.Contains(billingPhone))) &&
                    (string.IsNullOrEmpty(billingEmail) || (!string.IsNullOrEmpty(oba.Email) && oba.Email.Contains(billingEmail))) &&
                    (string.IsNullOrEmpty(billingLastName) || (!string.IsNullOrEmpty(oba.LastName) && oba.LastName.Contains(billingLastName)))
                select o;

            query = query.Where(o => !o.Deleted);
            query = query.OrderByDescending(o => o.CreatedOnUtc);

            //database layer paging
            return new PagedList<Order>(query, pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void InsertOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            _orderRepository.Insert(order);

            //event notification
            _eventPublisher.EntityInserted(order);
        }

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void UpdateOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            _orderRepository.Update(order);

            //event notification
            _eventPublisher.EntityUpdated(order);
        }

        /// <summary>
        /// Get an order by authorization transaction ID and payment method system name
        /// </summary>
        /// <param name="authorizationTransactionId">Authorization transaction ID</param>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderByAuthorizationTransactionIdAndPaymentMethod(string authorizationTransactionId,
            string paymentMethodSystemName)
        {
            var query = _orderRepository.Table;
            if (!string.IsNullOrWhiteSpace(authorizationTransactionId))
                query = query.Where(o => o.AuthorizationTransactionId == authorizationTransactionId);

            if (!string.IsNullOrWhiteSpace(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);

            query = query.OrderByDescending(o => o.CreatedOnUtc);
            var order = query.FirstOrDefault();
            return order;
        }

        /// <summary>
        /// Parse tax rates
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="taxRatesStr"></param>
        /// <returns>Rates</returns>
        public virtual SortedDictionary<decimal, decimal> ParseTaxRates(Order order, string taxRatesStr)
        {
            var taxRatesDictionary = new SortedDictionary<decimal, decimal>();

            if (string.IsNullOrEmpty(taxRatesStr))
                return taxRatesDictionary;

            var lines = taxRatesStr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                    continue;

                var taxes = line.Split(':');
                if (taxes.Length != 2)
                    continue;

                try
                {
                    var taxRate = decimal.Parse(taxes[0].Trim(), CultureInfo.InvariantCulture);
                    var taxValue = decimal.Parse(taxes[1].Trim(), CultureInfo.InvariantCulture);
                    taxRatesDictionary.Add(taxRate, taxValue);
                }
                catch
                {
                    // ignored
                }
            }

            //add at least one tax rate (0%)
            if (!taxRatesDictionary.Any())
                taxRatesDictionary.Add(decimal.Zero, decimal.Zero);

            return taxRatesDictionary;
        }

        /// <summary>
        /// Gets a value indicating whether an order has items to be added to a shipment
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether an order has items to be added to a shipment</returns>
        public virtual bool HasItemsToAddToShipment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            foreach (var orderItem in GetOrderItems(order.Id, isShipEnabled: true)) //we can ship only shippable products
            {
                var totalNumberOfItemsCanBeAddedToShipment = GetTotalNumberOfItemsCanBeAddedToShipment(orderItem);
                if (totalNumberOfItemsCanBeAddedToShipment <= 0)
                    continue;

                //yes, we have at least one item to create a new shipment
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether an order has items to ship
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether an order has items to ship</returns>
        public virtual bool HasItemsToShip(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            foreach (var orderItem in GetOrderItems(order.Id, isShipEnabled: true)) //we can ship only shippable products
            {
                var totalNumberOfNotYetShippedItems = GetTotalNumberOfNotYetShippedItems(orderItem);
                if (totalNumberOfNotYetShippedItems <= 0)
                    continue;

                //yes, we have at least one item to ship
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether an order has items to deliver
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether an order has items to deliver</returns>
        public virtual bool HasItemsToDeliver(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            foreach (var orderItem in GetOrderItems(order.Id, isShipEnabled: true)) //we can ship only shippable products
            {
                var totalNumberOfShippedItems = GetTotalNumberOfShippedItems(orderItem);
                var totalNumberOfDeliveredItems = GetTotalNumberOfDeliveredItems(orderItem);
                if (totalNumberOfShippedItems <= totalNumberOfDeliveredItems)
                    continue;

                //yes, we have at least one item to deliver
                return true;
            }

            return false;
        }

        #endregion

        #region Orders items

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>Order item</returns>
        public virtual OrderItem GetOrderItemById(int orderItemId)
        {
            if (orderItemId == 0)
                return null;

            return _orderItemRepository.ToCachedGetById(orderItemId);
        }

        /// <summary>
        /// Gets a product of specify order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>Product</returns>
        public virtual Product GetProductByOrderItemId(int orderItemId)
        {
            if (orderItemId == 0)
                return null;

            return (from p in _productRepository.Table
                    join oi in _orderItemRepository.Table on p.Id equals oi.ProductId
                    where oi.Id == orderItemId
                    select p).SingleOrDefault();
        }

        /// <summary>
        /// Gets a list items of order
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="isNotReturnable">Value indicating whether this product is returnable; pass null to ignore</param>
        /// <param name="isShipEnabled">Value indicating whether the entity is ship enabled; pass null to ignore</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore</param>
        /// <returns>Result</returns>
        public virtual IList<OrderItem> GetOrderItems(int orderId, bool? isNotReturnable = null, bool? isShipEnabled = null, int vendorId = 0)
        {
            if (orderId == 0)
                return new List<OrderItem>();

            return (from oi in _orderItemRepository.Table
                    join p in _productRepository.Table on oi.ProductId equals p.Id
                    where
                    oi.OrderId == orderId &&
                    (!isShipEnabled.HasValue || (p.IsShipEnabled == isShipEnabled.Value)) &&
                    (!isNotReturnable.HasValue || (p.NotReturnable == isNotReturnable)) &&
                    (vendorId <= 0 || (p.VendorId == vendorId))
                    select oi).ToList();
        }

        /// <summary>
        /// Gets an item
        /// </summary>
        /// <param name="orderItemGuid">Order identifier</param>
        /// <returns>Order item</returns>
        public virtual OrderItem GetOrderItemByGuid(Guid orderItemGuid)
        {
            if (orderItemGuid == Guid.Empty)
                return null;

            var query = from orderItem in _orderItemRepository.Table
                        where orderItem.OrderItemGuid == orderItemGuid
                        select orderItem;
            var item = query.FirstOrDefault();
            return item;
        }

        /// <summary>
        /// Gets all downloadable order items
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <returns>Order items</returns>
        public virtual IList<OrderItem> GetDownloadableOrderItems(int customerId)
        {
            if (customerId == 0)
                throw new ArgumentOutOfRangeException(nameof(customerId));

            var query = from orderItem in _orderItemRepository.Table
                        join o in _orderRepository.Table on orderItem.OrderId equals o.Id
                        join p in _productRepository.Table on orderItem.ProductId equals p.Id
                        where customerId == o.CustomerId &&
                        p.IsDownload &&
                        !o.Deleted
                        orderby o.CreatedOnUtc descending, orderItem.Id
                        select orderItem;

            var orderItems = query.ToList();
            return orderItems;
        }

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        public virtual void DeleteOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            _orderItemRepository.Delete(orderItem);

            //event notification
            _eventPublisher.EntityDeleted(orderItem);
        }

        /// <summary>
        /// Gets a total number of items in all shipments
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>Total number of items in all shipments</returns>
        public virtual int GetTotalNumberOfItemsInAllShipment(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var totalInShipments = 0;
            var shipments = _shipmentService.GetShipmentsByOrderId(orderItem.OrderId);

            for (var i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                var si = _shipmentService.GetShipmentItemsByShipmentId(shipment.Id)
                    .FirstOrDefault(x => x.OrderItemId == orderItem.Id);
                if (si != null)
                {
                    totalInShipments += si.Quantity;
                }
            }

            return totalInShipments;
        }

        /// <summary>
        /// Gets a total number of already items which can be added to new shipments
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>Total number of already delivered items which can be added to new shipments</returns>
        public virtual int GetTotalNumberOfItemsCanBeAddedToShipment(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var totalInShipments = GetTotalNumberOfItemsInAllShipment(orderItem);

            var qtyOrdered = orderItem.Quantity;
            var qtyCanBeAddedToShipmentTotal = qtyOrdered - totalInShipments;
            if (qtyCanBeAddedToShipmentTotal < 0)
                qtyCanBeAddedToShipmentTotal = 0;

            return qtyCanBeAddedToShipmentTotal;
        }

        /// <summary>
        /// Gets a total number of not yet shipped items (but added to shipments)
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>Total number of not yet shipped items (but added to shipments)</returns>
        public virtual int GetTotalNumberOfNotYetShippedItems(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var result = 0;
            var shipments = _shipmentService.GetShipmentsByOrderId(orderItem.OrderId);
            for (var i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                if (shipment.ShippedDateUtc.HasValue)
                    //already shipped
                    continue;

                var si = _shipmentService.GetShipmentItemsByShipmentId(shipment.Id)
                    .FirstOrDefault(x => x.OrderItemId == orderItem.Id);
                if (si != null)
                {
                    result += si.Quantity;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a total number of already shipped items
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>Total number of already shipped items</returns>
        public virtual int GetTotalNumberOfShippedItems(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var result = 0;
            var shipments = _shipmentService.GetShipmentsByOrderId(orderItem.OrderId);
            for (var i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                if (!shipment.ShippedDateUtc.HasValue)
                    //not shipped yet
                    continue;

                var si = _shipmentService.GetShipmentItemsByShipmentId(shipment.Id)
                    .FirstOrDefault(x => x.OrderItemId == orderItem.Id);
                if (si != null)
                {
                    result += si.Quantity;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a total number of already delivered items
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>Total number of already delivered items</returns>
        public virtual int GetTotalNumberOfDeliveredItems(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var result = 0;
            var shipments = _shipmentService.GetShipmentsByOrderId(orderItem.OrderId);

            for (var i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                if (!shipment.DeliveryDateUtc.HasValue)
                    //not delivered yet
                    continue;

                var si = _shipmentService.GetShipmentItemsByShipmentId(shipment.Id)
                    .FirstOrDefault(x => x.OrderItemId == orderItem.Id);
                if (si != null)
                {
                    result += si.Quantity;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether download is allowed
        /// </summary>
        /// <param name="orderItem">Order item to check</param>
        /// <returns>True if download is allowed; otherwise, false.</returns>
        public virtual bool IsDownloadAllowed(OrderItem orderItem)
        {
            if (orderItem is null)
                return false;

            var order = GetOrderById(orderItem.OrderId);
            if (order == null || order.Deleted)
                return false;

            //order status
            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            var product = _productService.GetProductById(orderItem.ProductId);

            if (product == null || !product.IsDownload)
                return false;

            //payment status
            switch (product.DownloadActivationType)
            {
                case DownloadActivationType.WhenOrderIsPaid:
                    if (order.PaymentStatus == PaymentStatus.Paid && order.PaidDateUtc.HasValue)
                    {
                        //expiration date
                        if (product.DownloadExpirationDays.HasValue)
                        {
                            if (order.PaidDateUtc.Value.AddDays(product.DownloadExpirationDays.Value) > DateTime.UtcNow)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }

                    break;
                case DownloadActivationType.Manually:
                    if (orderItem.IsDownloadActivated)
                    {
                        //expiration date
                        if (product.DownloadExpirationDays.HasValue)
                        {
                            if (order.CreatedOnUtc.AddDays(product.DownloadExpirationDays.Value) > DateTime.UtcNow)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }

                    break;
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether license download is allowed
        /// </summary>
        /// <param name="orderItem">Order item to check</param>
        /// <returns>True if license download is allowed; otherwise, false.</returns>
        public virtual bool IsLicenseDownloadAllowed(OrderItem orderItem)
        {
            if (orderItem == null)
                return false;

            return IsDownloadAllowed(orderItem) &&
                orderItem.LicenseDownloadId.HasValue &&
                orderItem.LicenseDownloadId > 0;
        }

        /// <summary>
        /// Inserts a order item
        /// </summary>
        /// <param name="orderItem">Order item</param>
        public virtual void InsertOrderItem(OrderItem orderItem)
        {
            if (orderItem is null)
                throw new ArgumentNullException(nameof(orderItem));

            _orderItemRepository.Insert(orderItem);

            _eventPublisher.EntityInserted(orderItem);
        }

        /// <summary>
        /// Updates a order item
        /// </summary>
        /// <param name="orderItem">Order item</param>
        public virtual void UpdateOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            _orderItemRepository.Update(orderItem);

            //event notification
            _eventPublisher.EntityUpdated(orderItem);
        }

        #endregion

        #region Orders notes

        /// <summary>
        /// Gets an order note
        /// </summary>
        /// <param name="orderNoteId">The order note identifier</param>
        /// <returns>Order note</returns>
        public virtual OrderNote GetOrderNoteById(int orderNoteId)
        {
            if (orderNoteId == 0)
                return null;

            return _orderNoteRepository.ToCachedGetById(orderNoteId);
        }

        /// <summary>
        /// Gets a list notes of order
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="displayToCustomer">Value indicating whether a customer can see a note; pass null to ignore</param>
        /// <returns>Result</returns>
        public virtual IList<OrderNote> GetOrderNotesByOrderId(int orderId, bool? displayToCustomer = null)
        {
            if (orderId == 0)
                return new List<OrderNote>();

            var query = _orderNoteRepository.Table.Where(on => on.OrderId == orderId);

            if (displayToCustomer.HasValue)
            {
                query = query.Where(on => on.DisplayToCustomer == displayToCustomer);
            }

            return query.ToList();
        }

        /// <summary>
        /// Deletes an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        public virtual void DeleteOrderNote(OrderNote orderNote)
        {
            if (orderNote == null)
                throw new ArgumentNullException(nameof(orderNote));

            _orderNoteRepository.Delete(orderNote);

            //event notification
            _eventPublisher.EntityDeleted(orderNote);
        }

        /// <summary>
        /// Formats the order note text
        /// </summary>
        /// <param name="orderNote">Order note</param>
        /// <returns>Formatted text</returns>
        public virtual string FormatOrderNoteText(OrderNote orderNote)
        {
            if (orderNote == null)
                throw new ArgumentNullException(nameof(orderNote));

            var text = orderNote.Note;

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);

            return text;
        }

        /// <summary>
        /// Inserts an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        public virtual void InsertOrderNote(OrderNote orderNote)
        {
            if (orderNote is null)
                throw new ArgumentNullException(nameof(orderNote));

            _orderNoteRepository.Insert(orderNote);

            //event notification
            _eventPublisher.EntityInserted(orderNote);
        }

        #endregion

        #region Recurring payments

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void DeleteRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            recurringPayment.Deleted = true;
            UpdateRecurringPayment(recurringPayment);

            //event notification
            _eventPublisher.EntityDeleted(recurringPayment);
        }

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>Recurring payment</returns>
        public virtual RecurringPayment GetRecurringPaymentById(int recurringPaymentId)
        {
            if (recurringPaymentId == 0)
                return null;

            return _recurringPaymentRepository.ToCachedGetById(recurringPaymentId);
        }

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void InsertRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            _recurringPaymentRepository.Insert(recurringPayment);

            //event notification
            _eventPublisher.EntityInserted(recurringPayment);
        }

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual void UpdateRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            _recurringPaymentRepository.Update(recurringPayment);

            //event notification
            _eventPublisher.EntityUpdated(recurringPayment);
        }

        /// <summary>
        /// Search recurring payments
        /// </summary>
        /// <param name="storeId">The store identifier; 0 to load all records</param>
        /// <param name="customerId">The customer identifier; 0 to load all records</param>
        /// <param name="initialOrderId">The initial order identifier; 0 to load all records</param>
        /// <param name="initialOrderStatus">Initial order status identifier; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Recurring payments</returns>
        public virtual IPagedList<RecurringPayment> SearchRecurringPayments(int storeId = 0,
            int customerId = 0, int initialOrderId = 0, OrderStatus? initialOrderStatus = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            int? initialOrderStatusId = null;
            if (initialOrderStatus.HasValue)
                initialOrderStatusId = (int)initialOrderStatus.Value;

            var query1 = from rp in _recurringPaymentRepository.Table
                         join o in _orderRepository.Table on rp.InitialOrderId equals o.Id
                         join c in _customerRepository.Table on o.CustomerId equals c.Id
                         where
                         !rp.Deleted &&
                         (showHidden || !o.Deleted) &&
                         (showHidden || !c.Deleted) &&
                         (showHidden || rp.IsActive) &&
                         (customerId == 0 || o.CustomerId == customerId) &&
                         (storeId == 0 || o.StoreId == storeId) &&
                         (initialOrderId == 0 || o.Id == initialOrderId) &&
                         (!initialOrderStatusId.HasValue || initialOrderStatusId.Value == 0 ||
                          o.OrderStatusId == initialOrderStatusId.Value)
                         select rp.Id;

            var query2 = from rp in _recurringPaymentRepository.Table
                         where query1.Contains(rp.Id)
                         orderby rp.StartDateUtc, rp.Id
                         select rp;

            var recurringPayments = new PagedList<RecurringPayment>(query2, pageIndex, pageSize);
            return recurringPayments;
        }

        #endregion

        #region Recurring payments history

        /// <summary>
        /// Gets a recurring payment history
        /// </summary>
        /// <param name="recurringPayment">The recurring payment</param>
        /// <returns>Result</returns>
        public virtual IList<RecurringPaymentHistory> GetRecurringPaymentHistory(RecurringPayment recurringPayment)
        {
            if (recurringPayment is null)
                throw new ArgumentNullException(nameof(recurringPayment));

            return _recurringPaymentHistoryRepository.Table.Where(rph => rph.RecurringPaymentId == recurringPayment.Id).ToList();
        }

        /// <summary>
        /// Inserts a recurring payment history entry
        /// </summary>
        /// <param name="recurringPaymentHistory">Recurring payment history entry</param>
        public virtual void InsertRecurringPaymentHistory(RecurringPaymentHistory recurringPaymentHistory)
        {
            if (recurringPaymentHistory == null)
                throw new ArgumentNullException(nameof(recurringPaymentHistory));

            _recurringPaymentHistoryRepository.Insert(recurringPaymentHistory);

            //event notification
            _eventPublisher.EntityInserted(recurringPaymentHistory);
        }

        #endregion

        #endregion
    }
}