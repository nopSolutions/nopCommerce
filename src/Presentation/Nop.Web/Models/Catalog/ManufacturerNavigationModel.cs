using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record ManufacturerNavigationModel : BaseNopModel
{
    public ManufacturerNavigationModel()
    {
        Manufacturers = new List<ManufacturerBriefInfoModel>();
    }

    public IList<ManufacturerBriefInfoModel> Manufacturers { get; set; }

    public int TotalManufacturers { get; set; }
}

public partial record ManufacturerBriefInfoModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public string SeName { get; set; }

    public bool IsActive { get; set; }
}