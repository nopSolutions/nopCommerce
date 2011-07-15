using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class ProductsByTagModel : BaseNopEntityModel
    {
        public ProductsByTagModel()
        {
            Products = new List<ProductModel>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
            AvailableSortOptions = new List<SelectListItem>();
            AvailableViewModes = new List<SelectListItem>();
        }

        public string TagName { get; set; }

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }
        public bool AllowProductFiltering { get; set; }
        public IList<SelectListItem> AvailableSortOptions { get; set; }
        public bool AllowProductViewModeChanging { get; set; }
        public IList<SelectListItem> AvailableViewModes { get; set; }
        
        public IList<ProductModel> Products { get; set; }
    }
}