using Nop.Core.Caching;

namespace Nop.Services.Stores;

/// <summary>
/// Represents default values related to stores services
/// </summary>
public static partial class NopStoreDefaults
{
    #region Caching defaults

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : entity ID
    /// {1} : entity name
    /// </remarks>
    public static CacheKey StoreMappingIdsCacheKey => new("Nop.storemapping.ids.{0}-{1}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : entity ID
    /// {1} : entity name
    /// </remarks>
    public static CacheKey StoreMappingsCacheKey => new("Nop.storemapping.{0}-{1}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : entity name
    /// </remarks>
    public static CacheKey StoreMappingExistsCacheKey => new("Nop.storemapping.exists.{0}");

    #endregion
}