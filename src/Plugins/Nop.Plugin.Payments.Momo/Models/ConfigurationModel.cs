using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Momo.Models;

public record ConfigurationModel : BaseNopModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Momo.Fields.SubscriptionKey")]
    public string SubscriptionKey { get; set; }
    public bool SubscriptionKey_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Momo.Fields.ApiUser")]
    public string ApiUser { get; set; }
    public bool ApiUser_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Momo.Fields.ApiKey")]
    public string ApiKey { get; set; }
    public bool ApiKey_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Momo.Fields.CallbackUrl")]
    public string CallbackUrl { get; set; }
    public bool CallbackUrl_OverrideForStore { get; set; }

    public bool UserCreated { get; set; }
}