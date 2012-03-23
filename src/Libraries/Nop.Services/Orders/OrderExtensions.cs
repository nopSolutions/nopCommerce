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
                var sopv = shipment.ShipmentOrderProductVariants
                    .Where(x => x.OrderProductVariantId == opv.Id)
                    .FirstOrDefault();
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
                    .Where(x => x.OrderProductVariantId == opv.Id)
                    .FirstOrDefault();
                if (sopv != null)
                {
                    result += sopv.Quantity;
                }
            }

            return result;
        }
    }
}
