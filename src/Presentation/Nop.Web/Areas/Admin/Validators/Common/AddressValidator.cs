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
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.FirstName.Required").Result)
                .When(x => x.FirstNameEnabled && x.FirstNameRequired);
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.LastName.Required").Result)
                .When(x => x.LastNameEnabled && x.LastNameRequired);
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.Email.Required").Result)
                .When(x => x.EmailEnabled && x.EmailRequired);
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(localizationService.GetResourceAsync("Admin.Common.WrongEmail").Result)
                .When(x => x.EmailEnabled && x.EmailRequired);
            RuleFor(x => x.Company)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.Company.Required").Result)
                .When(x => x.CompanyEnabled && x.CompanyRequired);
            RuleFor(x => x.CountryId)
                .NotNull()
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.Country.Required").Result)
                .When(x => x.CountryEnabled && x.CountryRequired);
            RuleFor(x => x.CountryId)
                .NotEqual(0)
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.Country.Required").Result)
                .When(x => x.CountryEnabled && x.CountryRequired);
            RuleFor(x => x.County)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.County.Required").Result)
                .When(x => x.CountyEnabled && x.CountyRequired);
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.City.Required").Result)
                .When(x => x.CityEnabled && x.CityRequired);
            RuleFor(x => x.Address1)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.Address1.Required").Result)
                .When(x => x.StreetAddressEnabled && x.StreetAddressRequired);
            RuleFor(x => x.Address2)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.Address2.Required").Result)
                .When(x => x.StreetAddress2Enabled && x.StreetAddress2Required);
            RuleFor(x => x.ZipPostalCode)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.ZipPostalCode.Required").Result)
                .When(x => x.ZipPostalCodeEnabled && x.ZipPostalCodeRequired);
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.PhoneNumber.Required").Result)
                .When(x => x.PhoneEnabled && x.PhoneRequired);
            RuleFor(x => x.FaxNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Address.Fields.FaxNumber.Required").Result)
                .When(x => x.FaxEnabled && x.FaxRequired);

            SetDatabaseValidationRules<Address>(dataProvider);
        }
    }
}