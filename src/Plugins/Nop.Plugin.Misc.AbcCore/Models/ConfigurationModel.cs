using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.AbcCore.Models
{
    public class ConfigurationModel
    {
        [NopResourceDisplayName(CoreLocales.BackendDbConnectionString)]
        public string BackendDbConnectionString { get; set; }

        [NopResourceDisplayName(CoreLocales.AreExternalCallsSkipped)]
        public bool AreExternalCallsSkipped { get; set; }

        [NopResourceDisplayName(CoreLocales.IsDebugMode)]
        public bool IsDebugMode { get; set; }
    }
}
