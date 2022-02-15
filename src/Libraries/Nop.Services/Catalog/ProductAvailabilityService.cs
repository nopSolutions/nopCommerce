using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    public class ProductAvailabilityService : IProductAvailabilityService
    {
        public const string AVAILABLE_ON_WEEKDAY_SPEC_ATTRIBUTE_NAME = "Available On Weekday";
        public const string BUSINESS_DAY_SPEC_OPTION_NAME = "Business Day";
        
        private readonly ISpecificationAttributeService _specificationAttributeService;

        public ProductAvailabilityService(ISpecificationAttributeService specificationAttributeService)
        {
            _specificationAttributeService = specificationAttributeService;
        }

        public async Task<bool> IsProductAvailabilityForDateAsync(Product product, DateTime dateUTC)
        {
            // Get product's spec attributes (essentially option ids)
            var productSpecAttributes =
                await _specificationAttributeService.GetProductSpecificationAttributesAsync(product.Id);
            
            // if there are no attributes - no filtering is applied on the product, it depends only on Published
            if (!productSpecAttributes.Any())
                return product.Published;
            
            // Get "Available On Weekday" spec attribute
            var availableOnWeekday = 
                (await _specificationAttributeService.GetSpecificationAttributesAsync())
                .FirstOrDefault(specAttribute => 
                    string.Equals(specAttribute.Name, AVAILABLE_ON_WEEKDAY_SPEC_ATTRIBUTE_NAME, 
                        StringComparison.OrdinalIgnoreCase));
            
            if (availableOnWeekday == null)
                return product.Published;

            // Get all its options
            var specAttributeOptions = await _specificationAttributeService
                .GetSpecificationAttributeOptionsBySpecificationAttributeAsync(availableOnWeekday.Id);

            var productsWeekdaySpecAttributes =
                productSpecAttributes.Join(specAttributeOptions,
                    psa => psa.SpecificationAttributeOptionId,
                    sao => sao.Id,
                    (psa, sao) => sao);

            foreach (var productWeekdaySpecAttribute in productsWeekdaySpecAttributes)
            {
                if (string.Equals(productWeekdaySpecAttribute.Name, BUSINESS_DAY_SPEC_OPTION_NAME))
                {
                    if (dateUTC.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday)
                        return true;
                }
                else
                {
                    var specDay = Enum.Parse<DayOfWeek>(productWeekdaySpecAttribute.Name);
                    if (specDay == dateUTC.DayOfWeek)
                        return true;
                }
            }

            return product.Published;
        }
    }
}