using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models
{
    public class CategoryModel
    {
        public CategoryModel()
        {
            Products = new List<ProductModel>();
            PagingFilteringContext = new PagingFilteringModel();
        }

        public string Name { get; set; }

        public IList<ProductModel> Products { get; set; }

        public PagingFilteringModel PagingFilteringContext { get; set; }
    }
}