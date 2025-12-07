using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Theme.KungFu.Models;

public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.Theme.KungFu.Fields.LastSyncedOn")]
    public string LastSyncedOn { get; set; }

    public bool IsOutdated { get; set; }

    [NopResourceDisplayName("Plugins.Theme.KungFu.Fields.SyncAutomatically")]
    public bool SyncAutomatically { get; set; }

    public string PluginVersion { get; set; }

    public string AccentSummary { get; set; }

    public bool TriggerResync { get; set; }
}
