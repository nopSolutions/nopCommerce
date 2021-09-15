using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a localization settings model
    /// </summary>
    public partial record LocalizationSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

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

        #endregion
    }
}