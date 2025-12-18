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

    [NopResourceDisplayName("Plugins.Theme.KungFu.Fields.AzureOpenAIEndpoint")]
    public string AzureOpenAIEndpoint { get; set; }

    [NopResourceDisplayName("Plugins.Theme.KungFu.Fields.AzureOpenAIKey")]
    public string AzureOpenAIKey { get; set; }

    [NopResourceDisplayName("Plugins.Theme.KungFu.Fields.AzureOpenAIDeploymentName")]
    public string AzureOpenAIDeploymentName { get; set; }

    [NopResourceDisplayName("Plugins.Theme.KungFu.Fields.EnableAISageMessages")]
    public bool EnableAISageMessages { get; set; }
}
