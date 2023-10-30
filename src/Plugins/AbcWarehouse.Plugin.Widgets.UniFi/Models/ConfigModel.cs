using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.UniFi.Models
{
    public class ConfigModel
    {
        [NopResourceDisplayName(UniFiLocales.PartnerId)]
        public string PartnerId { get; set; }

        [NopResourceDisplayName(UniFiLocales.UseIntegration)]
        public bool UseIntegration { get; set; }
    }
}
