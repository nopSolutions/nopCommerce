using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class CategoryModel : BaseNopEntityModel
    {
        public CategoryModel()
        {
            Products = new List<ProductModel>();
            PagingFilteringContext = new PagingFilteringModel();
            SubCategories = new List<SubCategoryModel>();
        }
        
        public string Name { get; set; }

        public PagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ProductModel> Products { get; set; }

        public IList<SubCategoryModel> SubCategories { get; set; }

		#region Nested Classes 
        
        public class SubCategoryModel : BaseNopEntityModel
        {
            public string ImageUrl { get; set; }

            public string Name { get; set; }

            public string SeName { get; set; }
        }
		#endregion Nested Classes 
    }
}