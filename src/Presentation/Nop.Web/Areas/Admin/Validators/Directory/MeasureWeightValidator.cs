using FluentValidation;
using Nop.Core.Domain.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Directory;

public partial class MeasureWeightValidator : BaseNopValidator<MeasureWeightModel>
{
    public MeasureWeightValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Admin.Configuration.Shipping.Measures.Weights.Fields.Name.Required");
        RuleFor(x => x.SystemKeyword).NotEmpty().WithMessage("Admin.Configuration.Shipping.Measures.Weights.Fields.SystemKeyword.Required");

        SetDatabaseValidationRules<MeasureWeight>();
    }
}