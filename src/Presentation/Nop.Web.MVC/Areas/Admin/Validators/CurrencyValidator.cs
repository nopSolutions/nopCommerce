using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.MVC.Areas.Admin.Models;

namespace Nop.Web.MVC.Areas.Admin.Validators
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