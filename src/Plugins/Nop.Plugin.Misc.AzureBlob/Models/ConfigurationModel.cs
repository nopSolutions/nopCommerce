using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.AzureBlob.Models;

/// <summary>
/// Represents configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    public AzureBlobConfigurationModel AzureBlobConfiguration { get; set; }
}

public record AzureBlobConfigurationModel : BaseNopModel
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

    [NopResourceDisplayName("Nop.Plugin.Misc.AzureBlob.StoreDataProtectionKeys")]
    public bool StoreDataProtectionKeys { get; set; }

    [NopResourceDisplayName("Nop.Plugin.Misc.AzureBlob.DataProtectionKeysContainerName")]
    public string DataProtectionKeysContainerName { get; set; }

    [NopResourceDisplayName("Nop.Plugin.Misc.AzureBlob.DataProtectionKeysVaultId")]
    public string DataProtectionKeysVaultId { get; set; }
}