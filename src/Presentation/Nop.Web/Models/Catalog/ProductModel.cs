using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models
{
    public class ProductModel : BaseNopEntityModel
    {
        public ProductModel()
        {
            ProductPrice = new ProductPriceModel();
        }

        #region Properties 

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public ProductPriceModel ProductPrice { get; set; }

		#endregion Properties 

		#region Nested Classes 

        public class ProductPriceModel
        {
            public bool CustomerEntersPrice { get; set; }
            public bool CallForPrice { get; set; }
            public bool HasMultipleVariants { get; set; }
            public decimal? Price {get;set;}
            public decimal? OldPrice {get;set;}
        }

		#endregion Nested Classes 
    }
}