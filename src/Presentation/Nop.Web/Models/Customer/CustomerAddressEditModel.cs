using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Customer
{
    public partial class CustomerAddressEditModel : BaseNopModel
    {
        public CustomerAddressEditModel()
        {
            Address = new AddressModel();
        }
        
        public AddressModel Address { get; set; }
    }
}