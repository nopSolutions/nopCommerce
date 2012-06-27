using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class HomePageBestsellersModel : BaseNopModel
    {
        public HomePageBestsellersModel()
        {
            Products = new List<ProductOverviewModel>();
        }

        public bool UseSmallProductBox { get; set; }

        public IList<ProductOverviewModel> Products { get; set; }
    }
}