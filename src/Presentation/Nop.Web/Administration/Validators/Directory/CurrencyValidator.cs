using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Admin.Models.Directory;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Directory
{
    public class CurrencyValidator : AbstractValidator<CurrencyModel>
    {
        public CurrencyValidator(ILocalizationService localizationService)
        {
            //TODO localize
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Location.Currencies.Fields.Name.Required"))
                .Length(1, 50).WithMessage("Name must be less than or equal to 50 characters");
            RuleFor(x => x.CurrencyCode)
                .NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Location.Currencies.Fields.CurrencyCode.Required"))
                .Length(1, 5).WithMessage("Currency code must be less than or equal to 5 characters");
            RuleFor(x => x.CustomFormatting)
                .Length(0, 50).WithMessage("Custom formatting must be less than or equal to 50 characters");
            RuleFor(x => x.DisplayLocale)
                .Must(x =>
                {
                    try
                    {
                        var culture = new CultureInfo(x);
                        return culture != null;
                    }
                    catch
                    {
                        return false;
                    }
                })
                .WithMessage(localizationService.GetResource("Admin.Configuration.Location.Currencies.Fields.DisplayLocale.Validation"));
        }
    }
}