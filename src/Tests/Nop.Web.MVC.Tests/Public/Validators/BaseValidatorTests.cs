using Moq;
using Nop.Core;
using Nop.Core.Infrastructure;
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
        protected Mock<IWorkContext> _workContext;
        
        [SetUp]
        public void Setup()
        {
            var nopEngine = new Mock<NopEngine>();
            var serviceProvider = new TestServiceProvider();
            nopEngine.Setup(x => x.ServiceProvider).Returns(serviceProvider);
            nopEngine.Setup(x => x.ResolveUnregistered(typeof(AddressValidator))).Returns(new AddressValidator(serviceProvider.LocalizationService.Object));
            EngineContext.Replace(nopEngine.Object);
            _localizationService = serviceProvider.LocalizationService.Object;
        }
    }
}
