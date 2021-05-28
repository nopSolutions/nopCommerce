using FluentValidation;
using Nop.Core.Domain.Companies;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Companies;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Customers
{
    public partial class CompanyValidator : BaseNopValidator<CompanyModel>
    {
        public CompanyValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Company.Companies.Fields.Name.Required"));
            RuleFor(x => x.AmountLimit).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Company.Companies.Fields.Name.AmountLimit"));
            
            SetDatabaseValidationRules<Company>(dataProvider);
        }
    }
}