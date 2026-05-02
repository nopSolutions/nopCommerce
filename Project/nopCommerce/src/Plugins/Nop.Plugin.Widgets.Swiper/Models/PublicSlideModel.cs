using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.Swiper.Models;

/// <summary>
/// Represents a slide model on the site
/// </summary>
public record PublicSlideModel : BaseNopModel
{
    #region Properties

    public int PictureId { get; set; }
    public string PictureUrl { get; set; }
    public string TitleText { get; set; }
    public string LinkUrl { get; set; }
    public string AltText { get; set; }
    public bool LazyLoading { get; set; }

    #endregion
}
