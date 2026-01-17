using FluentValidation;

namespace Nop.Web.Framework.Validators;

public static class RuleBuilderOptionsExtension
{
    public static IRuleBuilderOptions<T, TProperty> WithMessageAwait<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, Task<string> errorMessage)
    {
        return rule.WithMessage(errorMessage.Result);
    }

    public static IRuleBuilderOptions<T, TProperty> WithMessageAwait<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, Func<T, Task<string>> errorMessage)
    {
        return rule.WithMessage(x => errorMessage.Invoke(x).GetAwaiter().GetResult());
    }

    public static IRuleBuilderOptions<T, TProperty> WithMessageAwait<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, Task<string> errorMessage, params object[] args)
    {
        return rule.WithMessage(string.Format(errorMessage.Result, args));
    }
}