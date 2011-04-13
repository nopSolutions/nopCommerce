
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models
{
    public class CustomerAddressModel : BaseNopModel
    {
        public int CustomerId { get; set; }
        public AddressModel Address { get; set; }
    }
}