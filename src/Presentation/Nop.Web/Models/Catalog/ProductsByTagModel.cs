using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a Products by tag model
/// </summary>
public partial record ProductsByTagModel : BaseNopEntityModel
{
    #region Properties

    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string TagName { get; set; }
    public string TagSeName { get; set; }
    public CatalogProductsModel CatalogProductsModel { get; set; } = new();

    #endregion
}