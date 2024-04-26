using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Tax.Avalara.Models.Tax;

/// <summary>
/// Represents a tax category search model
/// </summary>
public record TaxCategorySearchModel : BaseSearchModel
{
    #region Ctor

    public TaxCategorySearchModel()
    {
        AddTaxCategory = new TaxCategoryModel();
        AvailableTypes = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    public TaxCategoryModel AddTaxCategory { get; set; }

    public IList<SelectListItem> AvailableTypes { get; set; }

    #endregion
}