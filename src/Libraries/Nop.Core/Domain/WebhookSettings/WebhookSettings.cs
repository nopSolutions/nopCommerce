using Nop.Core.Configuration;

namespace Nop.Core.Domain.NewPage;

public class WebhookSettings : ISettings
{
    public bool ConfigurationEnabled { get; set; }
    
    public string PlaceOrderEndpointUrl { get; set; }
}