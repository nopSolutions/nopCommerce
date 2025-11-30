using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTO.SpecificationAttributes;
using Nop.Plugin.Api.Helpers;
using System.Net.Http;

namespace Nop.Plugin.Api.Validators
{
    [UsedImplicitly]
    public class ProductSpecificationAttributeDtoValidator : BaseDtoValidator<ProductSpecificationAttributeDto>
    {
        public ProductSpecificationAttributeDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary)
            : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            if (HttpMethod == HttpMethod.Post)
            {
                //apply "create" rules
                RuleFor(x => x.Id).Equal(0).WithMessage("id must be zero or null for new records");

                ApplyProductIdRule();
                ApplyAttributeTypeIdRule();

                if (RequestJsonDictionary.ContainsKey("specification_attribute_option_id"))
                {
                    ApplySpecificationAttributeOptoinIdRule();
                }
            }
            else if (HttpMethod == HttpMethod.Put)
            {
                //apply "update" rules
                RuleFor(x => x.Id).GreaterThan(0).WithMessage("invalid id");

                if (RequestJsonDictionary.ContainsKey("product_id"))
                {
                    ApplyProductIdRule();
                }

                if (RequestJsonDictionary.ContainsKey("attribute_type_id"))
                {
                    ApplyAttributeTypeIdRule();
                }

                if (RequestJsonDictionary.ContainsKey("specification_attribute_option_id"))
                {
                    ApplySpecificationAttributeOptoinIdRule();
                }
            }
        }

        private void ApplyProductIdRule()
        {
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("invalid product id");
        }

        private void ApplyAttributeTypeIdRule()
        {
            var specificationAttributeTypes = (SpecificationAttributeType[])Enum.GetValues(typeof(SpecificationAttributeType));
            RuleFor(x => x.AttributeTypeId).InclusiveBetween((int)specificationAttributeTypes.First(), (int)specificationAttributeTypes.Last())
                                           .WithMessage("invalid attribute type id");
        }

        private void ApplySpecificationAttributeOptoinIdRule()
        {
            RuleFor(x => x.SpecificationAttributeOptionId).GreaterThan(0).WithMessage("invalid specification attribute option id");
        }
    }
}
