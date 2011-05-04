using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public class ProductModel : BaseNopEntityModel
    {
        public ProductModel()
        {
            ProductPrice = new ProductPriceModel();
            DefaultPictureModel = new PictureModel();
            PictureModels = new List<PictureModel>();
        }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string FullDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string MetaTitle { get; set; }

        public string SeName { get; set; }

        //price
        public ProductPriceModel ProductPrice { get; set; }

        //picture(s)
        public bool DefaultPictureZoomEnabled { get; set; }
        public PictureModel DefaultPictureModel { get; set; }
        public IList<PictureModel> PictureModels { get; set; }

		#region Nested Classes 
        
        public class ProductPriceModel
        {
            public string OldPrice { get; set; }

            public string Price {get;set;}
        }

        public class ProductBreadcrumbModel
        {
            public ProductBreadcrumbModel()
            {
                CategoryBreadcrumb = new List<CategoryModel>();
            }

            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public bool DisplayBreadcrumb { get; set; }
            public IList<CategoryModel> CategoryBreadcrumb { get; set; }
        }

		#endregion
    }
}