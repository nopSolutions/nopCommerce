using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Tax
{
    /// <summary>
    /// Represents a tax category list model
    /// </summary>
    public partial record TaxCategoryListModel : BasePagedListModel<TaxCategoryModel>
    {
    }
}