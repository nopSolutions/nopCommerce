using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial record VendorModel : BaseNopEntityModel
    {
        public VendorModel()
        {
            PictureModel = new PictureModel();
            CatalogProductsModel = new CatalogProductsModel();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public bool AllowCustomersToContactVendors { get; set; }

        public PictureModel PictureModel { get; set; }

        public CatalogProductsModel CatalogProductsModel { get; set; }
    }
}