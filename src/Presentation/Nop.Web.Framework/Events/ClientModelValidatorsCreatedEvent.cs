using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents an event that occurs after dictionary of client side model validators is created
/// </summary>
public partial class ClientModelValidatorsCreatedEvent
{
    #region Fields

    protected readonly Dictionary<Type, IClientModelValidator> _clientValidatorFactories;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="clientValidatorFactories">Dictionary of client side model validators</param>
    /// <param name="rule">Rule associated with a property</param>
    /// <param name="component">Individual component within a rule with a validator attached</param>
    public ClientModelValidatorsCreatedEvent(Dictionary<Type, IClientModelValidator> clientValidatorFactories, IValidationRule rule, IRuleComponent component)
    {
        _clientValidatorFactories = clientValidatorFactories;
        Rule = rule;
        Component = component;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds a client model validator for the specified type if one does not already exist
    /// </summary>
    /// <remarks>
    /// This method enables the registration of custom client-side validation logic for model
    /// binding. If a validator for the specified type is already registered, the method overwrite it
    /// </remarks>
    /// <param name="type">The type for which the client model validator is to be registered</param>
    /// <param name="clientModelValidator">The client model validator instance to associate with the specified type.</param>
    /// <returns>True if the client model validator was successfully added; otherwise, false.</returns>
    public virtual bool AddClientModelValidator(Type type, IClientModelValidator clientModelValidator)
    {
        if (!_clientValidatorFactories.ContainsKey(type))
            return _clientValidatorFactories.TryAdd(type, clientModelValidator);

        _clientValidatorFactories[type] = clientModelValidator;

        return true;
    }

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