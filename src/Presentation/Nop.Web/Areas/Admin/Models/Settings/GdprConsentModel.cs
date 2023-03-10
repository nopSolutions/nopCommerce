using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a GDPR consent model
    /// </summary>
    public partial record GdprConsentModel : BaseNopEntityModel, ILocalizedModel<GdprConsentLocalizedModel>
    {
        #region Ctor

        public GdprConsentModel()
        {
            Locales = new List<GdprConsentLocalizedModel>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.Message")]
        public string Message { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.IsRequired")]
        public bool IsRequired { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.RequiredMessage")]
        public string RequiredMessage { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.DisplayDuringRegistration")]
        public bool DisplayDuringRegistration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.DisplayOnCustomerInfoPage")]
        public bool DisplayOnCustomerInfoPage { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<GdprConsentLocalizedModel> Locales { get; set; }

        #endregion
    }
}