using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class CurrencyValidator : AbstractValidator<CurrencyModel>
    {
        #region Constructors (1)

        public CurrencyValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Admin.Common.Validation.Required");
            RuleFor(x => x.CurrencyCode).NotEmpty().WithMessage("Admin.Common.Validation.Required");
        }

        #endregion Constructors
    }
}