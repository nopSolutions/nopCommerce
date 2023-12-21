using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.NivoSlider.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.NivoSlider;

/// <summary>
/// PLugin
/// </summary>
public class NivoSliderPlugin : BasePlugin, IWidgetPlugin
{
    protected readonly ILocalizationService _localizationService;
    protected readonly IPictureService _pictureService;
    protected readonly ISettingService _settingService;
    protected readonly IWebHelper _webHelper;
    protected readonly INopFileProvider _fileProvider;

    public NivoSliderPlugin(ILocalizationService localizationService,
        IPictureService pictureService,
        ISettingService settingService,
        IWebHelper webHelper,
        INopFileProvider fileProvider)
    {
        _localizationService = localizationService;
        _pictureService = pictureService;
        _settingService = settingService;
        _webHelper = webHelper;
        _fileProvider = fileProvider;
    }

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
        return _webHelper.GetStoreLocation() + "Admin/WidgetsNivoSlider/Configure";
    }

    /// <summary>
    /// Gets a name of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component name</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(WidgetsNivoSliderViewComponent);
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //pictures
        var sampleImagesPath = _fileProvider.MapPath("~/Plugins/Widgets.NivoSlider/Content/nivoslider/sample-images/");

        //settings
        var settings = new NivoSliderSettings
        {
            Picture1Id = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "banner_01.webp")), MimeTypes.ImageWebp, "banner_1")).Id,
            Text1 = "",
            Link1 = _webHelper.GetStoreLocation(),
            Picture2Id = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "banner_02.webp")), MimeTypes.ImageWebp, "banner_2")).Id,
            Text2 = "",
            Link2 = _webHelper.GetStoreLocation()
            //Picture3Id = _pictureService.InsertPicture(File.ReadAllBytes(_fileProvider.Combine(sampleImagesPath,"banner3.jpg")), MimeTypes.ImagePJpeg, "banner_3").Id,
            //Text3 = "",
            //Link3 = _webHelper.GetStoreLocation(),
        };
        await _settingService.SaveSettingAsync(settings);

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Widgets.NivoSlider.Picture1"] = "Picture 1",
            ["Plugins.Widgets.NivoSlider.Picture2"] = "Picture 2",
            ["Plugins.Widgets.NivoSlider.Picture3"] = "Picture 3",
            ["Plugins.Widgets.NivoSlider.Picture4"] = "Picture 4",
            ["Plugins.Widgets.NivoSlider.Picture5"] = "Picture 5",
            ["Plugins.Widgets.NivoSlider.Picture"] = "Picture",
            ["Plugins.Widgets.NivoSlider.Picture.Hint"] = "Upload picture.",
            ["Plugins.Widgets.NivoSlider.Text"] = "Comment",
            ["Plugins.Widgets.NivoSlider.Text.Hint"] = "Enter comment for picture. Leave empty if you don't want to display any text.",
            ["Plugins.Widgets.NivoSlider.Link"] = "URL",
            ["Plugins.Widgets.NivoSlider.Link.Hint"] = "Enter URL. Leave empty if you don't want this picture to be clickable.",
            ["Plugins.Widgets.NivoSlider.AltText"] = "Image alternate text",
            ["Plugins.Widgets.NivoSlider.AltText.Hint"] = "Enter alternate text that will be added to image."
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
        await _settingService.DeleteSettingAsync<NivoSliderSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.NivoSlider");

        await base.UninstallAsync();
    }

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => false;
}