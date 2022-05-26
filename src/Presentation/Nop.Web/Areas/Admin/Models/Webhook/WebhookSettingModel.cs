using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Webhook;

public record WebhookSettingModel : BaseNopModel, ISettingsModel
{
    public int ActiveStoreScopeConfiguration { get; set; }
    
    [NopResourceDisplayName("Admin.Configuration.Settings.Webhooks.Enabled")]
    
    public bool ConfigurationEnabled { get; set; }
    
    public string PlaceOrderEndpointUrl { get; set; }
    
}