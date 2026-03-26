using Nop.Core.Configuration;

namespace Nop.Core.Domain;

/// <summary>
/// Store information settings
/// </summary>
public partial class StoreInformationSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether "powered by nopCommerce" text should be displayed.
    /// Please find more info at https://www.nopcommerce.com/nopcommerce-copyright-removal-key
    /// </summary>
    public bool HidePoweredByNopCommerce { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether store is closed
    /// </summary>
    public bool StoreClosed { get; set; }

    /// <summary>
    /// Gets or sets a picture identifier of the logo. If 0, then the default one will be used
    /// </summary>
    public int LogoPictureId { get; set; }

    /// <summary>
    /// Gets or sets a default store theme
    /// </summary>
    public string DefaultStoreTheme { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customers are allowed to select a theme
    /// </summary>
    public bool AllowCustomerToSelectTheme { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we should display warnings about the new EU cookie law
    /// </summary>
    public bool DisplayEuCookieLawWarning { get; set; }

    /// <summary>
    /// Gets or sets a value of Facebook page URL of the site
    /// </summary>
    public string FacebookLink { get; set; }

    /// <summary>
    /// Gets or sets a value of X page URL of the site
    /// </summary>
    public string XLink { get; set; }

    /// <summary>
    /// Gets or sets a value of YouTube channel URL of the site
    /// </summary>
    public string YoutubeLink { get; set; }

    /// <summary>
    /// Gets or sets a value of Instagram account URL of the site
    /// </summary>
    public string InstagramLink { get; set; } 

    /// <summary>
    /// Gets or sets a value of TikTok account URL of the site
    /// </summary>
    public string TikTokLink { get; set; }

    /// <summary>
    /// Gets or sets a value of Snapchat account URL of the site
    /// </summary>
    public string SnapchatLink { get; set; }

    /// <summary>
    /// Gets or sets a value of Pinterest account URL of the site
    /// </summary>
    public string PinterestLink { get; set; }

    /// <summary>
    /// Gets or sets a value of Tumblr account URL of the site
    /// </summary>
    public string TumblrLink { get; set; }

    /// <summary>
    /// Gets or sets a value of Discord account URL of the site
    /// </summary>
    public string DiscordLink { get; set; }
}