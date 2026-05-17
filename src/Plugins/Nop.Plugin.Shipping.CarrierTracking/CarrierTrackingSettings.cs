using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.CarrierTracking;

public class CarrierTrackingSettings : ISettings
{
    public string WireMockBaseUrl { get; set; } = "http://wiremock:8080";
    public int WireMockTimeoutMs { get; set; } = 3000;
    public string CarrierCode { get; set; } = "WIREMOCK";
}
