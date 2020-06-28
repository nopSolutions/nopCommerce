using System;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.PolyCommerce.Models
{
    public class PolyCommerceProduct
    {
        public int ExternalProductId { get; set; }

        public string Name { get; set; }

        public string Condition { get; set; }

        public string Sku { get; set; }

        public string Gtin { get; set; }

        public string Description { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Width { get; set; }

        public decimal? Depth { get; set; }

        public decimal? Height { get; set; }

        public decimal Price { get; set; }

        public decimal? CostPrice { get; set; }

        public decimal? RetailPrice { get; set; }

        public int? BrandId { get; set; }

        public int? CategoryId { get; set; }

        public int InventoryLevel { get; set; }

        public string Images { get; set; }

        public string Upc { get; set; }

        public string Mpn { get; set; }

        public bool IsDownload { get; set; }

        public int MinInventoryLevel { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<PolyCommerceProductAttribute> ProductAttributes { get; set; } = new List<PolyCommerceProductAttribute>();
        public List<PolyCommerceProductVariation> ProductVariations { get; set; } = new List<PolyCommerceProductVariation>();
    }
}
