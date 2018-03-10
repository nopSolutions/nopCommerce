using Nop.Core.Domain.Directory;
using Nop.Services.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class RoundingHelperTests : ServiceTest
    {
        [TestCase(12.366, 12.37, RoundingType.Rounding001)]
        [TestCase(12.363, 12.36, RoundingType.Rounding001)]
        [TestCase(12.000, 12.00, RoundingType.Rounding001)]
        [TestCase(12.001, 12.00, RoundingType.Rounding001)]
        [TestCase(12.34, 12.35, RoundingType.Rounding005Up)]
        [TestCase(12.36, 12.40, RoundingType.Rounding005Up)]
        [TestCase(12.35, 12.35, RoundingType.Rounding005Up)]
        [TestCase(12.00, 12.00, RoundingType.Rounding005Up)]
        [TestCase(12.05, 12.05, RoundingType.Rounding005Up)]
        [TestCase(12.20, 12.20, RoundingType.Rounding005Up)]
        [TestCase(12.001, 12.00, RoundingType.Rounding005Up)]
        [TestCase(12.34, 12.30, RoundingType.Rounding005Down)]
        [TestCase(12.36, 12.35, RoundingType.Rounding005Down)]
        [TestCase(12.00, 12.00, RoundingType.Rounding005Down)]
        [TestCase(12.05, 12.05, RoundingType.Rounding005Down)]
        [TestCase(12.20, 12.20, RoundingType.Rounding005Down)]
        [TestCase(12.35, 12.40, RoundingType.Rounding01Up)]
        [TestCase(12.36, 12.40, RoundingType.Rounding01Up)]
        [TestCase(12.00, 12.00, RoundingType.Rounding01Up)]
        [TestCase(12.10, 12.10, RoundingType.Rounding01Up)]
        [TestCase(12.35, 12.30, RoundingType.Rounding01Down)]
        [TestCase(12.36, 12.40, RoundingType.Rounding01Down)]
        [TestCase(12.00, 12.00, RoundingType.Rounding01Down)]
        [TestCase(12.10, 12.10, RoundingType.Rounding01Down)]
        [TestCase(12.24, 12.00, RoundingType.Rounding05)]
        [TestCase(12.49, 12.50, RoundingType.Rounding05)]
        [TestCase(12.74, 12.50, RoundingType.Rounding05)]
        [TestCase(12.99, 13.00, RoundingType.Rounding05)]
        [TestCase(12.00, 12.00, RoundingType.Rounding05)]
        [TestCase(12.50, 12.50, RoundingType.Rounding05)]
        [TestCase(12.49, 12.00, RoundingType.Rounding1)]
        [TestCase(12.50, 13.00, RoundingType.Rounding1)]
        [TestCase(12.00, 12.00, RoundingType.Rounding1)]
        [TestCase(12.01, 13.00, RoundingType.Rounding1Up)]
        [TestCase(12.99, 13.00, RoundingType.Rounding1Up)]
        [TestCase(12.00, 12.00, RoundingType.Rounding1Up)]
        public void can_round(decimal valueToRoundig, decimal roundedValue, RoundingType roundingType)
        {
            valueToRoundig.Round(roundingType).ShouldEqual(roundedValue);
        }
    }
}