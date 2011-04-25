using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class ProductVariantValidator : AbstractValidator<ProductVariantModel>
    {
        public ProductVariantValidator(ILocalizationService localizationService)
        {
            //TODO add required rules
        }
    }
}