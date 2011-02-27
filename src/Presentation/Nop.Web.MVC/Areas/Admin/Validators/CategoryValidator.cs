using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.MVC.Areas.Admin.Models;

namespace Nop.Web.MVC.Areas.Admin.Validators
{
    public class CategoryValidator : AbstractValidator<CategoryModel>
    {
		#region Constructors (1) 

        public CategoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Category.Name.Validation"));
        }

		#endregion Constructors 
    }
}