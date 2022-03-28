using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Mattresses.Domain
{
    public class AbcMattressBase : BaseEntity
    {
        public string ItemNo { get; set; }
        public string Name { get; set; }
        public bool IsAdjustable { get; set; }

        public ProductAttributeValue ToProductAttributeValue(
            int productAttributeMappingId,
            decimal packagePrice,
            decimal mattressSizePrice
        )
        {
            return new ProductAttributeValue()
            {
                ProductAttributeMappingId = productAttributeMappingId,
                Name = Name,
                PriceAdjustment = packagePrice - mattressSizePrice
            };
        }
    }
}