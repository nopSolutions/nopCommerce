using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.AzureBlob;

/// <summary>
/// Represents Azure Blob storage configuration parameters
/// </summary>
public partial class AzureBlobConfig : IConfig
{
    /// <summary>
    /// Gets a value indicating whether we should use Azure Blob storage
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to store Data Protection Keys in Azure Blob Storage
    /// </summary>
    public bool StoreDataProtectionKeys { get; set; } = false;

    /// <summary>
    /// Gets or sets Azure CacheControl header (e.g. "max-age=3600, public")
    /// </summary>
    /// <remarks>
    /// max-age=[seconds]     — specifies the maximum amount of time that a representation will be considered fresh. Similar to Expires, this directive is relative to the time of the request, rather than absolute. [seconds] is the number of seconds from the time of the request you wish the representation to be fresh for.
    /// s-maxage=[seconds]    — similar to max-age, except that it only applies to shared (e.g., proxy) caches.
    /// public                — marks authenticated responses as cacheable; normally, if HTTP authentication is required, responses are automatically private.
    /// private               — allows caches that are specific to one user (e.g., in a browser) to store the response; shared caches (e.g., in a proxy) may not.
    /// no-cache              — forces caches to submit the request to the origin server for validation before releasing a cached copy, every time. This is useful to assure that authentication is respected (in combination with public), or to maintain rigid freshness, without sacrificing all of the benefits of caching.
    /// no-store              — instructs caches not to keep a copy of the representation under any conditions.
    /// must-revalidate       — tells caches that they must obey any freshness information you give them about a representation. HTTP allows caches to serve stale representations under special conditions; by specifying this header, you’re telling the cache that you want it to strictly follow your rules.
    /// proxy-revalidate      — similar to must-revalidate, except that it only applies to proxy caches.
    /// </remarks>
    public string AzureCacheControlHeader { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets connection string for Azure Blob storage
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets container name for Azure Blob storage
    /// </summary>
    public string ContainerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets end point for Azure Blob storage
    /// </summary>
    public string EndPoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether or the Container Name is appended to the AzureBlobStorageEndPoint when constructing the url
    /// </summary>
    public bool AppendContainerName { get; set; } = true;

    /// <summary>
    /// Gets or sets the Azure container name for storing Data Protection Keys (this container should be separate from the container used for media and should be Private)
    /// </summary>
    public string DataProtectionKeysContainerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Azure key vault ID used to encrypt the Data Protection Keys. (this is optional)
    /// </summary>
    public string DataProtectionKeysVaultId { get; set; } = string.Empty;
}