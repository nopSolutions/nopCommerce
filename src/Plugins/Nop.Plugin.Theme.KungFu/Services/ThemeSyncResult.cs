using ClosedXML.Excel;

namespace Nop.Plugin.Theme.KungFu.Services;

public class ThemeSyncResult
{
    public DateTime? SyncedOnUtc { get; set; }

    public bool WasOutdated { get; set; }

    public bool Synced { get; set; }

    public string PluginVersion { get; set; }
}
