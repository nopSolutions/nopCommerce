using System.Reflection;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.ClientsideFluentValidation.Validators;

/// <summary>
/// Represent the equal to client validator
/// </summary>
public partial class EqualToClientValidator : ClientValidatorBase
{
    #region Ctor

    public EqualToClientValidator(IValidationRule rule, IRuleComponent component) : base(rule, component)
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
        if (((IComparisonValidator)Validator).MemberToCompare is not PropertyInfo propertyToCompare)
            return;

        var validatorConfiguration = EngineContext.Current.Resolve<ValidatorConfiguration>();

        var comparisonDisplayName = validatorConfiguration.DisplayNameResolver(Rule.TypeToValidate, propertyToCompare, null)
            ?? CommonHelper.SplitCamelCaseWord(propertyToCompare.Name);

        var message = BuildMessage(messageFormater => messageFormater.AppendArgument("ComparisonValue", comparisonDisplayName),
            languageManager => languageManager.GetString("EqualValidator"));

        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-equalto", message);
        context.Attributes.TryAdd("data-val-equalto-other", "*." + propertyToCompare.Name);
    }

    #endregion
}
