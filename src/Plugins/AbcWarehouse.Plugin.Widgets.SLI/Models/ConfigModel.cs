using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.SLI
{
    public class ConfigModel
    {
        [Required]
        [NopResourceDisplayName(SLILocaleKeys.ActionUrl)]
        public string ActionUrl { get; set; }

        [Required]
        [NopResourceDisplayName(SLILocaleKeys.CookieName)]
        public string CookieName { get; set; }
    }
}
