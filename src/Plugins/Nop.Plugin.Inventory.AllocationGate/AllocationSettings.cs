using Nop.Core.Configuration;

namespace Nop.Plugin.Inventory.AllocationGate;

public class AllocationSettings : ISettings
{
    public string PosApiKey { get; set; } = "verdemart-pos-key";
    public int WebReservationTtlSeconds { get; set; } = 30;
    public int PosReservationTtlSeconds { get; set; } = 300;
    public int ReleaseTaskBatchSize { get; set; } = 200;
    public int ReleaseTaskIntervalSeconds { get; set; } = 30;
}
