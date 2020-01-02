using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Directory
{
    [TestFixture]
    public class MeasureServiceTests : ServiceTest
    {
        private Mock<IRepository<MeasureDimension>> _measureDimensionRepository;
        private Mock<IRepository<MeasureWeight>> _measureWeightRepository;
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

            _measureDimensionRepository = new Mock<IRepository<MeasureDimension>>();
            _measureDimensionRepository.Setup(x => x.Table).Returns(new List<MeasureDimension> { measureDimension1, measureDimension2, measureDimension3, measureDimension4 }.AsQueryable());
            _measureDimensionRepository.Setup(x => x.GetById(measureDimension1.Id)).Returns(measureDimension1);
            _measureDimensionRepository.Setup(x => x.GetById(measureDimension2.Id)).Returns(measureDimension2);
            _measureDimensionRepository.Setup(x => x.GetById(measureDimension3.Id)).Returns(measureDimension3);
            _measureDimensionRepository.Setup(x => x.GetById(measureDimension4.Id)).Returns(measureDimension4);

            _measureWeightRepository = new Mock<IRepository<MeasureWeight>>();
            _measureWeightRepository.Setup(x => x.Table).Returns(new List<MeasureWeight> { measureWeight1, measureWeight2, measureWeight3, measureWeight4 }.AsQueryable());
            _measureWeightRepository.Setup(x => x.GetById(measureWeight1.Id)).Returns(measureWeight1);
            _measureWeightRepository.Setup(x => x.GetById(measureWeight2.Id)).Returns(measureWeight2);
            _measureWeightRepository.Setup(x => x.GetById(measureWeight3.Id)).Returns(measureWeight3);
            _measureWeightRepository.Setup(x => x.GetById(measureWeight4.Id)).Returns(measureWeight4);

            var cacheManager = new TestCacheManager();

            _measureSettings = new MeasureSettings
            {
                BaseDimensionId = measureDimension1.Id, //inch(es)
                BaseWeightId = measureWeight2.Id //lb(s)
            };

            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            _measureService = new MeasureService(cacheManager,
                _eventPublisher.Object,
                _measureDimensionRepository.Object,
                _measureWeightRepository.Object,
                _measureSettings);
        }

        [Test]
        public void Can_convert_dimension()
        {
            //from meter(s) to feet
            _measureService.ConvertDimension(10, measureDimension3, measureDimension2).ShouldEqual(32.81);
            //from inch(es) to meter(s)
            _measureService.ConvertDimension(10, measureDimension1, measureDimension3).ShouldEqual(0.25);
            //from meter(s) to meter(s)
            _measureService.ConvertDimension(13.333M, measureDimension3, measureDimension3).ShouldEqual(13.33);
            //from meter(s) to millimeter(s)
            _measureService.ConvertDimension(10, measureDimension3, measureDimension4).ShouldEqual(10000);
            //from millimeter(s) to meter(s)
            _measureService.ConvertDimension(10000, measureDimension4, measureDimension3).ShouldEqual(10);
        }

        [Test]
        public void Can_convert_weight()
        {
            //from ounce(s) to lb(s)
            _measureService.ConvertWeight(11, measureWeight1, measureWeight2).ShouldEqual(0.69);
            //from lb(s) to ounce(s)
            _measureService.ConvertWeight(11, measureWeight2, measureWeight1).ShouldEqual(176);
            //from ounce(s) to  ounce(s)
            _measureService.ConvertWeight(13.333M, measureWeight1, measureWeight1).ShouldEqual(13.33);
            //from kg(s) to ounce(s)
            _measureService.ConvertWeight(11, measureWeight3, measureWeight1).ShouldEqual(388.01);
            //from kg(s) to gram(s)
            _measureService.ConvertWeight(10, measureWeight3, measureWeight4).ShouldEqual(10000);
        }
    }
}
