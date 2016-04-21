using FluentValidation;
using Nop.Admin.Models.Tax;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Tax
{
    public partial class TaxCategoryValidator : BaseNopValidator<TaxCategoryModel>
    {
        public TaxCategoryValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Tax.Categories.Fields.Name.Required"));

            SetStringPropertiesMaxLength<TaxCategory>(dbContext);
        }
    }
}