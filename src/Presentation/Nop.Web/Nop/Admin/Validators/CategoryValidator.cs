using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
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