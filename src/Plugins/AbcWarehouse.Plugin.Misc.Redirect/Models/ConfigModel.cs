using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using AbcWarehouse.Plugin.Misc.Redirect;

namespace AbcWarehouse.Plugin.Misc.Redirect.Models
{
    public class ConfigModel
    {
        [NopResourceDisplayName(RedirectLocales.IsDebugMode)]
        public bool IsDebugMode { get; set; }
    }
}
