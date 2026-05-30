using System.Globalization;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Nop.Web.Framework.ClientsideFluentValidation.Validators;

/// <summary>
/// Represent the range min client validator
/// </summary>
public partial class RangeMinClientValidator : ClientValidatorBase
{
    #region Ctor

    public RangeMinClientValidator(IValidationRule rule, IRuleComponent component) : base(rule, component)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Called to add client-side model validation
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ClientModelValidationContext" /></param>
    public override void AddValidation(ClientModelValidationContext context)
    {
        if (Validator is not IComparisonValidator rangeValidator || rangeValidator.ValueToCompare == null)
            return;

        var message = BuildMessage(messageFormatter => messageFormatter
            .AppendArgument("ComparisonValue", rangeValidator.ValueToCompare), languageManager => languageManager.GetString("GreaterThanOrEqualValidator"));

        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-range", message);
        context.Attributes.TryAdd("data-val-range-min", Convert.ToString(rangeValidator.ValueToCompare, CultureInfo.InvariantCulture));
    }

    #endregion
}
