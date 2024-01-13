using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Tax;

/// <summary>
/// Represents a tax category search model
/// </summary>
public partial record TaxCategorySearchModel : BaseSearchModel
{
    #region Ctor

    public TaxCategorySearchModel()
    {
        AddTaxCategory = new TaxCategoryModel();
    }

    #endregion

    #region Properties

    public TaxCategoryModel AddTaxCategory { get; set; }

    #endregion
}