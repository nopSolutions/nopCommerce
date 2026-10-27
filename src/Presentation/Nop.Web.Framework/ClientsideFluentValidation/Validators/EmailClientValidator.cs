using FluentValidation;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Nop.Web.Framework.ClientsideFluentValidation.Validators;

/// <summary>
/// Represent the email client validator
/// </summary>
public partial class EmailClientValidator : ClientValidatorBase
{
    #region Ctor

    public EmailClientValidator(IValidationRule rule, IRuleComponent component) : base(rule, component)
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
        var message = BuildMessage(messageFormatter => messageFormatter, languageManager => languageManager.GetString("EmailValidator"));

        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-email", message);
    }

    #endregion
}
