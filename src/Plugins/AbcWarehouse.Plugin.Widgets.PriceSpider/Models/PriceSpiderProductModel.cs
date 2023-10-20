using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.PriceSpider.Models
{
    public class PriceSpiderProductModel
    {
        public string Gtin { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string BrandName { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }

        public override string ToString()
        {
            return $"ean={Gtin};cur=USD;pr={Price};qty={Quantity};vendor_name={BrandName};prod_name={ProductName};proprietary_id={Sku};";
        }
    }
}
