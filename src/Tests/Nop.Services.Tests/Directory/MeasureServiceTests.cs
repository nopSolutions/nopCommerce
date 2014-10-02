using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Directory
{
    [TestFixture]
    public class MeasureServiceTests : ServiceTest
    {
        private IRepository<MeasureDimension> _measureDimensionRepository;
        private IRepository<MeasureWeight> _measureWeightRepository;
        private MeasureSettings _measureSettings;
        private IEventPublisher _eventPublisher;
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
                DisplayOrder = 1,
            };
            measureDimension2 = new MeasureDimension
            {
                Id = 2,
                Name = "feet",
                SystemKeyword = "feet",
                Ratio = 0.08333333M,
                DisplayOrder = 2,
            };
            measureDimension3 = new MeasureDimension
            {
                Id = 3,
                Name = "meter(s)",
                SystemKeyword = "meters",
                Ratio = 0.0254M,
                DisplayOrder = 3,
            };
            measureDimension4 = new MeasureDimension
            {
                Id = 4,
                Name = "millimetre(s)",
                SystemKeyword = "millimetres",
                Ratio = 25.4M,
                DisplayOrder = 4,
            };



            measureWeight1 = new MeasureWeight
            {
                Id = 1,
                Name = "ounce(s)",
                SystemKeyword = "ounce",
                Ratio = 16M,
                DisplayOrder = 1,
            };
            measureWeight2 = new MeasureWeight
            {
                Id = 2,
                Name = "lb(s)",
                SystemKeyword = "lb",
                Ratio = 1M,
                DisplayOrder = 2,
            };
            measureWeight3 = new MeasureWeight
            {
                Id = 3,
                Name = "kg(s)",
                SystemKeyword = "kg",
                Ratio = 0.45359237M,
                DisplayOrder = 3,
            };
            measureWeight4 = new MeasureWeight
            {
                Id = 4,
                Name = "gram(s)",
                SystemKeyword = "grams",
                Ratio = 453.59237M,
                DisplayOrder = 4,
            };

            _measureDimensionRepository = MockRepository.GenerateMock<IRepository<MeasureDimension>>();
            _measureDimensionRepository.Expect(x => x.Table).Return(new List<MeasureDimension> { measureDimension1, measureDimension2, measureDimension3, measureDimension4 }.AsQueryable());
            _measureDimensionRepository.Expect(x => x.GetById(measureDimension1.Id)).Return(measureDimension1);
            _measureDimensionRepository.Expect(x => x.GetById(measureDimension2.Id)).Return(measureDimension2);
            _measureDimensionRepository.Expect(x => x.GetById(measureDimension3.Id)).Return(measureDimension3);
            _measureDimensionRepository.Expect(x => x.GetById(measureDimension4.Id)).Return(measureDimension4);

            _measureWeightRepository = MockRepository.GenerateMock<IRepository<MeasureWeight>>();
            _measureWeightRepository.Expect(x => x.Table).Return(new List<MeasureWeight> { measureWeight1, measureWeight2, measureWeight3, measureWeight4 }.AsQueryable());
            _measureWeightRepository.Expect(x => x.GetById(measureWeight1.Id)).Return(measureWeight1);
            _measureWeightRepository.Expect(x => x.GetById(measureWeight2.Id)).Return(measureWeight2);
            _measureWeightRepository.Expect(x => x.GetById(measureWeight3.Id)).Return(measureWeight3);
            _measureWeightRepository.Expect(x => x.GetById(measureWeight4.Id)).Return(measureWeight4);


            var cacheManager = new NopNullCache();

            _measureSettings = new MeasureSettings();
            _measureSettings.BaseDimensionId = measureDimension1.Id; //inch(es)
            _measureSettings.BaseWeightId = measureWeight2.Id; //lb(s)

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            _measureService = new MeasureService(cacheManager,
                _measureDimensionRepository,
                _measureWeightRepository,
                _measureSettings, _eventPublisher);
        }

        [Test]
        public void Can_convert_dimension()
        {
            //from meter(s) to feet
            _measureService.ConvertDimension(10, measureDimension3, measureDimension2, true).ShouldEqual(32.81);
            //from inch(es) to meter(s)
            _measureService.ConvertDimension(10, measureDimension1, measureDimension3, true).ShouldEqual(0.25);
            //from meter(s) to meter(s)
            _measureService.ConvertDimension(13.333M, measureDimension3, measureDimension3, true).ShouldEqual(13.33);
            //from meter(s) to millimeter(s)
            _measureService.ConvertDimension(10, measureDimension3, measureDimension4, true).ShouldEqual(10000);
            //from millimeter(s) to meter(s)
            _measureService.ConvertDimension(10000, measureDimension4, measureDimension3, true).ShouldEqual(10);
        }

        [Test]
        public void Can_convert_weight()
        {
            //from ounce(s) to lb(s)
            _measureService.ConvertWeight(11, measureWeight1, measureWeight2, true).ShouldEqual(0.69);
            //from lb(s) to ounce(s)
            _measureService.ConvertWeight(11, measureWeight2, measureWeight1, true).ShouldEqual(176);
            //from ounce(s) to  ounce(s)
            _measureService.ConvertWeight(13.333M, measureWeight1, measureWeight1, true).ShouldEqual(13.33);
            //from kg(s) to ounce(s)
            _measureService.ConvertWeight(11, measureWeight3, measureWeight1, true).ShouldEqual(388.01);
            //from kg(s) to gram(s)
            _measureService.ConvertWeight(10, measureWeight3, measureWeight4, true).ShouldEqual(10000);
        }
    }
}
