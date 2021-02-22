using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
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
            var addressSettings = EngineContext.Current.Resolve<AddressSettings>();

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.FirstName.Required"));
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.LastName.Required"));
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Email.Required"));
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
            RuleFor(x => x.Company)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Company.Required"))
                .When(_ => addressSettings.CompanyEnabled && addressSettings.CompanyRequired);
            RuleFor(x => x.CountryId)
                .NotNull()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Country.Required"))
                .When(_ => addressSettings.CountryEnabled);
            RuleFor(x => x.CountryId)
                .NotEqual(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Country.Required"))
                .When(_ => addressSettings.CountryEnabled);
            RuleFor(x => x.County)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.County.Required"))
                .When(_ => addressSettings.CountyEnabled && addressSettings.CountyRequired);
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.City.Required"))
                .When(_ => addressSettings.CityEnabled && addressSettings.CityRequired);
            RuleFor(x => x.Address1)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Address1.Required"))
                .When(_ => addressSettings.StreetAddressEnabled && addressSettings.StreetAddressRequired);
            RuleFor(x => x.Address2)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.Address2.Required"))
                .When(_ => addressSettings.StreetAddress2Enabled && addressSettings.StreetAddress2Required);
            RuleFor(x => x.ZipPostalCode)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.ZipPostalCode.Required"))
                .When(_ => addressSettings.ZipPostalCodeEnabled && addressSettings.ZipPostalCodeRequired);
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.PhoneNumber.Required"))
                .When(_ => addressSettings.PhoneEnabled && addressSettings.PhoneRequired);
            RuleFor(x => x.FaxNumber)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Address.Fields.FaxNumber.Required"))
                .When(_ => addressSettings.FaxEnabled && addressSettings.FaxRequired);

            SetDatabaseValidationRules<Address>(dataProvider);
        }
    }
}