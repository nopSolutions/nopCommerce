using Nop.Core.Configuration;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.CourierGuy.Models;

public record ConfigurationModel : BaseNopModel
{
    public bool UseSandbox_OverrideForStore { get; set; }
    public bool BaseUrl_OverrideForStore { get; set; }
    public bool ApiKey_OverrideForStore { get; set; }
    public bool SandBoxApiKey_OverrideForStore { get; set; }
    public bool TrackingUri_OverrideForStore { get; set; }
    public bool RateRequestUri_OverrideForStore { get; set; }
    public bool ShipmentRequestUri_OverrideForStore { get; set; }
    public bool PushoverApiKey_OverrideForStore { get; set; }
    public bool PushoverUserKey_OverrideForStore { get; set; }


    [NopResourceDisplayName("Plugins.Shipping.CourierGuy.Fields.UseSandbox")]
    public bool UseSandbox { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.CourierGuy.Fields.BaseUrl")]
    public Uri BaseUrl { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.CourierGuy.Fields.ApiKey")]
    public string ApiKey { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.CourierGuy.Fields.SandBoxApiKey")]
    public string SandBoxApiKey { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.CourierGuy.Fields.TrackingUri")]
    public Uri TrackingUri { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.CourierGuy.Fields.RateRequestUri")]
    public Uri RateRequestUri { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.CourierGuy.Fields.ShipmentRequestUri")]
    public Uri ShipmentRequestUri { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.CourierGuy.Fields.PushoverApiKey")]
    public string PushoverApiKey { get; set; } = "asz7kc48pkkivvf8uo8drfodeowb37";

    [NopResourceDisplayName("Plugins.Shipping.CourierGuy.Fields.PushoverUserKey")]
    public string PushoverUserKey { get; set; } = "grxswvwt4yi1mqtmkvuxk71pndyt4t";

    public int ActiveStoreScopeConfiguration { get; set; }
}