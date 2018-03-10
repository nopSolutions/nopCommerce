using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Framework.Security.Captcha;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    public partial class GeneralCommonSettingsModel : BaseNopModel
    {
        public GeneralCommonSettingsModel()
        {
            StoreInformationSettings = new StoreInformationSettingsModel();
            SeoSettings = new SeoSettingsModel();
            SecuritySettings = new SecuritySettingsModel();
            CaptchaSettings = new CaptchaSettingsModel();
            PdfSettings = new PdfSettingsModel();
            LocalizationSettings = new LocalizationSettingsModel();
            FullTextSettings = new FullTextSettingsModel();
            DisplayDefaultMenuItemSettings = new DisplayDefaultMenuItemSettingsModel();
            AdminAreaSettings = new AdminAreaSettingsModel();
        }

        public StoreInformationSettingsModel StoreInformationSettings { get; set; }
        public SeoSettingsModel SeoSettings { get; set; }
        public SecuritySettingsModel SecuritySettings { get; set; }
        public CaptchaSettingsModel CaptchaSettings { get; set; }
        public PdfSettingsModel PdfSettings { get; set; }
        public LocalizationSettingsModel LocalizationSettings { get; set; }
        public FullTextSettingsModel FullTextSettings { get; set; }
        public DisplayDefaultMenuItemSettingsModel DisplayDefaultMenuItemSettings { get; set; }
        public AdminAreaSettingsModel AdminAreaSettings { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }

        #region Nested classes

        public partial class StoreInformationSettingsModel : BaseNopModel
        {
            public StoreInformationSettingsModel()
            {
                this.AvailableStoreThemes = new List<ThemeModel>();
            }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.StoreClosed")]
            public bool StoreClosed { get; set; }
            public bool StoreClosed_OverrideForStore { get; set; }
            
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme")]
            public string DefaultStoreTheme { get; set; }
            public bool DefaultStoreTheme_OverrideForStore { get; set; }
            public IList<ThemeModel> AvailableStoreThemes { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AllowCustomerToSelectTheme")]
            public bool AllowCustomerToSelectTheme { get; set; }
            public bool AllowCustomerToSelectTheme_OverrideForStore { get; set; }

            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.Logo")]
            public int LogoPictureId { get; set; }
            public bool LogoPictureId_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayEuCookieLawWarning")]
            public bool DisplayEuCookieLawWarning { get; set; }
            public bool DisplayEuCookieLawWarning_OverrideForStore { get; set; }
            
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FacebookLink")]
            public string FacebookLink { get; set; }
            public bool FacebookLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TwitterLink")]
            public string TwitterLink { get; set; }
            public bool TwitterLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.YoutubeLink")]
            public string YoutubeLink { get; set; }
            public bool YoutubeLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GooglePlusLink")]
            public string GooglePlusLink { get; set; }
            public bool GooglePlusLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SubjectFieldOnContactUsForm")]
            public bool SubjectFieldOnContactUsForm { get; set; }
            public bool SubjectFieldOnContactUsForm_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseSystemEmailForContactUsForm")]
            public bool UseSystemEmailForContactUsForm { get; set; }
            public bool UseSystemEmailForContactUsForm_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PopupForTermsOfServiceLinks")]
            public bool PopupForTermsOfServiceLinks { get; set; }
            public bool PopupForTermsOfServiceLinks_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapEnabled")]
            public bool SitemapEnabled { get; set; }
            public bool SitemapEnabled_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeCategories")]
            public bool SitemapIncludeCategories { get; set; }
            public bool SitemapIncludeCategories_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeManufacturers")]
            public bool SitemapIncludeManufacturers { get; set; }
            public bool SitemapIncludeManufacturers_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeProducts")]
            public bool SitemapIncludeProducts { get; set; }
            public bool SitemapIncludeProducts_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeProductTags")]
            public bool SitemapIncludeProductTags { get; set; }
            public bool SitemapIncludeProductTags_OverrideForStore { get; set; }

            #region Nested classes

            public partial class ThemeModel
            {
                public string SystemName { get; set; }
                public string FriendlyName { get; set; }
                public string PreviewImageUrl { get; set; }
                public string PreviewText { get; set; }
                public bool SupportRtl { get; set; }
                public bool Selected { get; set; }
            }

            #endregion
        }

        public partial class SeoSettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeparator")]
            [NoTrim]
            public string PageTitleSeparator { get; set; }
            public bool PageTitleSeparator_OverrideForStore { get; set; }
            
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeoAdjustment")]
            public int PageTitleSeoAdjustment { get; set; }
            public bool PageTitleSeoAdjustment_OverrideForStore { get; set; }
            public SelectList PageTitleSeoAdjustmentValues { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultTitle")]
            public string DefaultTitle { get; set; }
            public bool DefaultTitle_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords")]
            public string DefaultMetaKeywords { get; set; }
            public bool DefaultMetaKeywords_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription")]
            public string DefaultMetaDescription { get; set; }
            public bool DefaultMetaDescription_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GenerateProductMetaDescription")]
            public bool GenerateProductMetaDescription { get; set; }
            public bool GenerateProductMetaDescription_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ConvertNonWesternChars")]
            public bool ConvertNonWesternChars { get; set; }
            public bool ConvertNonWesternChars_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CanonicalUrlsEnabled")]
            public bool CanonicalUrlsEnabled { get; set; }
            public bool CanonicalUrlsEnabled_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.WwwRequirement")]
            public int WwwRequirement { get; set; }
            public bool WwwRequirement_OverrideForStore { get; set; }
            public SelectList WwwRequirementValues { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableJsBundling")]
            public bool EnableJsBundling { get; set; }
            public bool EnableJsBundling_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableCssBundling")]
            public bool EnableCssBundling { get; set; }
            public bool EnableCssBundling_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TwitterMetaTags")]
            public bool TwitterMetaTags { get; set; }
            public bool TwitterMetaTags_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.OpenGraphMetaTags")]
            public bool OpenGraphMetaTags { get; set; }
            public bool OpenGraphMetaTags_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CustomHeadTags")]
            public string CustomHeadTags { get; set; }
            public bool CustomHeadTags_OverrideForStore { get; set; }
        }

        public partial class SecuritySettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EncryptionKey")]
            public string EncryptionKey { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminAreaAllowedIpAddresses")]
            public string AdminAreaAllowedIpAddresses { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ForceSslForAllPages")]
            public bool ForceSslForAllPages { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForAdminArea")]
            public bool EnableXsrfProtectionForAdminArea { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForPublicStore")]
            public bool EnableXsrfProtectionForPublicStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HoneypotEnabled")]
            public bool HoneypotEnabled { get; set; }
        }

        public partial class CaptchaSettingsModel : BaseNopModel
        {
            public CaptchaSettingsModel()
            {
                this.AvailableReCaptchaVersions = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaEnabled")]
            public bool Enabled { get; set; }
            public bool Enabled_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnLoginPage")]
            public bool ShowOnLoginPage { get; set; }
            public bool ShowOnLoginPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnRegistrationPage")]
            public bool ShowOnRegistrationPage { get; set; }
            public bool ShowOnRegistrationPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnContactUsPage")]
            public bool ShowOnContactUsPage { get; set; }
            public bool ShowOnContactUsPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailWishlistToFriendPage")]
            public bool ShowOnEmailWishlistToFriendPage { get; set; }
            public bool ShowOnEmailWishlistToFriendPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailProductToFriendPage")]
            public bool ShowOnEmailProductToFriendPage { get; set; }
            public bool ShowOnEmailProductToFriendPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnBlogCommentPage")]
            public bool ShowOnBlogCommentPage { get; set; }
            public bool ShowOnBlogCommentPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnNewsCommentPage")]
            public bool ShowOnNewsCommentPage { get; set; }
            public bool ShowOnNewsCommentPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnProductReviewPage")]
            public bool ShowOnProductReviewPage { get; set; }
            public bool ShowOnProductReviewPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnApplyVendorPage")]
            public bool ShowOnApplyVendorPage { get; set; }
            public bool ShowOnApplyVendorPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPublicKey")]
            public string ReCaptchaPublicKey { get; set; }
            public bool ReCaptchaPublicKey_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPrivateKey")]
            public string ReCaptchaPrivateKey { get; set; }
            public bool ReCaptchaPrivateKey_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaVersion")]
            public ReCaptchaVersion ReCaptchaVersion { get; set; }
            public bool ReCaptchaVersion_OverrideForStore { get; set; }

            public IList<SelectListItem> AvailableReCaptchaVersions { get; set; }
        }

        public partial class PdfSettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLetterPageSizeEnabled")]
            public bool LetterPageSizeEnabled { get; set; }
            public bool LetterPageSizeEnabled_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLogo")]
            [UIHint("Picture")]
            public int LogoPictureId { get; set; }
            public bool LogoPictureId_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisablePdfInvoicesForPendingOrders")]
            public bool DisablePdfInvoicesForPendingOrders { get; set; }
            public bool DisablePdfInvoicesForPendingOrders_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn1")]
            public string InvoiceFooterTextColumn1 { get; set; }
            public bool InvoiceFooterTextColumn1_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn2")]
            public string InvoiceFooterTextColumn2 { get; set; }
            public bool InvoiceFooterTextColumn2_OverrideForStore { get; set; }
        }

        public partial class LocalizationSettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseImagesForLanguageSelection")]
            public bool UseImagesForLanguageSelection { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SeoFriendlyUrlsForLanguagesEnabled")]
            public bool SeoFriendlyUrlsForLanguagesEnabled { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AutomaticallyDetectLanguage")]
            public bool AutomaticallyDetectLanguage { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup")]
            public bool LoadAllLocaleRecordsOnStartup { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllLocalizedPropertiesOnStartup")]
            public bool LoadAllLocalizedPropertiesOnStartup { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllUrlRecordsOnStartup")]
            public bool LoadAllUrlRecordsOnStartup { get; set; }
        }

        public partial class FullTextSettingsModel : BaseNopModel
        {
            public bool Supported { get; set; }

            public bool Enabled { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.SearchMode")]
            public int SearchMode { get; set; }
            public SelectList SearchModeValues { get; set; }
        }
        
        public partial class DisplayDefaultMenuItemSettingsModel: BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem")]
            public bool DisplayHomePageMenuItem { get; set; }
            public bool DisplayHomePageMenuItem_OverrideForStore { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem")]
            public bool DisplayNewProductsMenuItem { get; set; }
            public bool DisplayNewProductsMenuItem_OverrideForStore { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem")]
            public bool DisplayProductSearchMenuItem { get; set; }
            public bool DisplayProductSearchMenuItem_OverrideForStore { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem")]
            public bool DisplayCustomerInfoMenuItem { get; set; }
            public bool DisplayCustomerInfoMenuItem_OverrideForStore { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem")]
            public bool DisplayBlogMenuItem { get; set; }
            public bool DisplayBlogMenuItem_OverrideForStore { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem")]
            public bool DisplayForumsMenuItem { get; set; }
            public bool DisplayForumsMenuItem_OverrideForStore { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem")]
            public bool DisplayContactUsMenuItem { get; set; }
            public bool DisplayContactUsMenuItem_OverrideForStore { get; set; }
        }

        public partial class AdminAreaSettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminArea.UseRichEditorInMessageTemplates")]
            public bool UseRichEditorInMessageTemplates { get; set; }
            public bool UseRichEditorInMessageTemplates_OverrideForStore { get; set; }
        }

        #endregion
    }
}