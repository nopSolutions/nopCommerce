using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.AzureBlob;

/// <summary>
/// Represents Azure Blob Storage settings
/// </summary>
public class AzureBlobSettings : ISettings
{
    /// <summary>
    /// Gets a value indicating whether we should use Azure Blob Storage
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets Azure CacheControl header (e.g. "max-age=3600, public")
    /// <list type="bullet">
    /// <item>max-age=[seconds]     — specifies the maximum amount of time that a representation will be considered fresh. Similar to Expires, this directive is relative to the time of the request, rather than absolute. [seconds] is the number of seconds from the time of the request you wish the representation to be fresh for.</item>
    /// <item>s-max-age=[seconds]    — similar to max-age, except that it only applies to shared (e.g., proxy) caches.</item>
    /// <item>public                — marks authenticated responses as cacheable; normally, if HTTP authentication is required, responses are automatically private.</item>
    /// <item>private               — allows caches that are specific to one user (e.g., in a browser) to store the response; shared caches (e.g., in a proxy) may not.</item>
    /// <item>no-cache              — forces caches to submit the request to the origin server for validation before releasing a cached copy, every time. This is useful to assure that authentication is respected (in combination with public), or to maintain rigid freshness, without sacrificing all the benefits of caching.</item>
    /// <item>no-store              — instructs caches not to keep a copy of the representation under any conditions.</item>
    /// <item>must-revalidate       — tells caches that they must obey any freshness information you give them about a representation. HTTP allows caches to serve stale representations under special conditions; by specifying this header, you’re telling the cache that you want it to strictly follow your rules.</item>
    /// <item>proxy-revalidate      — similar to must-revalidate, except that it only applies to proxy caches.</item>
    /// </list>
    /// </summary>
    public string AzureCacheControlHeader { get; set; }

    /// <summary>
    /// Gets or sets connection string for Azure Blob Storage
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets container name for Azure Blob Storage
    /// </summary>
    public string ContainerName { get; set; }

    /// <summary>
    /// Gets or sets end point for Azure Blob Storage
    /// </summary>
    public string EndPoint { get; set; }

    /// <summary>
    /// Gets or sets whether or the Container Name is appended to the AzureBlobStorageEndPoint when constructing the url
    /// </summary>
    public bool AppendContainerName { get; set; }
}