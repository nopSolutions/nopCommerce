using Autofac;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Catalog;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class RoundingHelperTests : ServiceTest
    {
        private IWorkContext _workContext;

        [SetUp]
        public new void SetUp()
        {
            _workContext = MockRepository.GenerateMock<IWorkContext>();
            _workContext.Expect(w => w.WorkingCurrency).Return(new Currency { RoundingType = RoundingType.Rounding001 });
            
            var nopEngine = MockRepository.GenerateMock<NopEngine>();
            var containe = MockRepository.GenerateMock<IContainer>();
            var containerManager = MockRepository.GenerateMock<ContainerManager>(containe);
            nopEngine.Expect(x => x.ContainerManager).Return(containerManager);
            containerManager.Expect(x => x.Resolve<IWorkContext>()).Return(_workContext);
            EngineContext.Replace(nopEngine);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            EngineContext.Replace(null);
        }

        [TestCase(12.366, 12.37)]
        [TestCase(12.363, 12.36)]
        public void can_round_price(decimal price, decimal roundedValue)
        {
            RoundingHelper.RoundPrice(price).ShouldEqual(roundedValue);
        }

        [TestCase(12.366, 12.37, RoundingType.Rounding001)]
        [TestCase(12.363, 12.36, RoundingType.Rounding001)]
        [TestCase(12.34, 12.35, RoundingType.Rounding005Up)]
        [TestCase(12.36, 12.40, RoundingType.Rounding005Up)]
        [TestCase(12.34, 12.30, RoundingType.Rounding005Down)]
        [TestCase(12.36, 12.35, RoundingType.Rounding005Down)]
        [TestCase(12.35, 12.40, RoundingType.Rounding01Up)]
        [TestCase(12.36, 12.40, RoundingType.Rounding01Up)]
        [TestCase(12.35, 12.30, RoundingType.Rounding01Down)]
        [TestCase(12.36, 12.40, RoundingType.Rounding01Down)]
        [TestCase(12.24, 12.00, RoundingType.Rounding05)]
        [TestCase(12.49, 12.50, RoundingType.Rounding05)]
        [TestCase(12.74, 12.50, RoundingType.Rounding05)]
        [TestCase(12.99, 13.00, RoundingType.Rounding05)]
        [TestCase(12.49, 12.00, RoundingType.Rounding1)]
        [TestCase(12.50, 13.00, RoundingType.Rounding1)]
        [TestCase(12.01, 13.00, RoundingType.Rounding1Up)]
        [TestCase(12.99, 13.00, RoundingType.Rounding1Up)]
        public void can_round(decimal valueToRoundig, decimal roundedValue, RoundingType roundingType)
        {
            valueToRoundig.Round(roundingType).ShouldEqual(roundedValue);
        }
    }
}
