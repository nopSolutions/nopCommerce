namespace Nop.Core.Domain.Security;

/// <summary>
/// Security setting changed event
/// </summary>
public partial class SecuritySettingsChangedEvent
{
    #region Ctor

    /// <summary>
    /// Initialize a new instance of the SecuritySettingsChangedEvent
    /// </summary>
    /// <param name="SecuritySettings">Security settings</param>
    /// <param name="oldEncryptionPrivateKey">Previous encryption key value</param>
    public SecuritySettingsChangedEvent(SecuritySettings securitySettings, string oldEncryptionPrivateKey)
    {
        SecuritySettings = securitySettings;
        OldEncryptionPrivateKey = oldEncryptionPrivateKey;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Security settings
    /// </summary>
    public SecuritySettings SecuritySettings { get; set; }

    /// <summary>
    /// Previous encryption key value
    /// </summary>
    public string OldEncryptionPrivateKey { get; set; }

    #endregion
}