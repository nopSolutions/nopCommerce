using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record ProductsByTagModel : BaseNopEntityModel
    {
        public ProductsByTagModel()
        {
            CatalogProductsModel = new CatalogProductsModel();
        }

        public string TagName { get; set; }
        public string TagSeName { get; set; }
        //product tag seo update by Lancelot
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public CatalogProductsModel CatalogProductsModel { get; set; }
    }
}