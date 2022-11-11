using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Tax
{
    /// <summary>
    /// Represents a tax provider list model
    /// </summary>
    public partial record TaxProviderListModel : BasePagedListModel<TaxProviderModel>
    {
    }
}