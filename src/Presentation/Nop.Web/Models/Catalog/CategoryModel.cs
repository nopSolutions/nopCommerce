using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models
{
    public class CategoryModel : BaseNopEntityModel
    {
		#region Constructors 

        public CategoryModel()
        {
            Products = new List<ProductModel>();
            PagingFilteringContext = new PagingFilteringModel();
            SubCategories = new List<SubCategoryModel>();
        }

		#endregion Constructors 

		#region Properties 

        public string Name { get; set; }

        public PagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ProductModel> Products { get; set; }

        public IList<SubCategoryModel> SubCategories { get; set; }

		#endregion Properties 

		#region Nested Classes 


        public class SubCategoryModel : BaseNopEntityModel
        {
		#region Properties 

            public string ImageUrl { get; set; }

            public string Name { get; set; }

            public string SeName { get; set; }

		#endregion Properties 
        }
		#endregion Nested Classes 
    }
}