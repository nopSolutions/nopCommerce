using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Directory
{
    [TestFixture]
    public class MeasureServiceTests : ServiceTest
    {
        private IMeasureService _measureService;

        private MeasureDimension _measureDimensionInches, _measureDimensionFeet, _measureDimensionMeters, _measureDimensionMillimetres;
        private MeasureWeight _measureWeightOunce, _measureWeightLb, _measureWeightKg, _measureWeightGrams;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _measureService = GetService<IMeasureService>();

            _measureDimensionInches = await _measureService.GetMeasureDimensionBySystemKeywordAsync("inches");
            _measureDimensionFeet = await _measureService.GetMeasureDimensionBySystemKeywordAsync("feet");
            _measureDimensionMeters = await _measureService.GetMeasureDimensionBySystemKeywordAsync("meters");
            _measureDimensionMillimetres = await _measureService.GetMeasureDimensionBySystemKeywordAsync("millimetres");

            _measureWeightOunce = await _measureService.GetMeasureWeightBySystemKeywordAsync("ounce");
            _measureWeightLb = await _measureService.GetMeasureWeightBySystemKeywordAsync("lb");
            _measureWeightKg = await _measureService.GetMeasureWeightBySystemKeywordAsync("kg");
            _measureWeightGrams = await _measureService.GetMeasureWeightBySystemKeywordAsync("grams");
        }

        [Test]
        public async Task CanConvertDimension()
        {
            //from meter(s) to feet
            var newDimension = await _measureService.ConvertDimensionAsync(10, _measureDimensionMeters, _measureDimensionFeet);
            newDimension.Should().Be(32.81M);
            //from inch(es) to meter(s)
            newDimension = await _measureService.ConvertDimensionAsync(10, _measureDimensionInches, _measureDimensionMeters);
            newDimension.Should().Be(0.25M);
            //from meter(s) to meter(s)
            newDimension = await _measureService.ConvertDimensionAsync(13.333M, _measureDimensionMeters, _measureDimensionMeters);
            newDimension.Should().Be(13.33M);
            //from meter(s) to millimeter(s)
            newDimension = await _measureService.ConvertDimensionAsync(10, _measureDimensionMeters, _measureDimensionMillimetres);
            newDimension.Should().Be(10000);
            //from millimeter(s) to meter(s)
            newDimension = await _measureService.ConvertDimensionAsync(10000, _measureDimensionMillimetres, _measureDimensionMeters);
            newDimension.Should().Be(10);
        }

        [Test]
        public async Task CanConvertWeight()
        {
            //from ounce(s) to lb(s)
            var newWeight = await _measureService.ConvertWeightAsync(11, _measureWeightOunce, _measureWeightLb);
            newWeight.Should().Be(0.69M);
            //from lb(s) to ounce(s)
            newWeight = await _measureService.ConvertWeightAsync(11, _measureWeightLb, _measureWeightOunce);
            newWeight.Should().Be(176);
            //from ounce(s) to  ounce(s)
            newWeight = await _measureService.ConvertWeightAsync(13.333M, _measureWeightOunce, _measureWeightOunce);
            newWeight.Should().Be(13.33M);
            //from kg(s) to ounce(s)
            newWeight = await _measureService.ConvertWeightAsync(11, _measureWeightKg, _measureWeightOunce);
            newWeight.Should().Be(388.01M);
            //from kg(s) to gram(s)
            newWeight = await _measureService.ConvertWeightAsync(10, _measureWeightKg, _measureWeightGrams);
            newWeight.Should().Be(10000);
        }


        [Test]
        public async Task TestMeasureDimensionCrud()
        {
            var insertItem = new MeasureDimension
            {
                Name = "Test name",
                SystemKeyword = "test"
            };

            var updateItem = new MeasureDimension {
                Name = "Test name 1",
                SystemKeyword = "test"
            };

            await TestCrud(insertItem, _measureService.InsertMeasureDimensionAsync, updateItem, _measureService.UpdateMeasureDimensionAsync, _measureService.GetMeasureDimensionByIdAsync, (item, other) => item.Name.Equals(other.Name), _measureService.DeleteMeasureDimensionAsync);
        }

        [Test]
        public async Task TestMeasureWeightCrud()
        {
            var insertItem = new MeasureWeight
            {
                Name = "Test name",
                SystemKeyword = "test"
            };

            var updateItem = new MeasureWeight
            {
                Name = "Test name 1",
                SystemKeyword = "test"
            };

            await TestCrud(insertItem, _measureService.InsertMeasureWeightAsync, updateItem, _measureService.UpdateMeasureWeightAsync, _measureService.GetMeasureWeightByIdAsync, (item, other) => item.Name.Equals(other.Name), _measureService.DeleteMeasureWeightAsync);
        }
    }
}
