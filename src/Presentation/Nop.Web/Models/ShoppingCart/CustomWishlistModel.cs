using Nop.Web.Framework.Models;

namespace Nop.Web.Models.ShoppingCart;

public partial record CustomWishlistModel : BaseNopEntityModel
{
    public string Name { get; set; }
}
