using FluentAssertions;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using NUnit.Framework;

namespace Nop.Services.Tests.Directory
{
    [TestFixture]
    public class MeasureServiceTests : ServiceTest
    {
        private IMeasureService _measureService;

        private MeasureDimension _measureDimensionInches, _measureDimensionFeet, _measureDimensionMeters, _measureDimensionMillimetres;
        private MeasureWeight _measureWeightOunce, _measureWeightLb, _measureWeightKg, _measureWeightGrams;
        
        [SetUp]
        public void SetUp()
        {
            _measureService = GetService<IMeasureService>();

            _measureDimensionInches = _measureService.GetMeasureDimensionBySystemKeyword("inches");
            _measureDimensionFeet = _measureService.GetMeasureDimensionBySystemKeyword("feet");
            _measureDimensionMeters = _measureService.GetMeasureDimensionBySystemKeyword("meters");
            _measureDimensionMillimetres = _measureService.GetMeasureDimensionBySystemKeyword("millimetres");

            _measureWeightOunce = _measureService.GetMeasureWeightBySystemKeyword("ounce");
            _measureWeightLb = _measureService.GetMeasureWeightBySystemKeyword("lb");
            _measureWeightKg = _measureService.GetMeasureWeightBySystemKeyword("kg");
            _measureWeightGrams = _measureService.GetMeasureWeightBySystemKeyword("grams");
        }

        [Test]
        public void CanConvertDimension()
        {
            //from meter(s) to feet
            _measureService.ConvertDimension(10, _measureDimensionMeters, _measureDimensionFeet).Should().Be(32.81M);
            //from inch(es) to meter(s)
            _measureService.ConvertDimension(10, _measureDimensionInches, _measureDimensionMeters).Should().Be(0.25M);
            //from meter(s) to meter(s)
            _measureService.ConvertDimension(13.333M, _measureDimensionMeters, _measureDimensionMeters).Should().Be(13.33M);
            //from meter(s) to millimeter(s)
            _measureService.ConvertDimension(10, _measureDimensionMeters, _measureDimensionMillimetres).Should().Be(10000);
            //from millimeter(s) to meter(s)
            _measureService.ConvertDimension(10000, _measureDimensionMillimetres, _measureDimensionMeters).Should().Be(10);
        }

        [Test]
        public void CanConvertWeight()
        {
            //from ounce(s) to lb(s)
            _measureService.ConvertWeight(11, _measureWeightOunce, _measureWeightLb).Should().Be(0.69M);
            //from lb(s) to ounce(s)
            _measureService.ConvertWeight(11, _measureWeightLb, _measureWeightOunce).Should().Be(176);
            //from ounce(s) to  ounce(s)
            _measureService.ConvertWeight(13.333M, _measureWeightOunce, _measureWeightOunce).Should().Be(13.33M);
            //from kg(s) to ounce(s)
            _measureService.ConvertWeight(11, _measureWeightKg, _measureWeightOunce).Should().Be(388.01M);
            //from kg(s) to gram(s)
            _measureService.ConvertWeight(10, _measureWeightKg, _measureWeightGrams).Should().Be(10000);
        }
    }
}
