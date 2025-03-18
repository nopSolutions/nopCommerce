﻿using Nop.Core.Caching;

namespace Nop.Services.Messages;

/// <summary>
/// Represents default values related to messages services
/// </summary>
public static partial class NopMessageDefaults
{
    /// <summary>
    /// Gets a key for notifications list from TempDataDictionary
    /// </summary>
    public static string NotificationListKey => "NotificationList";

    /// <summary>
    /// Gets the path to directory used to store the token response
    /// </summary>
    public static string GmailAuthStorePath => "~/App_Data/Gmail/AuthStore";

    /// <summary>
    /// Gets the scopes requested to access a protected API (Gmail)
    /// </summary>
    public static string[] GmailScopes => ["https://mail.google.com/"];

    /// <summary>
    /// Gets the scopes requested to access a protected API (MSAL)
    /// </summary>
    public static string MSALTenantPattern => "https://login.microsoftonline.com/{0}/v2.0";

    /// <summary>
    /// Gets the scopes requested to access a protected API (MSAL)
    /// </summary>
    public static string[] MSALScopes => ["https://outlook.office365.com/.default"];

    #region Caching defaults

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : store ID
    /// {1} : is active?
    /// </remarks>
    public static CacheKey MessageTemplatesAllCacheKey => new("Nop.messagetemplate.all.{0}-{1}");

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : template name
    /// {1} : store ID
    /// </remarks>
    public static CacheKey MessageTemplatesByNameCacheKey => new("Nop.messagetemplate.byname.{0}-{1}");

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    /// <remarks>
    /// {0} : template name
    /// </remarks>
    public static string MessageTemplatesByNamePrefix => "Nop.messagetemplate.byname.{0}";

    #endregion
}