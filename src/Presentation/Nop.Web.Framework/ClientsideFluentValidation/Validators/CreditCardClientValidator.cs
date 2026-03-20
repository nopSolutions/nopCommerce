using FluentValidation;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Nop.Web.Framework.ClientsideFluentValidation.Validators;

/// <summary>
/// Represent the credit card client validator
/// </summary>
public partial class CreditCardClientValidator : ClientValidatorBase
{
    #region Ctor

    public CreditCardClientValidator(IValidationRule rule, IRuleComponent component) : base(rule, component)
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
        var message = BuildMessage(messageFormatter => messageFormatter, languageManager => languageManager.GetString("CreditCardValidator"));

        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-creditcard", message);
    }

    #endregion
}
