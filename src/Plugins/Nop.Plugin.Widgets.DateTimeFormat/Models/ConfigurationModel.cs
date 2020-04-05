using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.DateTimeFormat.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
        
        [NopResourceDisplayName("Plugins.Widgets.DateTimeFormat.FormatString")]
        public string FormatString { get; set; }
        public bool FormatString_OverrideForStore { get; set; }

    }
}