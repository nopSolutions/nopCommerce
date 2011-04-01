using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class MeasureWeightValidator : AbstractValidator<MeasureWeightModel>
    {
		#region Constructors 

        public MeasureWeightValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Configuration.Measures.Weights.Fields.Name.Validation"));
            RuleFor(x => x.SystemKeyword).NotNull().WithMessage(localizationService.GetResource("Admin.Configuration.Measures.Weights.Fields.SystemKeyword.Validation"));
        }

		#endregion Constructors 
    }
}