using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class CategorySimpleModel : BaseNopEntityModel
    {
        public CategorySimpleModel()
        {
            SubCategories = new List<CategorySimpleModel>();
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        /// <summary>
        /// Param for dispaying of products amount
        /// </summary>
        public int? NumberOfProducts { get; set; }

        /// <summary>
        /// Amount of products per category
        /// </summary>
        public int? ProductsCount { get; set; }

        public bool IncludeInTopMenu { get; set; }

        public List<CategorySimpleModel> SubCategories { get; set; }
    }
}