using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    /// <summary>
    /// Represents a customer address list model
    /// </summary>
    public partial record CustomerAddressListModel : BasePagedListModel<AddressModel>
    {
    }
}