using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class CurrencyValidator : AbstractValidator<CurrencyModel>
    {
        public CurrencyValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Location.Currencies.Fields.Name.Validation"));
            RuleFor(x => x.CurrencyCode).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Location.Currencies.Fields.CurrencyCode.Validation"));
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