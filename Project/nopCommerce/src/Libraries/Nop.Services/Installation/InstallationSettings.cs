using System.Globalization;

namespace Nop.Services.Installation;

/// <summary>
/// Installation settings
/// </summary>
public partial class InstallationSettings
{
    /// <summary>
    /// Administration user email
    /// </summary>
    public string AdminEmail { get; set; }

    /// <summary>
    /// Administration user password
    /// </summary>
    public string AdminPassword { get; set; }

    /// <summary>
    /// Language pack info download link
    /// </summary>
    public string LanguagePackDownloadLink { get; set; }

    /// <summary>
    /// Language pack info translation progress
    /// </summary>
    public int LanguagePackProgress { get; set; }

    /// <summary>
    /// Region info
    /// </summary>
    public RegionInfo RegionInfo { get; set; }

    /// <summary>
    /// Culture info
    /// </summary>
    public CultureInfo CultureInfo { get; set; }

    /// <summary>
    /// Indicate if we need to install sample data
    /// </summary>
    public bool InstallSampleData { get; set; }
}