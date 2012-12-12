using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class CopyProductVariantModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.Products.Variants.Copy.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Variants.Copy.Published")]
        public bool Published { get; set; }

    }
}