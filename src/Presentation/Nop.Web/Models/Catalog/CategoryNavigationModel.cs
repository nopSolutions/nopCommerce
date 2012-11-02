using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class CategoryNavigationModel : BaseNopModel
    {
        public CategoryNavigationModel()
        {
            Categories = new List<CategoryModel>();
        }

        public int CurrentCategoryId { get; set; }
        public List<CategoryModel> Categories { get; set; }

        public class CategoryModel : BaseNopEntityModel
        {
            public CategoryModel()
            {
                SubCategories = new List<CategoryModel>();
            }

            public string Name { get; set; }

            public string SeName { get; set; }

            public int NumberOfParentCategories { get; set; }

            public int? NumberOfProducts { get; set; }

            public List<CategoryModel> SubCategories { get; set; }
        }
    }
}