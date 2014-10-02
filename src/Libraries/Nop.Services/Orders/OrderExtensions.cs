using System;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Core.Html;

namespace Nop.Services.Orders
{
    public static class OrderExtensions
    {
        /// <summary>
        /// Formats the order note text
        /// </summary>
        /// <param name="orderNote">Order note</param>
        /// <returns>Formatted text</returns>
        public static string FormatOrderNoteText(this OrderNote orderNote)
        {
            if (orderNote == null)
                throw new ArgumentNullException("orderNote");

            string text = orderNote.Note;

            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);

            return text;
        }

        /// <summary>
        /// Gets a total number of items in all shipments
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>Total number of items in all shipmentss</returns>
        public static int GetTotalNumberOfItemsInAllShipment(this OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException("orderItem");

            var totalInShipments = 0;
            var shipments = orderItem.Order.Shipments.ToList();
            for (int i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                var si = shipment.ShipmentItems
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
        public static int GetTotalNumberOfItemsCanBeAddedToShipment(this OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException("orderItem");

            var totalInShipments = orderItem.GetTotalNumberOfItemsInAllShipment();

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
        public static int GetTotalNumberOfNotYetShippedItems(this OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException("orderItem");

            var result = 0;
            var shipments = orderItem.Order.Shipments.ToList();
            for (int i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                if (shipment.ShippedDateUtc.HasValue)
                    //already shipped
                    continue;

                var si = shipment.ShipmentItems
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
        public static int GetTotalNumberOfShippedItems(this OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException("orderItem");

            var result = 0;
            var shipments = orderItem.Order.Shipments.ToList();
            for (int i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                if (!shipment.ShippedDateUtc.HasValue)
                    //not shipped yet
                    continue;

                var si = shipment.ShipmentItems
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
        /// <param name="orderItem">Order  item</param>
        /// <returns>Total number of already delivered items</returns>
        public static int GetTotalNumberOfDeliveredItems(this OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException("orderItem");

            var result = 0;
            var shipments = orderItem.Order.Shipments.ToList();
            for (int i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                if (!shipment.DeliveryDateUtc.HasValue)
                    //not delivered yet
                    continue;

                var si = shipment.ShipmentItems
                    .FirstOrDefault(x => x.OrderItemId == orderItem.Id);
                if (si != null)
                {
                    result += si.Quantity;
                }
            }

            return result;
        }



        /// <summary>
        /// Gets a value indicating whether an order has items to be added to a shipment
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether an order has items to be added to a shipment</returns>
        public static bool HasItemsToAddToShipment(this Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            foreach (var orderItem in order.OrderItems)
            {
                //we can ship only shippable products
                if (!orderItem.Product.IsShipEnabled)
                    continue;

                var totalNumberOfItemsCanBeAddedToShipment = orderItem.GetTotalNumberOfItemsCanBeAddedToShipment();
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
        public static bool HasItemsToShip(this Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            foreach (var orderItem in order.OrderItems)
            {
                //we can ship only shippable products
                if (!orderItem.Product.IsShipEnabled)
                    continue;

                var totalNumberOfNotYetShippedItems = orderItem.GetTotalNumberOfNotYetShippedItems();
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
        public static bool HasItemsToDeliver(this Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            foreach (var orderItem in order.OrderItems)
            {
                //we can ship only shippable products
                if (!orderItem.Product.IsShipEnabled)
                    continue;

                var totalNumberOfShippedItems = orderItem.GetTotalNumberOfShippedItems();
                var totalNumberOfDeliveredItems = orderItem.GetTotalNumberOfDeliveredItems();
                if (totalNumberOfShippedItems <= totalNumberOfDeliveredItems)
                    continue;

                //yes, we have at least one item to deliver
                return true;
            }
            return false;
        }
    }
}
