﻿using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a GDPR settings model
/// </summary>
public partial record GdprSettingsModel : BaseNopModel, ISettingsModel
{
    #region Ctor

    public GdprSettingsModel()
    {
        GdprConsentSearchModel = new GdprConsentSearchModel();
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.GdprEnabled")]
    public bool GdprEnabled { get; set; }
    public bool GdprEnabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.LogPrivacyPolicyConsent")]
    public bool LogPrivacyPolicyConsent { get; set; }
    public bool LogPrivacyPolicyConsent_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.LogNewsletterConsent")]
    public bool LogNewsLetterConsent { get; set; }
    public bool LogNewsLetterConsent_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.LogUserProfileChanges")]
    public bool LogUserProfileChanges { get; set; }
    public bool LogUserProfileChanges_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.DeleteInactiveCustomersAfterMonths")]
    public int DeleteInactiveCustomersAfterMonths { get; set; }
    public bool DeleteInactiveCustomersAfterMonths_OverrideForStore { get; set; }

    public GdprConsentSearchModel GdprConsentSearchModel { get; set; }

    #endregion
}