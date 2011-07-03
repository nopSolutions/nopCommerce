using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    public class GeneralCommonSettingsModel : BaseNopModel
    {
        public GeneralCommonSettingsModel()
        {
            StoreInformationSettings = new StoreInformationSettingsModel();
            SeoSettings = new SeoSettingsModel();
            SecuritySettings = new SecuritySettingsModel();
            PdfSettings = new PdfSettingsModel();
            LocalizationSettings = new LocalizationSettingsModel();
            GoogleAnalyticsSettings = new GoogleAnalyticsSettingsModel();
        }
        public StoreInformationSettingsModel StoreInformationSettings { get; set; }
        public SeoSettingsModel SeoSettings { get; set; }
        public SecuritySettingsModel SecuritySettings { get; set; }
        public PdfSettingsModel PdfSettings { get; set; }
        public LocalizationSettingsModel LocalizationSettings { get; set; }
        public GoogleAnalyticsSettingsModel GoogleAnalyticsSettings { get; set; }

        #region Nested classes

        public class StoreInformationSettingsModel
        {
            public StoreInformationSettingsModel()
            {
                this.AvailableStoreThemes = new List<SelectListItem>();
            }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.StoreName")]
            [AllowHtml]
            public string StoreName { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.StoreUrl")]
            [AllowHtml]
            public string StoreUrl { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme")]
            [AllowHtml]
            public string DefaultStoreTheme { get; set; }
            public IList<SelectListItem> AvailableStoreThemes { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AllowCustomerToSelectTheme")]
            public bool AllowCustomerToSelectTheme { get; set; }
        }

        public class SeoSettingsModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeparator")]
            [AllowHtml]
            public string PageTitleSeparator { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultTitle")]
            [AllowHtml]
            public string DefaultTitle { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords")]
            [AllowHtml]
            public string DefaultMetaKeywords { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription")]
            [AllowHtml]
            public string DefaultMetaDescription { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ConvertNonWesternChars")]
            public bool ConvertNonWesternChars { get; set; }
        }

        public class SecuritySettingsModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EncryptionKey")]
            [AllowHtml]
            public string EncryptionKey { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminAreaAllowedIpAddresses")]
            [AllowHtml]
            public string AdminAreaAllowedIpAddresses { get; set; }
        }

        public class PdfSettingsModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfEnabled")]
            public bool Enabled { get; set; }
        }

        public class LocalizationSettingsModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseImagesForLanguageSelection")]
            public bool UseImagesForLanguageSelection { get; set; }
        }

        public class GoogleAnalyticsSettingsModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsEnabled")]
            public bool Enabled { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsId")]
            [AllowHtml]
            public string GoogleId { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsJavaScript")]
            [AllowHtml]
            public string JavaScript { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsPlacement")]
            [AllowHtml]
            public string Placement { get; set; }
        }
        #endregion
    }
}