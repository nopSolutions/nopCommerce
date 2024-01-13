namespace Nop.Services.Media.RoxyFileman;

/// <summary>
/// Represents default values related to roxyFileman
/// </summary>
public static partial class NopRoxyFilemanDefaults
{
    /// <summary>
    /// Default path to root directory of uploaded files (if appropriate settings are not specified)
    /// </summary>
    public static string DefaultRootDirectory { get; } = "/images/uploaded";

    /// <summary>
    /// Path to configuration file
    /// </summary>
    public static string ConfigurationFile { get; } = "/lib/Roxy_Fileman/conf.json";

    /// <summary>
    /// Path to directory of language files
    /// </summary>
    public static string LanguageDirectory { get; } = "/lib/Roxy_Fileman/lang";

    /// <summary>
    /// Array of file types that aren't allowed to be uploaded
    /// </summary>
    public static string[] ForbiddenUploadExtensions { get; } =
    [
    "zip", "js", "jsp", "jsb", "mhtml", "mht", "xhtml", "xht", "php", "phtml", "php3", "php4", "php5", "phps", "shtml", "jhtml",
    "pl", "sh", "py", "cgi", "exe", "application", "gadget", "hta", "cpl", "msc", "jar", "vb", "jse", "ws", "wsf", "wsc", "wsh",
    "ps1", "ps2", "psc1", "psc2", "msh", "msh1", "msh2", "inf", "reg", "scf", "msp", "scr", "dll", "msi", "vbs", "bat", "com",
    "pif", "cmd", "vxd", "cpl", "htpasswd", "htaccess"
    ];
}