using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Swiper;

/// <summary>
/// Represents plugin settings
/// </summary>
public class SwiperSettings : ISettings
{
    #region Properties

    public bool ShowNavigation { get; set; }
    public bool ShowPagination { get; set; }
    public bool Autoplay { get; set; }
    public int AutoplayDelay { get; set; }
    public bool LazyLoading { get; set; }
    public string Slides { get; set; }

    #endregion
}