using Nop.Web.Framework.Models;

namespace Nop.Plugin.Tax.Avalara.Models.Tax
{
    /// <summary>
    /// Represents a tax category list model
    /// </summary>
    public record TaxCategoryListModel : BasePagedListModel<TaxCategoryModel>
    {
    }
}