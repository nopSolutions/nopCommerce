using Nop.Core.Configuration;

namespace Nop.Plugin.Theme.KungFu;

public class ThemeKungFuSettings : ISettings
{
    public bool SyncAutomatically { get; set; }

    public DateTime? LastSyncedOnUtc { get; set; }

    public string LastSyncedVersion { get; set; }
}
