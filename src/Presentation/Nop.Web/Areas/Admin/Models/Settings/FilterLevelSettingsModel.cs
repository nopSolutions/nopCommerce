using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a filter level settings model
/// </summary>
public partial record FilterLevelSettingsModel : BaseNopModel, ISettingsModel
{
    #region Ctor

    public FilterLevelSettingsModel()
    {
        FilterLevelSearchModel = new FilterLevelSearchModel();
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.FilterLevel.FilterLevelEnabled")]
    public bool FilterLevelEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.FilterLevel.DisplayOnHomePage")]
    public bool DisplayOnHomePage { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.FilterLevel.DisplayOnProductDetailsPage")]
    public bool DisplayOnProductDetailsPage { get; set; }

    public FilterLevelSearchModel FilterLevelSearchModel { get; set; }

    #endregion
}
