using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Common;

public partial class AddressValidator : BaseNopValidator<AddressModel>
{
    public AddressValidator(AddressSettings addressSettings, ILocalizationService localizationService)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Admin.Address.Fields.FirstName.Required")
            .When(x => x.FirstNameRequired);
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Admin.Address.Fields.LastName.Required")
            .When(x => x.LastNameRequired);
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Admin.Address.Fields.Email.Required")
            .When(x => x.EmailRequired);
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessage("Admin.Common.WrongEmail");
        RuleFor(x => x.Company)
            .NotEmpty()
            .WithMessage("Admin.Address.Fields.Company.Required")
            .When(x => addressSettings.CompanyEnabled && x.CompanyRequired);
        RuleFor(x => x.CountryId)
            .NotNull()
            .WithMessage("Admin.Address.Fields.Country.Required")
            .When(x => addressSettings.CountryEnabled && x.CountryRequired);
        RuleFor(x => x.CountryId)
            .NotEqual(0)
            .WithMessage("Admin.Address.Fields.Country.Required")
            .When(x => addressSettings.CountryEnabled && x.CountryRequired);
        RuleFor(x => x.County)
            .NotEmpty()
            .WithMessage("Admin.Address.Fields.County.Required")
            .When(x => addressSettings.CountyEnabled && x.CountyRequired);
        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("Admin.Address.Fields.City.Required")
            .When(x => addressSettings.CityEnabled && x.CityRequired);
        RuleFor(x => x.Address1)
            .NotEmpty()
            .WithMessage("Admin.Address.Fields.Address1.Required")
            .When(x => addressSettings.StreetAddressEnabled && x.StreetAddressRequired);
        RuleFor(x => x.Address2)
            .NotEmpty()
            .WithMessage("Admin.Address.Fields.Address2.Required")
            .When(x => addressSettings.StreetAddress2Enabled && x.StreetAddress2Required);
        RuleFor(x => x.ZipPostalCode)
            .NotEmpty()
            .WithMessage("Admin.Address.Fields.ZipPostalCode.Required")
            .When(x => addressSettings.ZipPostalCodeEnabled && x.ZipPostalCodeRequired);
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Admin.Address.Fields.PhoneNumber.Required")
            .When(x => addressSettings.PhoneEnabled && x.PhoneRequired);
        RuleFor(x => x.FaxNumber)
            .NotEmpty()
            .WithMessage("Admin.Address.Fields.FaxNumber.Required")
            .When(x => addressSettings.FaxEnabled && x.FaxRequired);

        SetDatabaseValidationRules<Address>();
    }
}