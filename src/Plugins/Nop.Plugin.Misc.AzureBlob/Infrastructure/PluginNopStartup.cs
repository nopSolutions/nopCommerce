using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Misc.AzureBlob.Services;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.AzureBlob.Infrastructure;

/// <summary>
/// Represents the object for the configuring services on application startup
/// </summary>
public class PluginNopStartup : INopStartup
{
    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var config = Singleton<AppSettings>.Instance.Get<AzureBlobConfig>();

        if (!config.Enabled)
            return;

        services.AddTransient<IThumbService, AzureThumbService>();

        if (!config.StoreDataProtectionKeys || !DataSettingsManager.IsDatabaseInstalled())
            return;

        const string azureDataProtectionKeyFile = "DataProtectionKeys.xml";

        var blobServiceClient = new BlobServiceClient(config.ConnectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(config.DataProtectionKeysContainerName);
        var blobClient = blobContainerClient.GetBlobClient(azureDataProtectionKeyFile);

        var dataProtectionBuilder = services.AddDataProtection().PersistKeysToAzureBlobStorage(blobClient);

        if (string.IsNullOrEmpty(config.DataProtectionKeysVaultId))
            return;

        var keyIdentifier = config.DataProtectionKeysVaultId;
        var credentialOptions = new DefaultAzureCredentialOptions();
        var tokenCredential = new DefaultAzureCredential(credentialOptions);

        dataProtectionBuilder.ProtectKeysWithAzureKeyVault(new Uri(keyIdentifier), tokenCredential);
    }

    /// <summary>
    /// Configure the using of added middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public void Configure(IApplicationBuilder application)
    {
    }

    /// <summary>
    /// Gets order of this startup configuration implementation
    /// </summary>
    public int Order => 3000;
}