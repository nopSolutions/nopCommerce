using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class HomePageProductsModel : BaseNopModel
    {
        public HomePageProductsModel()
        {
            Products = new List<ProductModel>();
        }

        public bool UseSmallProductBox { get; set; }

        public IList<ProductModel> Products { get; set; }
    }
}