using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a store information settings model
    /// </summary>
    public partial class StoreInformationSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Ctor

        public StoreInformationSettingsModel()
        {
            AvailableStoreThemes = new List<ThemeModel>();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

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

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SubjectFieldOnContactUsForm")]
        public bool SubjectFieldOnContactUsForm { get; set; }
        public bool SubjectFieldOnContactUsForm_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseSystemEmailForContactUsForm")]
        public bool UseSystemEmailForContactUsForm { get; set; }
        public bool UseSystemEmailForContactUsForm_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PopupForTermsOfServiceLinks")]
        public bool PopupForTermsOfServiceLinks { get; set; }
        public bool PopupForTermsOfServiceLinks_OverrideForStore { get; set; }

        #endregion

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
}