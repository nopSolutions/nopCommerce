using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents an custom html settings model
/// </summary>
public partial record CustomHtmlSettingsModel : BaseNopModel, ISettingsModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HeaderCustomHtml")]
    public string HeaderCustomHtml { get; set; }
    public bool HeaderCustomHtml_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FooterCustomHtml")]
    public string FooterCustomHtml { get; set; }
    public bool FooterCustomHtml_OverrideForStore { get; set; }

    #endregion
}