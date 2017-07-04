using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;

namespace Nop.Plugin.Widgets.NivoSlider
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class NivoSliderPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public NivoSliderPlugin(IPictureService pictureService,
            ISettingService settingService, IWebHelper webHelper)
        {
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "home_page_top" };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsNivoSlider/Configure";
        }

        /// <summary>
        /// Gets a view component for displaying plugin in public store
        /// </summary>
        /// <param name="viewComponentName">View component name</param>
        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "WidgetsNivoSlider";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //pictures
            var sampleImagesPath = CommonHelper.MapPath("~/Plugins/Widgets.NivoSlider/Content/nivoslider/sample-images/");
            
            //settings
            var settings = new NivoSliderSettings
            {
                Picture1Id = _pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "banner1.jpg"), MimeTypes.ImagePJpeg, "banner_1").Id,
                Text1 = "",
                Link1 = _webHelper.GetStoreLocation(false),
                Picture2Id = _pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "banner2.jpg"), MimeTypes.ImagePJpeg, "banner_2").Id,
                Text2 = "",
                Link2 = _webHelper.GetStoreLocation(false),
                //Picture3Id = _pictureService.InsertPicture(File.ReadAllBytes(sampleImagesPath + "banner3.jpg"), MimeTypes.ImagePJpeg, "banner_3").Id,
                //Text3 = "",
                //Link3 = _webHelper.GetStoreLocation(false),
            };
            _settingService.SaveSetting(settings);


            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture1", "Picture 1");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture2", "Picture 2");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture3", "Picture 3");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture4", "Picture 4");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture5", "Picture 5");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture", "Picture");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture.Hint", "Upload picture.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Text", "Comment");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Text.Hint", "Enter comment for picture. Leave empty if you don't want to display any text.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Link", "URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.NivoSlider.Link.Hint", "Enter URL. Leave empty if you don't want this picture to be clickable.");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<NivoSliderSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture1");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture2");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture3");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture4");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture5");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Picture.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Text");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Text.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Link");
            this.DeletePluginLocaleResource("Plugins.Widgets.NivoSlider.Link.Hint");

            base.Uninstall();
        }
    }
}