using System.Collections.Generic;

namespace Nop.Web.Models.Api.Catalog
{
    public class ProductOrderSimpleAttribute
    {
        public int ProductAttributeId { get; set; }
        public int ProductAttributeValueId { get; set; }
    }

    public class ProductOrderWithAttributes
    {
        public int ProductId { get; set; }
        public ProductOrderSimpleAttribute[] ProductAttributes { get; set; }
        public int Quantity { get; set; }
    }
    
    public class ProductOrderRequestApiModel
    {
        public string ScheduleDate { get; set; }
        public ProductOrderWithAttributes[] Products { get; set; } 
    }
}