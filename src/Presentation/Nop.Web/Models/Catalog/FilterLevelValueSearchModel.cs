using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a filtering values search model
/// </summary>
public partial record FilterLevelValueSearchModel : BaseNopModel
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

    public string fl1id { get; set; }
    public bool HideSearchFilterValue1 { get; set; }

    public string fl2id { get; set; }
    public bool HideSearchFilterValue2 { get; set; }

    public string fl3id { get; set; }
    public bool HideSearchFilterValue3 { get; set; }
    public IList<SelectListItem> AvailableFilterLevel1Values { get; set; }
    public IList<SelectListItem> AvailableFilterLevel2Values { get; set; }
    public IList<SelectListItem> AvailableFilterLevel3Values { get; set; }

    #endregion
}
