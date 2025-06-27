using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represent a filter level value overview model
/// </summary>
public partial record FilterLevelValueOverviewModel : BaseNopModel
{
    public FilterLevelValueOverviewModel()
    {
        FilterLevelValues = new List<FilterLevelValueInfoModel>();
    }

    public bool FilterLevel1ValueEnabled { get; set; }
    public bool FilterLevel2ValueEnabled { get; set; }
    public bool FilterLevel3ValueEnabled { get; set; }

    public IList<FilterLevelValueInfoModel> FilterLevelValues { get; set; }

    public int TotalFilterLevelValues { get; set; }

}

public partial record FilterLevelValueInfoModel : BaseNopModel
{
    public string FilterLevel1Value { get; set; }
    public string FilterLevel2Value { get; set; }
    public string FilterLevel3Value { get; set; }
}
