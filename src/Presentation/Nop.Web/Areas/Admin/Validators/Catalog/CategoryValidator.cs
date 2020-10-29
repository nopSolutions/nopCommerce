using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog
{
    public partial class CategoryValidator : BaseNopValidator<CategoryModel>
    {
        public CategoryValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResourceAsync("Admin.Catalog.Categories.Fields.Name.Required").Result);
            RuleFor(x => x.PageSizeOptions).Must(ValidatorUtilities.PageSizeOptionsValidator).WithMessage(localizationService.GetResourceAsync("Admin.Catalog.Categories.Fields.PageSizeOptions.ShouldHaveUniqueItems").Result);
            RuleFor(x => x.PageSize).Must((x, context) =>
            {
                if (!x.AllowCustomersToSelectPageSize && x.PageSize <= 0)
                    return false;

                return true;
            }).WithMessage(localizationService.GetResourceAsync("Admin.Catalog.Categories.Fields.PageSize.Positive").Result);
            RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation").Result, NopSeoDefaults.SearchEngineNameLength));

            SetDatabaseValidationRules<Category>(dataProvider);
        }
    }
}