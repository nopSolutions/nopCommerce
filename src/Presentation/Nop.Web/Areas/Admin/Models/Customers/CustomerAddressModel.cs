using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    /// <summary>
    /// Represents a customer address model
    /// </summary>
    public partial record CustomerAddressModel : BaseNopModel
    {
        #region Ctor

        public CustomerAddressModel()
        {
            Address = new AddressModel();
        }

        #endregion

        #region Properties

        public int CustomerId { get; set; }

        public AddressModel Address { get; set; }

        #endregion
    }
}