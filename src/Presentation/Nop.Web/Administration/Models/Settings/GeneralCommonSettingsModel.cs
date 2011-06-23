using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core.Domain.Forums;
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
        }
        public StoreInformationSettingsModel StoreInformationSettings { get; set; }
        public SeoSettingsModel SeoSettings { get; set; }
        public SecuritySettingsModel SecuritySettings { get; set; }
        public PdfSettingsModel PdfSettings { get; set; }
        public LocalizationSettingsModel LocalizationSettings { get; set; }

        #region Nested classes

        public class StoreInformationSettingsModel
        {
            public StoreInformationSettingsModel()
            {
                this.AvailableStoreThemes = new List<SelectListItem>();
            }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.StoreName")]
            public string StoreName { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.StoreUrl")]
            public string StoreUrl { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme")]
            public string DefaultStoreTheme { get; set; }
            public IList<SelectListItem> AvailableStoreThemes { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AllowCustomerToSelectTheme")]
            public bool AllowCustomerToSelectTheme { get; set; }
        }

        public class SeoSettingsModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeparator")]
            public string PageTitleSeparator { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultTitle")]
            public string DefaultTitle { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords")]
            public string DefaultMetaKeywords { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription")]
            public string DefaultMetaDescription { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ConvertNonWesternChars")]
            public bool ConvertNonWesternChars { get; set; }
        }

        public class SecuritySettingsModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EncryptionKey")]
            public string EncryptionKey { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminAreaAllowedIpAddresses")]
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
        #endregion
    }
}