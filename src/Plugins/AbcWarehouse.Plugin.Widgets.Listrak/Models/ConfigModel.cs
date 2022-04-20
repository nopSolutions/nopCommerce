using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.Listrak.Models
{
    public class ConfigModel
    {
        [Required]
        [NopResourceDisplayName(ListrakLocales.MerchantId)]
        public string MerchantId { get; set; }
    }
}
