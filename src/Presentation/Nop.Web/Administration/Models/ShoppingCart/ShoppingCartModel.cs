using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.ShoppingCart
{
    public partial class ShoppingCartModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.CurrentCarts.Customer")]
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.CurrentCarts.Customer")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.CurrentCarts.TotalItems")]
        public int TotalItems { get; set; }
    }
}