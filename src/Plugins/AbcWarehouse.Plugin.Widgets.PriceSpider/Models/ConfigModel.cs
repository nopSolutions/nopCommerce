using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.PriceSpider.Models
{
    public class ConfigModel
    {
        [NopResourceDisplayName(PriceSpiderLocales.MerchantId)]
        public string MerchantId { get; set; }
    }
}
