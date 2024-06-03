using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.AbcExportOrder.Models
{
    public class ConfigModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [Required]
        [NopResourceDisplayName(ExportOrderLocaleKeys.OrderIdPrefix)]
        public string OrderIdPrefix { get; set; }
        public bool OrderIdPrefix_OverrideForStore { get; set; }

        [NopResourceDisplayName(ExportOrderLocaleKeys.TablePrefix)]
        public string TablePrefix { get; set; }
        public bool TablePrefix_OverrideForStore { get; set; }

        [NopResourceDisplayName(ExportOrderLocaleKeys.FailureAlertEmail)]
        public string FailureAlertEmail { get; set; }
        public bool FailureAlertEmail_OverrideForStore { get; set; }
    }
}
