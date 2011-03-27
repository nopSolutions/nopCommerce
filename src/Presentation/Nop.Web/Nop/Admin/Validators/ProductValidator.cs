using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class ProductValidator : AbstractValidator<ProductModel>
    {
        #region Constructors 

        public ProductValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Common.Validation.Required"));
        }

		#endregion Constructors 
    }
}