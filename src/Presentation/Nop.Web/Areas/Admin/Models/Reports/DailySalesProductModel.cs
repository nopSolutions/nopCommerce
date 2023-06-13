using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a low stock product model
    /// </summary>
    public partial record DailySalesProductModel : BaseNopEntityModel
    {
       
        
            #region Properties

            [NopResourceDisplayName("Admin.Catalog.Products.Fields.Name")]
            public string Name { get; set; }

            public string Attributes { get; set; }
        
        [NopResourceDisplayName("VendorID")]

        public int VendorID { get; set; }

        [NopResourceDisplayName("CostPrice")]

        public decimal CostPrice { get; set; }



        [NopResourceDisplayName("SellingPrice")]

        public decimal SellingPrice { get; set; }


        [NopResourceDisplayName("Price Difference")]

        public decimal PriceDifference { get; set; }


        #endregion
    }
}

