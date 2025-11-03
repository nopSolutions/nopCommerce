using System;
using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTO.SpecificationAttributes;
using Nop.Plugin.Api.Helpers;
using System.Net.Http;

namespace Nop.Plugin.Api.Validators
{
    [UsedImplicitly]
    public class SpecificationAttributeOptionDtoValidator : BaseDtoValidator<SpecificationAttributeOptionDto>
    {
        public SpecificationAttributeOptionDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary)
            : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            if (HttpMethod == HttpMethod.Post)
            {
                //apply "create" rules
                RuleFor(x => x.Id).Equal(0).WithMessage("id must be zero or null for new records");

                ApplyNameRule();
                ApplySpecificationAttributeIdRule();
            }
            else if (HttpMethod == HttpMethod.Put)
            {
                //apply "update" rules
                RuleFor(x => x.Id).GreaterThan(0).WithMessage("invalid id");

                if (RequestJsonDictionary.ContainsKey("name"))
                {
                    ApplyNameRule();
                }

                if (RequestJsonDictionary.ContainsKey("specification_attribute_id"))
                {
                    ApplySpecificationAttributeIdRule();
                }
            }
        }

        private void ApplyNameRule()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("invalid name");
        }

        private void ApplySpecificationAttributeIdRule()
        {
            RuleFor(x => x.SpecificationAttributeId).GreaterThan(0).WithMessage("invalid specification attribute id");
        }
    }
}
