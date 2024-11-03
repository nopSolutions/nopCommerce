namespace Nop.Plugin.Widgets.Swiper.Domain;

/// <summary>
/// Represents a slider item in the settings
/// </summary>
public class Slide
{
    #region Properties

    /// <summary>
    /// Picture identifier
    /// </summary>
    public int PictureId { get; set; }

    /// <summary>
    /// Title attribute for image
    /// </summary>
    public string TitleText { get; set; }

    /// <summary>
    /// Link URL 
    /// </summary>
    public string LinkUrl { get; set; }

    /// <summary>
    /// Image alternate text
    /// </summary>
    public string AltText { get; set; }

    #endregion
}
