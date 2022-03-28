using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.EnhancedLogging.Models
{
    public class EnhancedLoggingConfigModel
    {
        [Required]
        [NopResourceDisplayName(EnhancedLoggingPlugin.LocaleKey.DaysToKeepLogs)]
        public int DaysToKeepLogs { get; set; }
    }
}
