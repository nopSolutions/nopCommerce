using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Customers
{
    
    public partial interface ICustomerAttributeParser
    {
        Task<IList<CustomerAttributeValue>> ParseCustomerAttributeValuesCustomAsync(string attributesXml);
    }

    public partial class CustomerAttributeParser
    {
        private ISpecificationAttributeService _specificationAttributeService;

        public CustomerAttributeParser(ISpecificationAttributeService specificationAttributeService)
        {
            _specificationAttributeService = specificationAttributeService;
        }

        public virtual async Task<IList<CustomerAttributeValue>> ParseCustomerAttributeValuesCustomAsync(string attributesXml)
        {
            _specificationAttributeService = EngineContext.Current.Resolve<ISpecificationAttributeService>();

            var values = new List<CustomerAttributeValue>();
            if (string.IsNullOrEmpty(attributesXml))
                return values;

            var attributes = await ParseCustomerAttributesAsync(attributesXml);
            foreach (var attribute in attributes)
            {
                if (!attribute.ShouldHaveValues())
                    continue;

                var valuesStr = ParseValues(attributesXml, attribute.Id);
                foreach (var valueStr in valuesStr)
                {
                    if (string.IsNullOrEmpty(valueStr))
                        continue;

                    if (!int.TryParse(valueStr, out var id))
                        continue;

                    var attributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(id);
                    if (attributeOption != null)
                    {
                        var valueModel = new CustomerAttributeValue
                        {
                            Id = attributeOption.Id,
                            Name = await _localizationService.GetLocalizedAsync(attributeOption, x => x.Name),
                        };
                        values.Add(valueModel);
                    }
                }
            }

            return values;
        }
    }
}
