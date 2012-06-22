using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class ManufacturerNavigationModel : BaseNopModel
    {
        public ManufacturerNavigationModel()
        {
            this.Manufacturers = new List<ManufacturerBriefInfoModel>();
        }

        public IList<ManufacturerBriefInfoModel> Manufacturers { get; set; }

        public int TotalManufacturers { get; set; }
    }

    public class ManufacturerBriefInfoModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }
        
        public bool IsActive { get; set; }
    }
}