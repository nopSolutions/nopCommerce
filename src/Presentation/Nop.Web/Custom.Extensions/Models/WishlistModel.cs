using Nop.Web.Models.Catalog;
using System.Collections.Generic;

namespace Nop.Web.Models.ShoppingCart
{
    public partial record WishlistModel
    {
        public IList<ProductOverviewModel> Products { get; set; }

    }
}
