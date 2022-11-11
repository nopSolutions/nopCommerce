using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Stores
{
    /// <summary>
    /// Represents a store list model
    /// </summary>
    public partial record StoreListModel : BasePagedListModel<StoreModel>
    {
    }
}