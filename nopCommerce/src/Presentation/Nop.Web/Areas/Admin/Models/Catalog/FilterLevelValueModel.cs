using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a filter level value model
/// </summary>
public partial record FilterLevelValueModel : BaseNopEntityModel
{
    #region Ctor

    public FilterLevelValueModel()
    {
        FilterLevelValueProductSearchModel = new FilterLevelValueProductSearchModel();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel1")]
    public string FilterLevel1Value { get; set; }
    public bool FilterLevel1ValueEnabled { get; set; }
    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel2")]
    public string FilterLevel2Value { get; set; }
    public bool FilterLevel2ValueEnabled { get; set; }
    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel3")]
    public string FilterLevel3Value { get; set; }
    public bool FilterLevel3ValueEnabled { get; set; }

    public FilterLevelValueProductSearchModel FilterLevelValueProductSearchModel { get; set; }

    #endregion
}
