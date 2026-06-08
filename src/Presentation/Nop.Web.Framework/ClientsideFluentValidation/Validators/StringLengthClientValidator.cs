using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Nop.Web.Framework.ClientsideFluentValidation.Validators;

/// <summary>
/// Represent the length client validator
/// </summary>
public partial class LengthClientValidator : ClientValidatorBase
{
    #region Ctor

    public LengthClientValidator(IValidationRule rule, IRuleComponent component) : base(rule, component)
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
        if (Validator is not ILengthValidator lengthVal)
            return;

        var message = BuildMessage(messageFormatter => messageFormatter
            .AppendArgument("MinLength", lengthVal.Min)
            .AppendArgument("MaxLength", lengthVal.Max), languageManager =>
        {
            if (lengthVal is IExactLengthValidator)
                return languageManager.GetString("ExactLength_Simple");

            return languageManager.GetString("Length_Simple");
        });

        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-length", message);
        context.Attributes.TryAdd("data-val-length-max", lengthVal.Max.ToString());
        context.Attributes.TryAdd("data-val-length-min", lengthVal.Min.ToString());
    }

    #endregion
}
