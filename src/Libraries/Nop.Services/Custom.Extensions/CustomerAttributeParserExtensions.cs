using Nop.Core.Domain.Attributes;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Attributes
{

    public partial interface IAttributeParser<TAttribute, TAttributeValue>
        where TAttribute : BaseAttribute
        where TAttributeValue : BaseAttributeValue
    {
        Task<IList<CustomerAttributeValue>> ParseCustomerAttributeValuesCustomAsync(string attributesXml);
    }

    public partial class AttributeParser<TAttribute, TAttributeValue> : IAttributeParser<TAttribute, TAttributeValue>
        where TAttribute : BaseAttribute
        where TAttributeValue : BaseAttributeValue
    {
        private ISpecificationAttributeService _specificationAttributeService;
        protected IAttributeParser<CustomerAttribute, CustomerAttributeValue> _attributeParser;

        //protected readonly ILocalizationService _localizationService;

        //public AttributeParser(ISpecificationAttributeService specificationAttributeService,
        //    IAttributeParser<CustomerAttribute, CustomerAttributeValue> attributeParser)
        //{
        //    _specificationAttributeService = specificationAttributeService;
        //    _attributeParser = attributeParser;
        //}

        public virtual async Task<IList<CustomerAttributeValue>> ParseCustomerAttributeValuesCustomAsync(string attributesXml)
        {
            _specificationAttributeService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            _attributeParser = EngineContext.Current.Resolve<IAttributeParser<CustomerAttribute, CustomerAttributeValue>>();

            var values = new List<CustomerAttributeValue>();
            if (string.IsNullOrEmpty(attributesXml))
                return values;

            var attributes = await _attributeParser.ParseAttributesAsync(attributesXml);
            foreach (var attribute in attributes)
            {
                if (!attribute.ShouldHaveValues)
                    continue;

                var valuesStr = _attributeParser.ParseValues(attributesXml, attribute.Id);
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
