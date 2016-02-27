using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public class CustomerRoleModel : BaseNopEntityModel
    {
        public string Description { get; set; }
        public bool IsChecked { get; set; }
    }
}
