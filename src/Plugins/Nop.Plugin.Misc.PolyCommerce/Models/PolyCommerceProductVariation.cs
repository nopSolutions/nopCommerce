using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Misc.PolyCommerce.Models
{
    public class PolyCommerceProductVariation
    {
        public long ExternalProductId { get; set; }
        public int InventoryLevel { get; set; }
        public decimal? Price { get; set; }
        public decimal? CostPrice { get; set; }
        public string Sku { get; set; }
        public string Mpn { get; set; }
        public string Gtin { get; set; }
        public string Images { get; set; }
        public List<PolyCommerceProductVariationOptionValue> ProductVariationOptionValues { get; set; } = new List<PolyCommerceProductVariationOptionValue>();
    }
}
