
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models
{
    public class OrderAddressModel : BaseNopModel
    {
        public int OrderId { get; set; }

        public AddressModel Address { get; set; }
    }
}