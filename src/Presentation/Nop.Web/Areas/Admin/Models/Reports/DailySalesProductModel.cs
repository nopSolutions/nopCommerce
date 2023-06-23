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
       
        public int Id { get; set; } 
        public int OrderId { get; set; } 
        public int ProductId { get; set; } 
        public decimal OriginalProductCost { get; set; } 
        public int VendorId { get; set; } 
        public string Name { get; set; } 
        public decimal Price { get; set; } 
        public decimal PriceDifference { get; set; }
        public  DateTime Date { get; set; }
        public string FormattedDate => Date.ToString("MM/dd/yyyy");

    }
}

