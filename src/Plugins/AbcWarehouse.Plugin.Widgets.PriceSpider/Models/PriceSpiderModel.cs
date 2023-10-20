using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace AbcWarehouse.Plugin.Widgets.PriceSpider.Models
{
    public class PriceSpiderModel
    {
        public string MerchantId { get; set; }
        public IList<PriceSpiderProductModel> Products { get; set; }
    }
}
