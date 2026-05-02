using Nop.Core.Caching;

namespace Nop.Plugin.Misc.AzureBlob;

/// <summary>
/// Represents plugin constants
/// </summary>
public static class AzureBlobDefaults
{
    #region Caching defaults

    /// <summary>
    /// Gets a key to cache whether thumb exists
    /// </summary>
    /// <remarks>
    /// {0} : thumb file name
    /// </remarks>
    public static CacheKey ThumbExistsCacheKey => new("Nop.azure.thumb.exists.{0}");

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    public static string ThumbsExistsPrefix => "Nop.azure.thumb.exists.";

    #endregion
}