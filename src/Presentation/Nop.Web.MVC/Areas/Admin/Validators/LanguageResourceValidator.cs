using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.MVC.Areas.Admin.Models;

namespace Nop.Web.MVC.Areas.Admin.Validators
{
    public class LanguageResourceValidator : AbstractValidator<LanguageResourceModel>
    {
        #region Constructors (1) 

        public LanguageResourceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Configuration.Location.Languages.Resources.Fields.Name.Validation"));
        }

		#endregion Constructors 
    }
}