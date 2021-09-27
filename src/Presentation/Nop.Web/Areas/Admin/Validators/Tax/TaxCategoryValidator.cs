using FluentValidation;
using Nop.Core.Domain.Tax;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Tax
{
    public partial class TaxCategoryValidator : BaseNopValidator<TaxCategoryModel>
    {
        public TaxCategoryValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Tax.Categories.Fields.Name.Required"));

            SetDatabaseValidationRules<TaxCategory>(mappingEntityAccessor);
        }
    }
}