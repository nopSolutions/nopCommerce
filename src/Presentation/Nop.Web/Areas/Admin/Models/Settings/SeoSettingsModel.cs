using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a SEO settings model
/// </summary>
public partial record SeoSettingsModel : BaseNopModel, ISettingsModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeparator")]
    [NoTrim]
    public string PageTitleSeparator { get; set; }
    public bool PageTitleSeparator_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeoAdjustment")]
    public int PageTitleSeoAdjustment { get; set; }
    public bool PageTitleSeoAdjustment_OverrideForStore { get; set; }
    public SelectList PageTitleSeoAdjustmentValues { get; set; }

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

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TwitterMetaTags")]
    public bool TwitterMetaTags { get; set; }
    public bool TwitterMetaTags_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.OpenGraphMetaTags")]
    public bool OpenGraphMetaTags { get; set; }
    public bool OpenGraphMetaTags_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CustomHeadTags")]
    public string CustomHeadTags { get; set; }
    public bool CustomHeadTags_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.Microdata")]
    public bool MicrodataEnabled { get; set; }
    public bool MicrodataEnabled_OverrideForStore { get; set; }
    #endregion
}