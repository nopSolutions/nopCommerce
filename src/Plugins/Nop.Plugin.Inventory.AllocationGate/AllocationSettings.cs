using Nop.Core.Configuration;

namespace Nop.Plugin.Inventory.AllocationGate;

public class AllocationSettings : ISettings
{
    public string PosApiKey { get; set; } = "verdemart-pos-key";
    public int WebReservationTtlSeconds { get; set; } = 30;
    public int PosReservationTtlSeconds { get; set; } = 300;
    public int ReleaseTaskBatchSize { get; set; } = 200;
    public int ReleaseTaskIntervalSeconds { get; set; } = 30;
    public string OpenBoxesBaseUrl { get; set; } = "";
    public string OpenBoxesApiKey { get; set; } = "";
    public int PollerIntervalSeconds { get; set; } = 30;
    public int PollerBatchSize { get; set; } = 50;
}
