using FluentValidation;
using FluentValidation.Validators;
using Nop.Core;

namespace Nop.Web.Framework.Validators;

/// <summary>
/// Email address validator
/// </summary>
public partial class EmailPropertyValidator<T> : PropertyValidator<T, string>, IRegularExpressionValidator, IPropertyValidator
{

    #region Utilities

    /// <summary>
    /// Returns the default error message template for this validator, when not overridden.
    /// </summary>
    /// <param name="errorCode">The currently configured error code for the validator.</param>
    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return Localized(errorCode, Name);
    }

    #endregion


    #region Methods

    /// <summary>
    /// Validates a specific property value.
    /// </summary>
    /// <param name="context">The validation context. The parent object can be obtained from here.</param>
    /// <param name="value">The current property value to validate</param>
    /// <returns>True if valid, otherwise false.</returns>
    public override bool IsValid(ValidationContext<T> context, string value)
    {
        return value == null || CommonHelper.GetEmailRegex().IsMatch(value);
    }

    #endregion

    #region Properties

    /// <summary>
    /// The name of the validator. This is usually the type name without any generic parameters. This is used as the default Error Code for the validator.
    /// </summary>
    public override string Name => "EmailPropertyValidator";

    /// <summary>
    /// Gets regular expression for client side (pattern source: https://emailregex.com/)
    /// </summary>
    public string Expression => @"^(([^<>()\[\]\\.,;:\s@""]+(\.[^<>()\[\]\\.,;:\s@""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";

    #endregion


}