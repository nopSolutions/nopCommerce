using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class HomePageBestsellersModel : BaseNopModel
    {
        public HomePageBestsellersModel()
        {
            Products = new List<ProductModel>();
        }

        public bool UseSmallProductBox { get; set; }

        public IList<ProductModel> Products { get; set; }
    }
}