using System.Globalization;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Nop.Web.Framework.ClientsideFluentValidation.Validators;

/// <summary>
/// Represent the range client validator
/// </summary>
public partial class RangeClientValidator : ClientValidatorBase
{
    #region Ctor

    public RangeClientValidator(IValidationRule rule, IRuleComponent component) : base(rule, component)
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
        if (Validator is not IBetweenValidator rangeValidator || rangeValidator.To == null || rangeValidator.From == null)
            return;

        var message = BuildMessage(messageFormatter => messageFormatter
            .AppendArgument("From", rangeValidator.From)
            .AppendArgument("To", rangeValidator.To), languageManager => languageManager.GetString("InclusiveBetween_Simple"));

        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-range", message);
        context.Attributes.TryAdd("data-val-range-max", Convert.ToString(rangeValidator.To, CultureInfo.InvariantCulture));
        context.Attributes.TryAdd("data-val-range-min", Convert.ToString(rangeValidator.From, CultureInfo.InvariantCulture));
    }

    #endregion
}
