using System.Linq;
using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Catalog
{
    public class CategoryValidator : BaseNopValidator<CategoryModel>
    {
        public CategoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Categories.Fields.Name.Required"));

            RuleFor(x => x.PageSizeOptions).Must(UniqueOptionValidator).WithMessage("Admin.Catalog.Categories.Fields.PageSizeOptions.ShouldHaveUniqueItems");
        }

        private bool UniqueOptionValidator(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }
            var notValid = input.Split(',').Select(p => p.Trim()).GroupBy(p => p).Any(p => p.Count() > 1);
            return !notValid;
        }
    }
}