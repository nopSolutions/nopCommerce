using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class ManufacturerListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Catalog.Manufacturers.List.SearchManufacturerName")]
        [AllowHtml]
        public string SearchManufacturerName { get; set; }

        public GridModel<ManufacturerModel> Manufacturers { get; set; }
    }
}