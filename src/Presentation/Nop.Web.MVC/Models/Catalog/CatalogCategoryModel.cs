using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.MVC.Models.Catalog
{
    public class CatalogCategoryModel
    {
        public CatalogCategoryModel()
        {
            Products = new List<CatalogProductModel>();
        }

        public string Name { get; set; }

        public IList<CatalogProductModel> Products { get; set; }
    }
}