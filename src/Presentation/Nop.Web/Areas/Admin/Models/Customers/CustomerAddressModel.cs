using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    public partial class CustomerAddressModel : BaseNopModel
    {
        public int CustomerId { get; set; }

        public AddressModel Address { get; set; }
    }
}