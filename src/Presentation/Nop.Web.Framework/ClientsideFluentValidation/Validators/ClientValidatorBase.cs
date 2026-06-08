using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Resources;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.ClientsideFluentValidation.Validators;

/// <summary>
/// Specifies the base contract for performing validation in the browser.
/// </summary>
public abstract partial class ClientValidatorBase : IClientModelValidator
{
    #region Ctor

    protected ClientValidatorBase(IValidationRule rule, IRuleComponent component)
    {
        Component = component;
        Rule = rule;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Constructs the final message from the specified template
    /// </summary>
    /// <param name="messageFormatter">Function to get assists in the construction of validation messages</param>
    /// <param name="defaultErrorMessage">Function to get default error message</param>
    /// <returns>The message with placeholders replaced with their appropriate values</returns>
    protected virtual string BuildMessage(Func<MessageFormatter, MessageFormatter> messageFormatter,
        Func<ILanguageManager, string> defaultErrorMessage)
    {
        var validatorConfiguration = EngineContext.Current.Resolve<ValidatorConfiguration>();
        string message;

        try
        {
            message = Component.GetUnformattedErrorMessage();
        }
        catch (NullReferenceException)
        {
            message = defaultErrorMessage(validatorConfiguration.LanguageManager);
        }

        return messageFormatter(validatorConfiguration.MessageFormatterFactory().AppendPropertyName(Rule.GetDisplayName(null)))
            .BuildMessage(message);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Called to add client-side model validation
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ClientModelValidationContext" /></param>
    public abstract void AddValidation(ClientModelValidationContext context);

    #endregion

    #region Properties

    /// <summary>
    /// Gets the custom property validator
    /// </summary>
    public virtual IPropertyValidator Validator => Component.Validator;

    /// <summary>
    /// Gets a rule associated with a property
    /// </summary>
    public virtual IValidationRule Rule { get; }

    /// <summary>
    /// Gets an individual component within a rule with a validator attached
    /// </summary>
    public virtual IRuleComponent Component { get; }

    #endregion
}
