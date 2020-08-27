using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Nop.Core.Domain.Directory;
using Nop.Core.Events;
using Nop.Services.Directory;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Directory
{
    [TestFixture]
    public class MeasureServiceTests : ServiceTest
    {
        private FakeRepository<MeasureDimension> _measureDimensionRepository;
        private FakeRepository<MeasureWeight> _measureWeightRepository;
        private MeasureSettings _measureSettings;
        private Mock<IEventPublisher> _eventPublisher;
        private IMeasureService _measureService;

        private MeasureDimension measureDimension1, measureDimension2, measureDimension3, measureDimension4;
        private MeasureWeight measureWeight1, measureWeight2, measureWeight3, measureWeight4;
        
        [SetUp]
        public new void SetUp()
        {
            measureDimension1 = new MeasureDimension
            {
                Id = 1,
                Name = "inch(es)",
                SystemKeyword = "inches",
                Ratio = 1M,
                DisplayOrder = 1
            };
            measureDimension2 = new MeasureDimension
            {
                Id = 2,
                Name = "feet",
                SystemKeyword = "feet",
                Ratio = 0.08333333M,
                DisplayOrder = 2
            };
            measureDimension3 = new MeasureDimension
            {
                Id = 3,
                Name = "meter(s)",
                SystemKeyword = "meters",
                Ratio = 0.0254M,
                DisplayOrder = 3
            };
            measureDimension4 = new MeasureDimension
            {
                Id = 4,
                Name = "millimetre(s)",
                SystemKeyword = "millimetres",
                Ratio = 25.4M,
                DisplayOrder = 4
            };
            
            measureWeight1 = new MeasureWeight
            {
                Id = 1,
                Name = "ounce(s)",
                SystemKeyword = "ounce",
                Ratio = 16M,
                DisplayOrder = 1
            };
            measureWeight2 = new MeasureWeight
            {
                Id = 2,
                Name = "lb(s)",
                SystemKeyword = "lb",
                Ratio = 1M,
                DisplayOrder = 2
            };
            measureWeight3 = new MeasureWeight
            {
                Id = 3,
                Name = "kg(s)",
                SystemKeyword = "kg",
                Ratio = 0.45359237M,
                DisplayOrder = 3
            };
            measureWeight4 = new MeasureWeight
            {
                Id = 4,
                Name = "gram(s)",
                SystemKeyword = "grams",
                Ratio = 453.59237M,
                DisplayOrder = 4
            };

            _measureDimensionRepository = new FakeRepository<MeasureDimension>(new List<MeasureDimension> { measureDimension1, measureDimension2, measureDimension3, measureDimension4 });

            _measureWeightRepository = new FakeRepository<MeasureWeight>(new List<MeasureWeight> { measureWeight1, measureWeight2, measureWeight3, measureWeight4 });

            _measureSettings = new MeasureSettings
            {
                BaseDimensionId = measureDimension1.Id, //inch(es)
                BaseWeightId = measureWeight2.Id //lb(s)
            };

            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            _measureService = new MeasureService(_measureDimensionRepository,
                _measureWeightRepository,
                _measureSettings);
        }

        [Test]
        public void Can_convert_dimension()
        {
            RunWithTestServiceProvider(() =>
            {
                //from meter(s) to feet
                _measureService.ConvertDimension(10, measureDimension3, measureDimension2).Should().Be(32.81M);
                //from inch(es) to meter(s)
                _measureService.ConvertDimension(10, measureDimension1, measureDimension3).Should().Be(0.25M);
                //from meter(s) to meter(s)
                _measureService.ConvertDimension(13.333M, measureDimension3, measureDimension3).Should().Be(13.33M);
                //from meter(s) to millimeter(s)
                _measureService.ConvertDimension(10, measureDimension3, measureDimension4).Should().Be(10000);
                //from millimeter(s) to meter(s)
                _measureService.ConvertDimension(10000, measureDimension4, measureDimension3).Should().Be(10);
            });
        }

        [Test]
        public void Can_convert_weight()
        {
            RunWithTestServiceProvider(() =>
            {
                //from ounce(s) to lb(s)
                _measureService.ConvertWeight(11, measureWeight1, measureWeight2).Should().Be(0.69M);
                //from lb(s) to ounce(s)
                _measureService.ConvertWeight(11, measureWeight2, measureWeight1).Should().Be(176);
                //from ounce(s) to  ounce(s)
                _measureService.ConvertWeight(13.333M, measureWeight1, measureWeight1).Should().Be(13.33M);
                //from kg(s) to ounce(s)
                _measureService.ConvertWeight(11, measureWeight3, measureWeight1).Should().Be(388.01M);
                //from kg(s) to gram(s)
                _measureService.ConvertWeight(10, measureWeight3, measureWeight4).Should().Be(10000);
            });
        }
    }
}
