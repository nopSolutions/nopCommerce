using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.MVC.Areas.Admin.Models;

namespace Nop.Web.MVC.Areas.Admin.Validators
{
    public class CategoryProductValidator : AbstractValidator<CategoryProductModel>
    {
        #region Constructors (1) 

        public CategoryProductValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage(localizationService.GetResource("test"));
        }

		#endregion Constructors 
    }
}