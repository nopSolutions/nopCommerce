using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public class NewProductsModel : BaseNopEntityModel
    {
        public NewProductsModel()
        {
            Products = new List<ProductOverviewModel>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
        }

        public IList<ProductOverviewModel> Products { get; set; }
        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }
    }
}
