using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a filtering values search model
/// </summary>
public partial record FilterLevelValueSearchModel : BaseSearchModel
{
    #region Ctor

    public FilterLevelValueSearchModel()
    {
        AvailableFilterLevel1Values = new List<SelectListItem>();
        AvailableFilterLevel2Values = new List<SelectListItem>();
        AvailableFilterLevel3Values = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel1")]
    public string SearchFilterValue1Id { get; set; }
    public bool HideSearchFilterValue1 { get; set; }

    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel2")]
    public string SearchFilterValue2Id { get; set; }
    public bool HideSearchFilterValue2 { get; set; }

    [NopResourceDisplayName("Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.FilterLevel3")]
    public string SearchFilterValue3Id { get; set; }
    public bool HideSearchFilterValue3 { get; set; }
    public IList<SelectListItem> AvailableFilterLevel1Values { get; set; }
    public IList<SelectListItem> AvailableFilterLevel2Values { get; set; }
    public IList<SelectListItem> AvailableFilterLevel3Values { get; set; }

    public int ProductId { get; set; }

    #endregion
}
