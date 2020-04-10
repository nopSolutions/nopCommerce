using Moq;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Tests;
using Nop.Web.Areas.Admin.Validators.Common;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Validators
{
    [TestFixture]
    public abstract class BaseValidatorTests
    {
        protected ILocalizationService _localizationService;
        private Mock<INopDataProvider> _dataProvider;
        protected Mock<IWorkContext> _workContext;
        
        [SetUp]
        public void Setup()
        {
            var nopEngine = new Mock<NopEngine>();
            var serviceProvider = new TestServiceProvider();
            nopEngine.Setup(x => x.ServiceProvider).Returns(serviceProvider);
            _dataProvider = new Mock<INopDataProvider>();
            _localizationService = serviceProvider.LocalizationService.Object;
            nopEngine.Setup(x => x.ResolveUnregistered(typeof(AddressValidator))).Returns(new AddressValidator(_localizationService, _dataProvider.Object));
            EngineContext.Replace(nopEngine.Object);
        }
    }
}
