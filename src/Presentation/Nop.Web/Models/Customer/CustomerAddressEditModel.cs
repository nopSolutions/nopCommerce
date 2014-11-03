using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Customer
{
    public partial class CustomerAddressEditModel : BaseNopModel
    {
        public CustomerAddressEditModel()
        {
            this.Address = new AddressModel();
        }
        public AddressModel Address { get; set; }
    }
}