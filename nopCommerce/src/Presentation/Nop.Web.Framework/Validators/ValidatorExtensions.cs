using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Validators;

/// <summary>
/// Validator extensions
/// </summary>
public static class ValidatorExtensions
{
    /// <summary>
    /// Set credit card validator
    /// </summary>
    /// <typeparam name="TModel">Type of model being validated</typeparam>
    /// <param name="ruleBuilder">Rule builder</param>
    /// <returns>Result</returns>
    public static IRuleBuilderOptions<TModel, string> IsCreditCard<TModel>(this IRuleBuilder<TModel, string> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new CreditCardPropertyValidator<TModel, string>());
    }

    /// <summary>
    /// Set decimal validator
    /// </summary>
    /// <typeparam name="TModel">Type of model being validated</typeparam>
    /// <param name="ruleBuilder">Rule builder</param>
    /// <param name="maxValue">Maximum value</param>
    /// <returns>Result</returns>
    public static IRuleBuilderOptions<TModel, decimal> IsDecimal<TModel>(this IRuleBuilder<TModel, decimal> ruleBuilder, decimal maxValue)
    {
        return ruleBuilder.SetValidator(new DecimalPropertyValidator<TModel, decimal>(maxValue));
    }

    /// <summary>
    /// Set username validator
    /// </summary>
    /// <typeparam name="TModel">Type of model being validated</typeparam>
    /// <param name="ruleBuilder">Rule builder</param>
    /// <param name="customerSettings">Customer settings</param>
    /// <returns>Result</returns>
    public static IRuleBuilderOptions<TModel, string> IsUsername<TModel>(this IRuleBuilder<TModel, string> ruleBuilder,
        CustomerSettings customerSettings)
    {
        return ruleBuilder.SetValidator(new UsernamePropertyValidator<TModel, string>(customerSettings));
    }

    /// <summary>
    /// Set phone number validator
    /// </summary>
    /// <typeparam name="TModel">Type of model being validated</typeparam>
    /// <param name="ruleBuilder">Rule builder</param>
    /// <param name="customerSettings">Customer settings</param>
    /// <returns>Result</returns>
    public static IRuleBuilderOptions<TModel, string> IsPhoneNumber<TModel>(this IRuleBuilder<TModel, string> ruleBuilder,
        CustomerSettings customerSettings)
    {
        return ruleBuilder.SetValidator(new PhoneNumberPropertyValidator<TModel, string>(customerSettings));
    }

    /// <summary>
    /// Implement password validator
    /// </summary>
    /// <typeparam name="TModel">Type of model being validated</typeparam>
    /// <param name="ruleBuilder">Rule builder</param>
    /// <param name="localizationService">Localization service</param>
    /// <param name="customerSettings">Customer settings</param>
    /// <returns>Result</returns>
    public static IRuleBuilder<TModel, string> IsPassword<TModel>(this IRuleBuilder<TModel, string> ruleBuilder,
        ILocalizationService localizationService, CustomerSettings customerSettings)
    {
        return ruleBuilder.SetValidator(new PasswordPropertyValidator<TModel, string>(localizationService, customerSettings));
    }

    /// <summary>
    /// Set email address validator
    /// </summary>
    /// <typeparam name="TModel">Type of model being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<TModel, string> IsEmailAddress<TModel>(this IRuleBuilder<TModel, string> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new EmailPropertyValidator<TModel>());
    }
}