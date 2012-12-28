using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class CategoryNavigationModel : BaseNopModel, ICloneable
    {
        public CategoryNavigationModel()
        {
            Categories = new List<CategoryModel>();
        }

        public int CurrentCategoryId { get; set; }
        public List<CategoryModel> Categories { get; set; }

        public object Clone()
        {
            //we use a shallow copy (deep clone is not required here)
            return this.MemberwiseClone();
        }

        public class CategoryModel : BaseNopEntityModel
        {
            public CategoryModel()
            {
                SubCategories = new List<CategoryModel>();
            }

            public string Name { get; set; }

            public string SeName { get; set; }

            public int? NumberOfProducts { get; set; }

            public List<CategoryModel> SubCategories { get; set; }
        }
    }
}