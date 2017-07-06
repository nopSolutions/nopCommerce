using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public partial class OrderAddressModel : BaseNopModel
    {
        public int OrderId { get; set; }

        public AddressModel Address { get; set; }
    }
}