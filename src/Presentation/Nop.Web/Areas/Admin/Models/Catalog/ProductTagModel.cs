using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product tag model
/// </summary>
public partial record ProductTagModel : BaseNopEntityModel, ILocalizedModel<ProductTagLocalizedModel>, IMetaTagsSupportedModel
{
    #region Ctor

    public ProductTagModel()
    {
        Locales = new List<ProductTagLocalizedModel>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.ProductTags.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductTags.Fields.ProductCount")]
    public int ProductCount { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductTags.Fields.MetaKeywords")]
    public string MetaKeywords { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductTags.Fields.MetaDescription")]
    public string MetaDescription { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductTags.Fields.MetaTitle")]
    public string MetaTitle { get; set; }

    public IList<ProductTagLocalizedModel> Locales { get; set; }

    public ProductTagProductSearchModel ProductTagProductSearchModel { get; set; } = new();

    #endregion
}

public partial record ProductTagLocalizedModel : ILocalizedLocaleModel, IMetaTagsSupportedModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductTags.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductTags.Fields.MetaKeywords")]
    public string MetaKeywords { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductTags.Fields.MetaDescription")]
    public string MetaDescription { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductTags.Fields.MetaTitle")]
    public string MetaTitle { get; set; }
}