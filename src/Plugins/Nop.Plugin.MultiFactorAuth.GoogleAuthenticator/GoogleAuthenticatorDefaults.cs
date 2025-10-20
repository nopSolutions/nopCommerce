﻿using Nop.Core.Caching;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator;

/// <summary>
/// Represents plugin constants
/// </summary>
public class GoogleAuthenticatorDefaults
{
    /// <summary>
    /// Gets a plugin system name
    /// </summary>
    public static string SystemName => "MultiFactorAuth.GoogleAuthenticator";

    /// <summary>
    /// Gets the configuration route name
    /// </summary>
    public static string ConfigurationRouteName => "Plugin.MultiFactorAuth.GoogleAuthenticator.Configure";

    /// <summary>
    /// Gets a default QRPixelsPerModule
    /// </summary>
    public static int DefaultQRPixelsPerModule => 3;

    #region Caching

    /// <summary>
    /// Gets the cache key for configuration
    /// </summary>
    /// <remarks>
    /// {0} : configuration identifier
    /// </remarks>
    public static CacheKey ConfigurationCacheKey => new("Nop.PluginMultiFactorAuth.GoogleAuthenticator.Configuration-{0}");

    /// <summary>
    /// Gets the prefix key to clear cache
    /// </summary>
    public static string PrefixCacheKey => "Nop.Plugin.MultiFactorAuth.GoogleAuthenticator";

    /// <summary>
    /// Gets the generic attribute name to hide search block on the plugin configuration page
    /// </summary>
    public static string HideSearchBlockAttribute => "GoogleAuthenticator.HideSearchBlock";

    #endregion
}