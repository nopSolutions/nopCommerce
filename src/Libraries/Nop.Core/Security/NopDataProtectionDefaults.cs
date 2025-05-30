namespace Nop.Core.Security;

/// <summary>
/// Represents default values related to data protection
/// </summary>
public static partial class NopDataProtectionDefaults
{
    /// <summary>
    /// Gets the name of the key path used to store the protection key list to local file system
    /// </summary>
    public static string DataProtectionKeysPath => "~/App_Data/DataProtectionKeys";
}