using System;
using System.Threading.Tasks;
using Nop.Core.Domain.Directory;
using Optional;
using Triplex.Validations;

namespace Nop.Services.Directory;

/// <summary>
/// Convert money amount between currencies. All conversion operations take into account when no conversion is needed 
/// (zero amount or source equals target currency)and when the rate to be used is invalid (Zero). 
/// </summary>
internal sealed record class MoneyConverter(decimal Amount, Currency Source, Currency Target)
{
    /// <summary>Custom conversion logic, using lambda.</summary>
    /// <remarks><paramref name="asyncConverter"/> is not invoked if no conversion is required.</remarks>
    /// <param name="asyncConverter">Can not be <see langword="null" /></param>
    internal async Task<decimal> ConvertAsync(Func<MoneyConverter, Task<decimal>> asyncConverter)
    {
        Arguments.NotNull(asyncConverter, nameof(asyncConverter));

        return DoesNotRequireConversion() ? Amount : await asyncConverter(this);
    }

    /// <summary>Source to Target conversion.</summary>
    /// <returns>Filled option when <see cref="Source"> has non-zero rate.</returns>
    internal Option<decimal> ConvertForward()
        => Amount.SomeWhen(_ => DoesNotRequireConversion())
                 .Else(NonZeroSourceRate.Map(sourceRate => Amount / sourceRate));

    /// <summary>Target to Source conversion.</summary>
    /// <returns>Filled option when <see cref="Target"> has non-zero rate.</returns>
    internal Option<decimal> ConvertBackward()
        => Amount.SomeWhen(_ => DoesNotRequireConversion())
                 .Else(NonZeroTargetRate.Map(targetRate => Amount * targetRate));

    private bool DoesNotRequireConversion()
        => Amount is decimal.Zero || Source.Id == Target.Id;

    private decimal SourceRate => Source.Rate;
    private decimal TargetRate => Target.Rate;

    private Option<decimal> NonZeroSourceRate
        => SourceRate.NoneWhen(x => x is decimal.Zero);

    private Option<decimal> NonZeroTargetRate
        => TargetRate.NoneWhen(x => x is decimal.Zero);
}
