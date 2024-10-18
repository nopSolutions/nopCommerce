using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Swiper.Models;

/// <summary>
/// Represents a slide model
/// </summary>
public record SlidePictureModel : BaseNopModel
{
    #region Properties

    [UIHint("Picture")]
    [NopResourceDisplayName("Plugins.Widgets.Swiper.Picture")]
    public int PictureId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.Swiper.TitleText")]
    public string TitleText { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.Swiper.LinkUrl")]
    public string LinkUrl { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.Swiper.AltText")]
    public string AltText { get; set; }

    #endregion
}
