namespace Nop.Plugin.Widgets.FacebookPixel.Models;

/// <summary>
/// Represents a configuration model
/// </summary>
public class ConfigurationModel
{
    #region Ctor

    public ConfigurationModel()
    {
        FacebookPixelSearchModel = new FacebookPixelSearchModel();
    }

    #endregion

    #region Properties

    public bool HideList { get; set; }

    public FacebookPixelSearchModel FacebookPixelSearchModel { get; set; }

    #endregion
}