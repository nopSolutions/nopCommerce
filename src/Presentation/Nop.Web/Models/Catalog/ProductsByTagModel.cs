using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record ProductsByTagModel : BaseNopEntityModel
{
    public ProductsByTagModel()
    {
        CatalogProductsModel = new CatalogProductsModel();
    }

    public string TagName { get; set; }
    public string TagSeName { get; set; }

    public CatalogProductsModel CatalogProductsModel { get; set; }
}