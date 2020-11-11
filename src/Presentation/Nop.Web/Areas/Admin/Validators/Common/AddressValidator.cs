using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Common
{
    public partial class AddressValidator : BaseNopValidator<AddressModel>
    {
        public AddressValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.FirstName.Required"))
                .When(x => x.FirstNameEnabled && x.FirstNameRequired);
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.LastName.Required"))
                .When(x => x.LastNameEnabled && x.LastNameRequired);
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Email.Required"))
                .When(x => x.EmailEnabled && x.EmailRequired);
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"))
                .When(x => x.EmailEnabled && x.EmailRequired);
            RuleFor(x => x.Company)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Company.Required"))
                .When(x => x.CompanyEnabled && x.CompanyRequired);
            RuleFor(x => x.CountryId)
                .NotNull()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Country.Required"))
                .When(x => x.CountryEnabled && x.CountryRequired);
            RuleFor(x => x.CountryId)
                .NotEqual(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Country.Required"))
                .When(x => x.CountryEnabled && x.CountryRequired);
            RuleFor(x => x.County)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.County.Required"))
                .When(x => x.CountyEnabled && x.CountyRequired);
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.City.Required"))
                .When(x => x.CityEnabled && x.CityRequired);
            RuleFor(x => x.Address1)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Address1.Required"))
                .When(x => x.StreetAddressEnabled && x.StreetAddressRequired);
            RuleFor(x => x.Address2)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Address2.Required"))
                .When(x => x.StreetAddress2Enabled && x.StreetAddress2Required);
            RuleFor(x => x.ZipPostalCode)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.ZipPostalCode.Required"))
                .When(x => x.ZipPostalCodeEnabled && x.ZipPostalCodeRequired);
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.PhoneNumber.Required"))
                .When(x => x.PhoneEnabled && x.PhoneRequired);
            RuleFor(x => x.FaxNumber)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.FaxNumber.Required"))
                .When(x => x.FaxEnabled && x.FaxRequired);

            SetDatabaseValidationRules<Address>(dataProvider);
        }
    }
}