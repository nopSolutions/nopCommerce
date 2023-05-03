using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.GA4.Models
{
    public class ConfigModel
    {
        [Required]
        [NopResourceDisplayName(GA4Locales.GoogleTag)]
        public string GoogleTag { get; set; }

        [NopResourceDisplayName(GA4Locales.IsDebugMode)]
        public bool IsDebugMode { get; set; }
    }
}
