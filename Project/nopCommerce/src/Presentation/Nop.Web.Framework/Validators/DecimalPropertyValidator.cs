using FluentValidation;
using FluentValidation.Validators;

namespace Nop.Web.Framework.Validators;

/// <summary>
/// Decimal validator
/// </summary>
public partial class DecimalPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
    protected readonly decimal _maxValue;

    public override string Name => "DecimalPropertyValidator";

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="maxValue">Maximum value</param>
    public DecimalPropertyValidator(decimal maxValue)
    {
        _maxValue = maxValue;
    }

    /// <summary>
    /// Is valid?
    /// </summary>
    /// <param name="context">Validation context</param>
    /// <returns>Result</returns>
    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
        if (decimal.TryParse(value.ToString(), out var propertyValue))
            return Math.Round(propertyValue, 3) < _maxValue;

        return false;
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "Decimal value is out of range";
}