using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Catalog
{
    public class ProductVariantValidator : AbstractValidator<ProductVariantModel>
    {
        public ProductVariantValidator(ILocalizationService localizationService)
        {
        }
    }
}