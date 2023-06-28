using FluentValidation;

namespace Nop.Web.Framework.Validators
{
    public static class RuleBuilderOptionsExtension
    {
        public static IRuleBuilderOptions<T, TProperty> WithMessageAwait<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule, Task<string> errorMessage)
        {
            return rule.WithMessage(errorMessage.Result);
        }

        public static IRuleBuilderOptions<T, TProperty> WithMessageAwait<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule, Func<Task<string>> errorMessage)
        {
            return rule.WithMessage(errorMessage().Result);
        }

        public static IRuleBuilderOptions<T, TProperty> WithMessageAwait<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule, Task<string> errorMessage, params object[] args)
        {
            return rule.WithMessage(string.Format(errorMessage.Result, args));
        }

        public static IRuleBuilderOptions<T, TProperty> MustAwait<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
            Func<T, TProperty, Task<bool>> predicate)
        {
            return ruleBuilder.Must((x, context) => predicate(x, context).Result);
        }

        public static IRuleBuilderOptions<T, TProperty> WhenAwait<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule,
            Func<T, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
        {
            return rule.When((x) => predicate(x).Result, applyConditionTo);
        }
    }
}