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
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }


        public PictureModel PictureModel { get; set; }

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ProductModel> FeaturedProducts { get; set; }
        public IList<ProductModel> Products { get; set; }
    }
}