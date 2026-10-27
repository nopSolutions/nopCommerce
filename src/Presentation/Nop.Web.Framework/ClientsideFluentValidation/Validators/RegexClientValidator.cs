using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Nop.Web.Framework.ClientsideFluentValidation.Validators;

/// <summary>
/// Represent the regex client validator
/// </summary>
public partial class RegexClientValidator : ClientValidatorBase
{
    #region Ctor

    public RegexClientValidator(IValidationRule rule, IRuleComponent component) : base(rule, component)
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
        if (Validator is not IRegularExpressionValidator regexVal)
            return;

        var message = BuildMessage(messageFormatter => messageFormatter, languageManager => languageManager.GetString("RegularExpressionValidator"));

        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-regex", message);
        context.Attributes.TryAdd("data-val-regex-pattern", regexVal.Expression);
    }

    #endregion
}
