using System;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a low stock product model
    /// </summary>
    public partial record DailySalesProductModel : BaseNopEntityModel
    {
       
        
        //    #region Properties

        //    [NopResourceDisplayName("Admin.Catalog.Products.Fields.Name")]
        //    public string Name { get; set; }

        //    public string Attributes { get; set; }
        
        //[NopResourceDisplayName("VendorID")]

        //public int VendorID { get; set; }

        //[NopResourceDisplayName("CostPrice")]

        //public decimal CostPrice { get; set; }


        //[NopResourceDisplayName("SellingPrice")]

        //public decimal SellingPrice { get; set; }


        //[NopResourceDisplayName("Price Difference")]

        //public decimal PriceDifference { get; set; }

        //[NopResourceDisplayName("Date")]

        //public DateTime Date { get; set; }



        public int Id { get; set; } 
        public int OrderId { get; set; } 
        public int ProductId { get; set; } 
        public decimal OriginalProductCost { get; set; } 
        public int VendorId { get; set; } 
        public string Name { get; set; } 
        public decimal Price { get; set; } 
        public decimal PriceDifference { get; set; }
        public  DateTime Date { get; set; }



    }
}

