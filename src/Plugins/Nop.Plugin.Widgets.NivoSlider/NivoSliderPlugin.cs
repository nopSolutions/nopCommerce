using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.NivoSlider
{
    /// <summary>
    /// Plugin
    /// </summary>
    public class NivoSliderPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly INopFileProvider _fileProvider;

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
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.HomepageTop };
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
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsNivoSlider";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            InstallNivoSliderSettings();
            InstallLocaleResources();

            base.Install();
        }

        /// <summary>
        /// Install Nivo Slider plugin settings
        /// </summary>
        protected virtual void InstallNivoSliderSettings()
        {
            var sampleImagesPath = _fileProvider.MapPath("~/Plugins/Widgets.NivoSlider/Content/nivoslider/sample-images/");

            var storeLocation = _webHelper.GetStoreLocation(false);

            var settings = new NivoSliderSettings
            {
                Picture1Id = InstallPicture(sampleImagesPath, "banner1.jpg", MimeTypes.ImagePJpeg, "banner_1").Id,
                Text1 = "",
                Link1 = storeLocation,
                Picture2Id = InstallPicture(sampleImagesPath, "banner2.jpg", MimeTypes.ImagePJpeg, "banner_2").Id,
                Text2 = "",
                Link2 = storeLocation,
            };
            _settingService.SaveSetting(settings);
        }

        /// <summary>
        /// Install picture
        /// </summary>
        /// <param name="imageFilePath">Image file path</param>
        /// <param name="fileName">Image file name including file extension</param>
        /// <param name="mimeType">Image mime type</param>
        /// <param name="seoFileName">Image seo file description</param>
        /// <returns></returns>
        protected virtual Picture InstallPicture(string imageFilePath, string fileName, string mimeType, string seoFileName)
        {
            var filePath = _fileProvider.Combine(imageFilePath, fileName);
            var image = _fileProvider.ReadAllBytes(filePath);
            var picture = _pictureService.InsertPicture(image, mimeType, seoFileName);
            return picture;
        }

        /// <summary>
        /// Install Nivo Slider plugin locale resources
        /// </summary>
        protected virtual void InstallLocaleResources()
        {
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture1", "Picture 1");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture2", "Picture 2");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture3", "Picture 3");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture4", "Picture 4");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture5", "Picture 5");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture", "Picture");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture.Hint", "Upload picture.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Text", "Comment");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Text.Hint", "Enter comment for picture. Leave empty if you don't want to display any text.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Link", "URL");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Link.Hint", "Enter URL. Leave empty if you don't want this picture to be clickable.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.AltText", "Image alternate text");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.AltText.Hint", "Enter alternate text that will be added to image.");
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            UninstallNivoSliderSettings();
            UninstallLocaleResources();

            base.Uninstall();
        }

        /// <summary>
        /// Uninstall Nivo Slider plugin settings
        /// </summary>
        protected virtual void UninstallNivoSliderSettings()
        {
            _settingService.DeleteSetting<NivoSliderSettings>();
        }

        /// <summary>
        /// Uninstall Nivo Slider plugin locale resources
        /// </summary>
        protected virtual void UninstallLocaleResources()
        {
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture1");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture2");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture3");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture4");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture5");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Text");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Text.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Link");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Link.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.AltText");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.AltText.Hint");
        }
    }
}