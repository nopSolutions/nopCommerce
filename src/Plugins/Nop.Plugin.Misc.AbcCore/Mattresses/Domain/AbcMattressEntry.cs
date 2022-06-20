using System;
using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Mattresses.Domain
{
    public class AbcMattressEntry : BaseEntity
    {
        public int AbcMattressModelId { get; set; }
        public string Size { get; set; }
        public string ItemNo { get; set; }
        public decimal OldPrice { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }

        public ProductAttributeValue ToProductAttributeValue(
            int productAttributeMappingId,
            decimal productPrice,
            decimal oldPrice)
        {
            return new ProductAttributeValue()
            {
                ProductAttributeMappingId = productAttributeMappingId,
                Name = Size,
                PriceAdjustment = Price - productPrice,
                IsPreSelected = Size == "Queen",
                DisplayOrder = GetDisplayOrder(Size),
                // Using cost as a weird option to store the old price
                Cost = oldPrice
            };
        }

        private int GetDisplayOrder(string size)
        {
            switch (size)
            {
                case "Twin":
                    return 0;
                case "TwinXL":
                    return 10;
                case "Full":
                    return 20;
                case "Queen":
                    return 30;
                case "Queen-Flexhead":
                    return 31;
                case "King":
                    return 40;
                case "King-Flexhead":
                    return 41;
                case "California King":
                    return 50;
                case "California King-Flexhead":
                    return 51;
                default:
                    throw new ArgumentException("Invalid mattress size provided.");
            }
        }
    }
}