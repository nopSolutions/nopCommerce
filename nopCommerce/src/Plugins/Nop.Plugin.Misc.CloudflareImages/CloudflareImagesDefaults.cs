namespace Nop.Plugin.Misc.CloudflareImages;

/// <summary>
/// Represents plugin constants
/// </summary>
public static class CloudflareImagesDefaults
{
    /// <summary>
    /// Gets a plugin system name
    /// </summary>
    public static string SystemName => "Misc.CloudflareImages";

    /// <summary>
    /// Gets a period (in seconds) before the request times out
    /// </summary>
    public static int RequestTimeout => 30;

    /// <summary>
    /// Gets the base Cloudflare Images API URL
    /// </summary>
    public static string BaseApiUrl => "https://api.cloudflare.com/client/v4/accounts/{0}/images/v1{1}";

    /// <summary>
    /// Gets a image id pattern
    /// </summary>
    public static string ImageIdPattern => "<image_id>";

    /// <summary>
    /// Gets a variant name pattern
    /// </summary>
    public static string VariantNamePattern => "<variant_name>";
}