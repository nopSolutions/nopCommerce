using Nop.Web.Models.Catalog;
using System.Collections.Generic;

namespace Nop.Web.Models.ShoppingCart
{
    public partial record WishlistModel
    {
        private IList<ProductOverviewModel> _products;

        public IList<ProductOverviewModel> Products
        {
            get
            {
                if (_products == null)
                {
                    _products = new List<ProductOverviewModel>();
                }
                return _products;
            }
            set
            {
                _products = value;
            }
        }

    }
}
