using System;
using System.Threading.Tasks;
using FluentValidation;
using Nop.Core;

namespace Nop.Web.Framework.Validators
{
    public static class RuleBuilderOptionsExtension
    {
        public static IRuleBuilderOptions<T, TProperty> WithMessageAwait<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule, Task<string> errorMessage)
        {
            return rule.WithMessage(AsyncHelper.RunSync(() => errorMessage));
        }

        public static IRuleBuilderOptions<T, TProperty> WithMessageAwait<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule, Func<Task<string>> errorMessage)
        {
            return rule.WithMessage(AsyncHelper.RunSync(() => errorMessage()));
        }

        public static IRuleBuilderOptions<T, TProperty> WithMessageAwait<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule, Func<T, Task<string>> errorMessage)
        {
            return rule.WithMessage(x => AsyncHelper.RunSync(() => errorMessage(x)));
        }

        public static IRuleBuilderOptions<T, TProperty> WithMessageAwait<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule, Task<string> errorMessage, params object[] args)
        {
            return rule.WithMessage(string.Format(AsyncHelper.RunSync(() => errorMessage), args));
        }

        public static IRuleBuilderOptions<T, TProperty> MustAwait<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
            Func<T, TProperty, Task<bool>> predicate)
        {
            return ruleBuilder.Must((x, context) => AsyncHelper.RunSync(() => predicate(x, context)));
        }

        public static IRuleBuilderOptions<T, TProperty> WhenAwait<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule,
            Func<T, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
        {
            return rule.When((x) => AsyncHelper.RunSync(() => predicate(x)), applyConditionTo);
        }
    }
}