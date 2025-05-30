using System.Text.Json;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.Swiper.Components;
using Nop.Plugin.Widgets.Swiper.Domain;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Swiper;

/// <summary>
/// Represents swiper widget
/// </summary>
public class SwiperPlugin : BasePlugin, IWidgetPlugin
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IPictureService _pictureService;
    protected readonly ISettingService _settingService;
    protected readonly IWebHelper _webHelper;
    protected readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public SwiperPlugin(ILocalizationService localizationService,
        INopFileProvider fileProvider,
        IPictureService pictureService,
        ISettingService settingService,
        IWebHelper webHelper,
        WidgetSettings widgetSettings)
    {
        _localizationService = localizationService;
        _fileProvider = fileProvider;
        _pictureService = pictureService;
        _settingService = settingService;
        _webHelper = webHelper;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the widget zones
    /// </returns>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.HomepageTop });
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _webHelper.GetStoreLocation() + "Admin/WidgetSwiper/Configure";
    }

    /// <summary>
    /// Gets a name of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component name</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(WidgetSwiperViewComponent);
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //pictures
        var sampleImagesPath = _fileProvider.MapPath("~/Plugins/Widgets.Swiper/Content/sample-images/");

        //settings

        var slides = new List<Slide>
        {
            new()
            {
                PictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "banner_01.webp")), MimeTypes.ImageWebp, "banner_1")).Id,
                TitleText = string.Empty,
                AltText = string.Empty,
                LinkUrl = _webHelper.GetStoreLocation(),
            },
            new()
            {
                PictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "banner_02.webp")), MimeTypes.ImageWebp, "banner_2")).Id,
                TitleText = string.Empty,
                AltText = string.Empty,
                LinkUrl = _webHelper.GetStoreLocation(),
            }
        };

        var settings = new SwiperSettings
        {
            ShowNavigation = false,
            ShowPagination = true,
            Autoplay = true,
            AutoplayDelay = 3000,
            LazyLoading = true,
            Slides = JsonSerializer.Serialize(slides)
        };
        await _settingService.SaveSettingAsync(settings);

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Widgets.Swiper.Slide"] = "Slide",
            ["Plugins.Widgets.Swiper.SlideList"] = "Add new slide",
            ["Plugins.Widgets.Swiper.Slide.Add"] = "Add",
            ["Plugins.Widgets.Swiper.Settings"] = "Settings",
            ["Plugins.Widgets.Swiper.Picture"] = "Picture",
            ["Plugins.Widgets.Swiper.Picture.Hint"] = "Upload picture.",
            ["Plugins.Widgets.Swiper.Picture.Required"] = "Picture is required",
            ["Plugins.Widgets.Swiper.TitleText"] = "Title",
            ["Plugins.Widgets.Swiper.TitleText.Hint"] = "Enter title for picture. Leave empty if you don't want to display any text.",
            ["Plugins.Widgets.Swiper.LinkUrl"] = "URL",
            ["Plugins.Widgets.Swiper.LinkUrl.Hint"] = "Enter URL. Leave empty if you don't want this picture to be clickable.",
            ["Plugins.Widgets.Swiper.AltText"] = "Image alternate text",
            ["Plugins.Widgets.Swiper.AltText.Hint"] = "Enter alternate text that will be added to image.",
            ["Plugins.Widgets.Swiper.Autoplay"] = "Autoplay",
            ["Plugins.Widgets.Swiper.Autoplay.Hint"] = "Check to enable autoplay.",
            ["Plugins.Widgets.Swiper.LazyLoading"] = "Lazy loading",
            ["Plugins.Widgets.Swiper.LazyLoading.Hint"] = "Check to enable lazy loading of pictures.",
            ["Plugins.Widgets.Swiper.AutoplayDelay"] = "Delay",
            ["Plugins.Widgets.Swiper.AutoplayDelay.Hint"] = "Delay between transitions (in ms). If this parameter is not specified, auto play will be disabled.",
            ["Plugins.Widgets.Swiper.ShowNavigation"] = "Show navigation arrows",
            ["Plugins.Widgets.Swiper.ShowNavigation.Hint"] = "Check to display navigation arrows for the slider.",
            ["Plugins.Widgets.Swiper.ShowPagination"] = "Show pagination",
            ["Plugins.Widgets.Swiper.ShowPagination.Hint"] = "Check to display pagination for the slider.",

        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<SwiperSettings>();
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDescriptor.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(PluginDescriptor.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.Swiper");

        await base.UninstallAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => false;

    #endregion
}