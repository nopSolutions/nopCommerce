using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.AbcExportOrder.Models
{
    public class ConfigModel
    {
        [Required]
        [NopResourceDisplayName(ExportOrderLocaleKeys.OrderIdPrefix)]
        public string OrderIdPrefix { get; set; }

        [NopResourceDisplayName(ExportOrderLocaleKeys.TablePrefix)]
        public string TablePrefix { get; set; }

        [NopResourceDisplayName(ExportOrderLocaleKeys.FailureAlertEmail)]
        public string FailureAlertEmail { get; set; }
    }
}
