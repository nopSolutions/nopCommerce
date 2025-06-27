using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Misc.SearchSpring.Models
{
    public class ConfigurationModel
    {
        [NopResourceDisplayName(SearchSpringLocales.IsDebugMode)]
        public bool IsDebugMode { get; set; }
    }
}
