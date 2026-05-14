using Nop.Core.Configuration;

namespace Nop.Plugin.Inventory.AllocationGate;

public class AllocationSettings : ISettings
{
    public string PosApiKey { get; set; } = "verdemart-pos-key";
    public int PosTtlSeconds { get; set; } = 300;
    public int ReleaseTaskIntervalSeconds { get; set; } = 30;
}
