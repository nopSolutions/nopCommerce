using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Swiper.Models;

/// <summary>
/// Represents a configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.Swiper.ShowNavigation")]
    public bool ShowNavigation { get; set; }
    public bool ShowNavigation_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.Swiper.ShowPagination")]
    public bool ShowPagination { get; set; }
    public bool ShowPagination_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.Swiper.Autoplay")]
    public bool Autoplay { get; set; }
    public bool Autoplay_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.Swiper.AutoplayDelay")]
    public int AutoplayDelay { get; set; }
    public bool AutoplayDelay_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.Swiper.LazyLoading")]
    public bool LazyLoading { get; set; }
    public bool LazyLoading_OverrideForStore { get; set; }

    public SlidesSearchModel SlidesSearchModel { get; set; } = new();
    public SlidePictureModel AddSlideModel { get; set; } = new();

    #endregion
}