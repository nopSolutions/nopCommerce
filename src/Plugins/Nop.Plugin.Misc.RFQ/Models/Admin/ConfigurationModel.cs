using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.Misc.RFQ.Enabled")]
    public bool Enabled { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.ShowCaptchaOnRequestPage")]
    public bool ShowCaptchaOnRequestPage { get; set; }
}