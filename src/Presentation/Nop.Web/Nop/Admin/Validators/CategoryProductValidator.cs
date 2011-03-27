using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class CategoryProductValidator : AbstractValidator<CategoryProductModel>
    {
		#region Constructors 

        public CategoryProductValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage(localizationService.GetResource("test"));
        }

		#endregion Constructors 
    }
}