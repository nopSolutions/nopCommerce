using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.ShoppingCart
{
    public class TopShoppingCartModel : BaseNopModel
    {
        public int ProductCount { get; set; }
        public bool MiniShoppingCartEnabled { get; set; }
    }
}