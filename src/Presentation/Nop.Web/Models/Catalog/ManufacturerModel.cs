using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public class ManufacturerModel : BaseNopEntityModel
    {
        public ManufacturerModel()
        {
            PictureModel = new PictureModel();
            FeaturedProducts = new List<ProductModel>();
            Products = new List<ProductModel>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
            AvailableSortOptions = new List<SelectListItem>();
            AvailableViewModes = new List<SelectListItem>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string MetaTitle { get; set; }

        public string SeName { get; set; }


        public PictureModel PictureModel { get; set; }

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }
        public bool AllowProductFiltering { get; set; }
        public IList<SelectListItem> AvailableSortOptions { get; set; }
        public bool AllowProductViewModeChanging { get; set; }
        public IList<SelectListItem> AvailableViewModes { get; set; }

        public IList<ProductModel> FeaturedProducts { get; set; }
        public IList<ProductModel> Products { get; set; }
    }
}