using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a filter level model
/// </summary>
public partial record FilterLevelModel : BaseNopEntityModel, ILocalizedModel<FilterLevelLocalizedModel>
{
    #region Ctor

    public FilterLevelModel()
    {
        Locales = new List<FilterLevelLocalizedModel>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Settings.FilterLevel.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.FilterLevel.Enabled")]
    public bool Enabled { get; set; }

    public IList<FilterLevelLocalizedModel> Locales { get; set; }

    #endregion
}
