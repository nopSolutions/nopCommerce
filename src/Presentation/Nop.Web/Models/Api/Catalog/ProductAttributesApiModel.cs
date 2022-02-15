using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Web.Models.Api.Catalog
{
    public class ProductAttributeValueApiModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool PriceAdjustmentUsePercentage { get; set; }
        public decimal PriceAdjustment { get; set; }
        public bool IsPreSelected { get; set; }
    }
    
    public class ProductAttributeApiModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRequired { get; set; }
        public AttributeControlType AttributeControlType { get; set; }
        public ProductAttributeValueApiModel[] AttributeValues { get; set; }
    }
    
    public class ProductAttributesApiModel
    {
        public ProductAttributeApiModel[] ProductAttributes { get; set; }
    }
}