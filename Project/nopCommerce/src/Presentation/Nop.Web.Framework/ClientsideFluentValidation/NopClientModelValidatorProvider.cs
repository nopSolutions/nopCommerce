using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.ClientsideFluentValidation.Validators;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Framework.ClientsideFluentValidation;

/// <summary>
/// Used to generate client side metadata from FluentValidation's rules.
/// </summary>
public class NopClientModelValidatorProvider : IClientModelValidatorProvider
{
    #region Utilities

    /// <summary>
    /// Gets the appropriate validator for client side validation
    /// </summary>
    /// <param name="validationRule">Rule associated with a property</param>
    /// <param name="ruleComponent">An individual component within a rule with a validator attached</param>
    /// <returns>Appropriate client side validator or null</returns>
    protected virtual IClientModelValidator GetModelValidator(IValidationRule validationRule, IRuleComponent ruleComponent)
    {
        //no need to look for validators with a rule set
        //it's used to prevent auto-validation of child models
        if (validationRule?.RuleSets?.Any(rs => rs.Equals(NopValidationDefaults.ValidationRuleSet)) ?? false)
            return null;

        //default validator implementations
        Dictionary<Type, IClientModelValidator> clientValidatorFactories = new()
        {
            { typeof(INotNullValidator), new RequiredClientValidator(validationRule, ruleComponent) },
            { typeof(INotEmptyValidator), new RequiredClientValidator(validationRule, ruleComponent) },
            { typeof(IEmailValidator), new EmailClientValidator(validationRule, ruleComponent) },
            { typeof(IRegularExpressionValidator), new RegexClientValidator(validationRule, ruleComponent) },
            { typeof(IMaximumLengthValidator), new MaxLengthClientValidator(validationRule, ruleComponent) },
            { typeof(IMinimumLengthValidator), new MinLengthClientValidator(validationRule, ruleComponent) },
            { typeof(IExactLengthValidator), new LengthClientValidator(validationRule, ruleComponent)},
            { typeof(ILengthValidator), new LengthClientValidator(validationRule, ruleComponent)},
            { typeof(IInclusiveBetweenValidator), new RangeClientValidator(validationRule, ruleComponent) },
            { typeof(IGreaterThanOrEqualValidator), new RangeMinClientValidator(validationRule, ruleComponent) },
            { typeof(ILessThanOrEqualValidator), new RangeMaxClientValidator(validationRule, ruleComponent) },
            { typeof(IEqualValidator), new EqualToClientValidator(validationRule, ruleComponent) },
            { typeof(ICreditCardValidator), new CreditCardClientValidator(validationRule, ruleComponent) },
        };

        //allow third-party handlers to associate custom validators
        EngineContext.Current.Resolve<IEventPublisher>()
            .PublishAsync(new ClientModelValidatorsCreatedEvent(clientValidatorFactories, validationRule, ruleComponent))
            .Wait();

        var type = ruleComponent.Validator.GetType();
        var validator = clientValidatorFactories
            .FirstOrDefault(x => x.Key.IsAssignableFrom(type))
            .Value;

        return validator;
    }

    /// <summary>
    /// Gets cached metadata about a validator
    /// </summary>
    /// <param name="context">Client validator context</param>
    /// <returns>Metadata about a validator or null</returns>
    protected virtual IValidatorDescriptor GetCachedDescriptor(ClientValidatorProviderContext context)
    {
        var httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();

        ArgumentNullException.ThrowIfNull(httpContextAccessor?.HttpContext);

        var modelType = context.ModelMetadata.ContainerType;
        if (modelType == null)
            return null;

        //try to get a validator descriptor and cache it within the request
        Dictionary<Type, IValidatorDescriptor> cache = null;

        if (httpContextAccessor.HttpContext.Items.TryGetValue(NopValidationDefaults.NopClientValidationCacheKey, out var item))
            cache = item as Dictionary<Type, IValidatorDescriptor>;

        cache ??= new Dictionary<Type, IValidatorDescriptor>();
        httpContextAccessor.HttpContext.Items[NopValidationDefaults.NopClientValidationCacheKey] = cache;

        if (cache.TryGetValue(modelType, out var cachedDescriptor))
            return cachedDescriptor;

        var validator = EngineContext.Current.Resolve(typeof(IValidator<>).MakeGenericType(modelType)) as IValidator;

        cachedDescriptor = validator?.CreateDescriptor();
        cache[modelType] = cachedDescriptor;

        return cachedDescriptor;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates set of <see cref="T:Microsoft.AspNetCore.Mvc.ModelBinding.Validation.IClientModelValidator" />s by updating
    /// <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ClientValidatorItem.Validator" /> in <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ClientValidatorProviderContext.Results" />.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ClientModelValidationContext" /> associated with this call.</param>
    public void CreateValidators(ClientValidatorProviderContext context)
    {
        var descriptor = GetCachedDescriptor(context);
        if (descriptor == null)
            return;

        //prepare model validators
        var validatorsWithRules = descriptor.GetRulesForMember(context.ModelMetadata.PropertyName)
            .Where(rule => !rule.HasCondition && !rule.HasAsyncCondition)
            .Select(rule => new { rule, components = rule.Components })
            .Where(t => t.components.Any())
            .SelectMany(t => t.components, (t, component) => new { t, component })
            .Where(t => !t.component.HasCondition && !t.component.HasAsyncCondition)
            .Select(t => new { t, modelValidatorForProperty = GetModelValidator(t.t.rule, t.component) })
            .Where(t => t.modelValidatorForProperty != null)
            .Select(t => t.modelValidatorForProperty)
            .ToList();

        //and add them as items
        if (validatorsWithRules.Any())
        {
            foreach (var propVal in validatorsWithRules)
                context.Results.Add(new() { Validator = propVal, IsReusable = false });
        }
        else
        {
            //add one ClientValidatorItem with IsReusable = false to prevent MVC cache the list of validators
            context.Results.Add(new() { IsReusable = false });
        }

        //if the property is a non-nullable value type, then MVC will have already generated a Required rule
        if (context.ModelMetadata.ModelType.IsValueType &&
            Nullable.GetUnderlyingType(context.ModelMetadata.ModelType) == null)
        {
            var fvHasRequiredRule = context.Results.Any(x => x.Validator is RequiredClientValidator);
            
            //if we've provided our own Required rule, then remove the MVC one
            if (fvHasRequiredRule)
            {
                var dataAnnotationsRequiredRule =
                    context.Results.FirstOrDefault(x => x.Validator is RequiredAttributeAdapter);
                context.Results.Remove(dataAnnotationsRequiredRule);
            }
        }
    }

    #endregion
}