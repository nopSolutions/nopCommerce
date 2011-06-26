using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class ManufacturerNavigationModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }
        
        public bool IsActive { get; set; }
    }
}