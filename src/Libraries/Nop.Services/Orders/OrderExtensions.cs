using System;
using System.Linq;
using Nop.Core.Domain.Orders;

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

            text = Nop.Core.Html.HtmlHelper.FormatText(text, false, true, false, false, false, false);

            return text;
        }

        /// <summary>
        /// Gets a total number of items in all shipments
        /// </summary>
        /// <param name="opv">Order product variant</param>
        /// <returns>Total number of items in all shipmentss</returns>
        public static int GetTotalNumberOfItemsInAllShipment(this OrderProductVariant opv)
        {
            if (opv == null)
                throw new ArgumentNullException("opv");

            var totalInShipments = 0;
            var shipments = opv.Order.Shipments.ToList();
            for (int i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                var sopv = shipment.ShipmentOrderProductVariants
                    .FirstOrDefault(x => x.OrderProductVariantId == opv.Id);
                if (sopv != null)
                {
                    totalInShipments += sopv.Quantity;
                }
            }
            return totalInShipments;
        }

        /// <summary>
        /// Gets a total number of already items which can be added to new shipments
        /// </summary>
        /// <param name="opv">Order product variant</param>
        /// <returns>Total number of already delivered items which can be added to new shipments</returns>
        public static int GetTotalNumberOfItemsCanBeAddedToShipment(this OrderProductVariant opv)
        {
            if (opv == null)
                throw new ArgumentNullException("opv");

            var totalInShipments = opv.GetTotalNumberOfItemsInAllShipment();

            var qtyOrdered = opv.Quantity;
            var qtyCanBeAddedToShipmentTotal = qtyOrdered - totalInShipments;
            if (qtyCanBeAddedToShipmentTotal < 0)
                qtyCanBeAddedToShipmentTotal = 0;

            return qtyCanBeAddedToShipmentTotal;
        }

        /// <summary>
        /// Gets a total number of not yet shipped items (but added to shipments)
        /// </summary>
        /// <param name="opv">Order product variant</param>
        /// <returns>Total number of not yet shipped items (but added to shipments)</returns>
        public static int GetTotalNumberOfNotYetShippedItems(this OrderProductVariant opv)
        {
            if (opv == null)
                throw new ArgumentNullException("opv");

            var result = 0;
            var shipments = opv.Order.Shipments.ToList();
            for (int i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                if (shipment.ShippedDateUtc.HasValue)
                    //already shipped
                    continue;

                var sopv = shipment.ShipmentOrderProductVariants
                    .FirstOrDefault(x => x.OrderProductVariantId == opv.Id);
                if (sopv != null)
                {
                    result += sopv.Quantity;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a total number of already shipped items
        /// </summary>
        /// <param name="opv">Order product variant</param>
        /// <returns>Total number of already shipped items</returns>
        public static int GetTotalNumberOfShippedItems(this OrderProductVariant opv)
        {
            if (opv == null)
                throw new ArgumentNullException("opv");

            var result = 0;
            var shipments = opv.Order.Shipments.ToList();
            for (int i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                if (!shipment.ShippedDateUtc.HasValue)
                    //not shipped yet
                    continue;

                var sopv = shipment.ShipmentOrderProductVariants
                    .FirstOrDefault(x => x.OrderProductVariantId == opv.Id);
                if (sopv != null)
                {
                    result += sopv.Quantity;
                }
            }
            
            return result;
        }

        /// <summary>
        /// Gets a total number of already delivered items
        /// </summary>
        /// <param name="opv">Order product variant</param>
        /// <returns>Total number of already delivered items</returns>
        public static int GetTotalNumberOfDeliveredItems(this OrderProductVariant opv)
        {
            if (opv == null)
                throw new ArgumentNullException("opv");

            var result = 0;
            var shipments = opv.Order.Shipments.ToList();
            for (int i = 0; i < shipments.Count; i++)
            {
                var shipment = shipments[i];
                if (!shipment.DeliveryDateUtc.HasValue)
                    //not delivered yet
                    continue;

                var sopv = shipment.ShipmentOrderProductVariants
                    .FirstOrDefault(x => x.OrderProductVariantId == opv.Id);
                if (sopv != null)
                {
                    result += sopv.Quantity;
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

            foreach (var opv in order.OrderProductVariants)
            {
                //we can ship only shippable products
                if (!opv.ProductVariant.IsShipEnabled)
                    continue;

                var totalNumberOfItemsCanBeAddedToShipment = opv.GetTotalNumberOfItemsCanBeAddedToShipment();
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

            foreach (var opv in order.OrderProductVariants)
            {
                //we can ship only shippable products
                if (!opv.ProductVariant.IsShipEnabled)
                    continue;

                var totalNumberOfNotYetShippedItems = opv.GetTotalNumberOfNotYetShippedItems();
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

            foreach (var opv in order.OrderProductVariants)
            {
                //we can ship only shippable products
                if (!opv.ProductVariant.IsShipEnabled)
                    continue;

                var totalNumberOfShippedItems = opv.GetTotalNumberOfShippedItems();
                var totalNumberOfDeliveredItems = opv.GetTotalNumberOfDeliveredItems();
                if (totalNumberOfShippedItems <= totalNumberOfDeliveredItems)
                    continue;

                //yes, we have at least one item to deliver
                return true;
            }
            return false;
        }
    }
}
