using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AO.Services.Orders.Models
{
    public class AOOrderItem
    {
        public int ProductId { get; set; }

        public int OrderItemId { get; set; }

        public string ProductName { get; set; }

        public bool IstakenAside { get; set; }

        public bool IsOrdered { get; set; }

        public int Quantity { get; set; }

        public int ColorAttributeId { get; set; }

        public int SizeAttributeId { get; set; }

        public string Gtin { get; set; }

        public string StockPosition { get; set; }

        public string DeliveryDateStr { get; set; }

    }
}
