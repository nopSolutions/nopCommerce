using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Misc.PolyCommerce.Models
{
    public class PolyCommerceOrderItem
    {
        public int ExternalProductId { get; set; }
        public decimal UnitPriceInclTax { get; set; }
        public decimal UnitPriceExclTax { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal PriceExclTax { get; set; }
        public int Quantity { get; set; }
        public decimal? ItemWeight { get; set; }
    }
}
