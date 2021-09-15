using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    /// <summary>
    /// Represents an online customer list model
    /// </summary>
    public partial record OnlineCustomerListModel : BasePagedListModel<OnlineCustomerModel>
    {
    }
}