using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class CopyProductModel : BaseNopEntityModel
    {

        [NopResourceDisplayName("Admin.Catalog.Products.Copy.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Copy.CopyImages")]
        public bool CopyImages { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Copy.Published")]
        public bool Published { get; set; }

    }
}