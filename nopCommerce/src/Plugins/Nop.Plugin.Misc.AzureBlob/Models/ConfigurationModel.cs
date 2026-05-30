using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.AzureBlob.Models;

/// <summary>
/// Represents configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Nop.Plugin.Misc.AzureBlob.Enabled")]
    public bool Enabled { get; set; }

    [NopResourceDisplayName("Nop.Plugin.Misc.AzureBlob.ConnectionString")]
    public string ConnectionString { get; set; }

    [NopResourceDisplayName("Nop.Plugin.Misc.AzureBlob.ContainerName")]
    public string ContainerName { get; set; }

    [NopResourceDisplayName("Nop.Plugin.Misc.AzureBlob.EndPoint")]
    public string EndPoint { get; set; }

    [NopResourceDisplayName("Nop.Plugin.Misc.AzureBlob.AppendContainerName")]
    public bool AppendContainerName { get; set; }
}