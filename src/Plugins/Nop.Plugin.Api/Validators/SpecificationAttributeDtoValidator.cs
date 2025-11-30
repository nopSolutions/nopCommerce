using System;
using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using Nop.Plugin.Api.DTO.SpecificationAttributes;
using Nop.Plugin.Api.Helpers;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Api.Validators
{
    [UsedImplicitly]
    public class SpecificationAttributeDtoValidator : BaseDtoValidator<SpecificationAttributeDto>
    {
        public SpecificationAttributeDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary)
            : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            if (HttpMethod == HttpMethod.Post)
            {
                //apply "create" rules
                RuleFor(x => x.Id).Equal(0).WithMessage("id must be zero or null for new records");

                ApplyNameRule();
            }
            else if (HttpMethod == HttpMethod.Put)
            {
                //apply "update" rules
                RuleFor(x => x.Id).GreaterThan(0).WithMessage("invalid id");
                ApplyNameRule();
            }
        }

        private void ApplyNameRule()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("invalid name");
        }
    }
}
