using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class CategoryModel : BaseNopEntityModel
    {
        public CategoryModel()
        {
            FeaturedProducts = new List<ProductModel>();
            Products = new List<ProductModel>();
            PagingFilteringContext = new PagingFilteringModel();
            SubCategories = new List<SubCategoryModel>();
            CategoryBreadcrumb = new List<CategoryModel>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SeName { get; set; }

        public PagingFilteringModel PagingFilteringContext { get; set; }
        public bool AllowProductFiltering { get; set; }
        public SelectList AllowedSortOptions { get; set; }


        public bool DisplayCategoryBreadcrumb { get; set; }
        public IList<CategoryModel> CategoryBreadcrumb { get; set; }



        public IList<SubCategoryModel> SubCategories { get; set; }


        public IList<ProductModel> FeaturedProducts { get; set; }
        public IList<ProductModel> Products { get; set; }
        

		#region Nested Classes
        
        public class SubCategoryModel : BaseNopEntityModel
        {
            public string ImageUrl { get; set; }

            public string Name { get; set; }

            public string SeName { get; set; }
        }

		#endregion
    }
}