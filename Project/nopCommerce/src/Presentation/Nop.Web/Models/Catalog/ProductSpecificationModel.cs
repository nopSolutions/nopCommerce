using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a product specification model
/// </summary>
public partial record ProductSpecificationModel : BaseNopModel
{
    #region Properties

    /// <summary>
    /// Gets or sets the grouped specification attribute models
    /// </summary>
    public IList<ProductSpecificationAttributeGroupModel> Groups { get; set; }

    #endregion

    #region Ctor

    public ProductSpecificationModel()
    {
        Groups = new List<ProductSpecificationAttributeGroupModel>();
    }

    #endregion
}