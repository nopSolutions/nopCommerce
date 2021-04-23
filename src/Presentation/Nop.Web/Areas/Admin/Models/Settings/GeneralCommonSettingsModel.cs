using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a general and common settings model
    /// </summary>
    public partial record GeneralCommonSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Ctor

        public GeneralCommonSettingsModel()
        {
            StoreInformationSettings = new StoreInformationSettingsModel();
            SitemapSettings = new SitemapSettingsModel();
            SeoSettings = new SeoSettingsModel();
            SecuritySettings = new SecuritySettingsModel();
            CaptchaSettings = new CaptchaSettingsModel();
            PdfSettings = new PdfSettingsModel();
            LocalizationSettings = new LocalizationSettingsModel();
            DisplayDefaultMenuItemSettings = new DisplayDefaultMenuItemSettingsModel();
            DisplayDefaultFooterItemSettings = new DisplayDefaultFooterItemSettingsModel();
            AdminAreaSettings = new AdminAreaSettingsModel();
            MinificationSettings = new MinificationSettingsModel();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        public StoreInformationSettingsModel StoreInformationSettings { get; set; }

        public SitemapSettingsModel SitemapSettings { get; set; }

        public SeoSettingsModel SeoSettings { get; set; }

        public SecuritySettingsModel SecuritySettings { get; set; }

        public CaptchaSettingsModel CaptchaSettings { get; set; }

        public PdfSettingsModel PdfSettings { get; set; }

        public LocalizationSettingsModel LocalizationSettings { get; set; }

        public DisplayDefaultMenuItemSettingsModel DisplayDefaultMenuItemSettings { get; set; }

        public DisplayDefaultFooterItemSettingsModel DisplayDefaultFooterItemSettings { get; set; }

        public AdminAreaSettingsModel AdminAreaSettings { get; set; }

        public MinificationSettingsModel MinificationSettings { get; set; }

        #endregion
    }
}