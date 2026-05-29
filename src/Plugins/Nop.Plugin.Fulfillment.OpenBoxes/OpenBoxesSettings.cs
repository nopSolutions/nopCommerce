using Nop.Core.Configuration;

namespace Nop.Plugin.Fulfillment.OpenBoxes;

public class OpenBoxesSettings : ISettings
{
    public string OpenBoxesBaseUrl { get; set; } = "http://openboxes:8080/openboxes/";
    public string OpenBoxesUsername { get; set; } = "admin";
    public string OpenBoxesPassword { get; set; } = "password";
    public string OpenBoxesOriginLocationId { get; set; } = "1";
    public int PollerIntervalSeconds { get; set; } = 30;
    public int PollerBatchSize { get; set; } = 50;
}
