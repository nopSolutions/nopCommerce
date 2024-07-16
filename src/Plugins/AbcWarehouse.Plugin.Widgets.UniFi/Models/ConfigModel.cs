using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.UniFi.Models
{
    public class ConfigModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName(UniFiLocales.IsEnabled)]
        public bool IsEnabled { get; set; }
        public bool IsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName(UniFiLocales.PartnerId)]
        public string PartnerId { get; set; }
        public bool PartnerId_OverrideForStore { get; set; }

        [NopResourceDisplayName(UniFiLocales.UseIntegration)]
        public bool UseIntegration { get; set; }
        public bool UseIntegration_OverrideForStore { get; set; }
    }
}
