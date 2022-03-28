using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.AbcPromos.Models
{
    public class ConfigModel
    {
        [NopResourceDisplayName(AbcPromosLocales.IncludeExpiredPromosOnRebatesPromosPage)]
        public bool IncludeExpiredPromosOnRebatesPromosPage { get; set; }
    }
}
