namespace Nop.Plugin.Widgets.AccessiBe;

/// <summary>
/// Represents plugin constants
/// </summary>
public class AccessiBeDefaults
{
    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    public static string SystemName => "Widgets.AccessiBe";

    /// <summary>
    /// Gets the configuration route name
    /// </summary>
    public static string ConfigurationRouteName => "Plugin.Widgets.AccessiBe.Configure";

    /// <summary>
    /// Gets the script configuration token
    /// </summary>
    public static string ConfigToken => "{WIDGET_CONFIG}";

    /// <summary>
    /// Gets a list interface languages
    /// </summary>
    public static Dictionary<string, string> SupportedLanuages => new()
    {
        ["English"] = "en",
        ["Español"] = "es",
        ["Français"] = "fr",
        ["Deutsche"] = "de",
        ["Polski"] = "pl",
        ["Italiano"] = "it",
        ["Português"] = "pt",
        ["Nederlands"] = "nl",
        ["Magyar"] = "hu",
        ["Norsk"] = "no",
        ["Slovenščina"] = "sl",
        ["Slovenčina"] = "sk",
        ["Svenska"] = "sv",
        ["Čeština"] = "cs",
        ["Türkçe"] = "tr",
        ["日本語"] = "ja",
        ["台灣"] = "tw",
        ["中文"] = "zh",
        ["עברית"] = "he",
        ["русский"] = "ru",
        ["الإمارات العربية المتحدة"] = "ar",
        ["عربى"] = "ar"
    };
}