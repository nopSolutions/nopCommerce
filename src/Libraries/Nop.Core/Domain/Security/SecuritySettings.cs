using Nop.Core.Configuration;

namespace Nop.Core.Domain.Security;

/// <summary>
/// Security settings
/// </summary>
public partial class SecuritySettings : ISettings
{
    /// <summary>
    /// Gets or sets an encryption key
    /// </summary>
    public string EncryptionKey { get; set; }

    /// <summary>
    /// Gets or sets a list of admin area allowed IP addresses
    /// </summary>
    public List<string> AdminAreaAllowedIpAddresses { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether honeypot is enabled on the registration page
    /// </summary>
    public bool HoneypotEnabled { get; set; }

    /// <summary>
    /// Gets or sets a honeypot input name
    /// </summary>
    public string HoneypotInputName { get; set; }

    // <summary>
    /// Gets or sets a value indicating whether Honeypot events should be logged
    /// </summary>
    public bool LogHoneypotDetection { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to allow non-ASCII characters in headers
    /// </summary>
    public bool AllowNonAsciiCharactersInHeaders { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Advanced Encryption Standard (AES) is used
    /// </summary>
    public bool UseAesEncryptionAlgorithm { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to allow export and import customers with hashed password
    /// </summary>
    public bool AllowStoreOwnerExportImportCustomersWithHashedPassword { get; set; }
}