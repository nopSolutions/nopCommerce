using FluentValidation;
using Nop.Core.Domain.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Directory;

public partial class MeasureDimensionValidator : BaseNopValidator<MeasureDimensionModel>
{
    public MeasureDimensionValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Admin.Configuration.Shipping.Measures.Dimensions.Fields.Name.Required");
        RuleFor(x => x.SystemKeyword).NotEmpty().WithMessage("Admin.Configuration.Shipping.Measures.Dimensions.Fields.SystemKeyword.Required");

        SetDatabaseValidationRules<MeasureDimension>();
    }
}