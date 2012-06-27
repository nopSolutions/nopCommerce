using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class HomePageProductsModel : BaseNopModel
    {
        public HomePageProductsModel()
        {
            Products = new List<ProductOverviewModel>();
        }

        public bool UseSmallProductBox { get; set; }

        public IList<ProductOverviewModel> Products { get; set; }
    }
}