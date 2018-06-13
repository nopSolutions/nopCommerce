using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
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
            var serviceProvider = new Mock<IServiceProvider>();
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            serviceProvider.Setup(x => x.GetRequiredService(typeof(IHttpContextAccessor))).Returns(httpContextAccessor);
            //set up localziation service used by almost all validators
            var localizationService = new Mock<ILocalizationService>();
            _workContext = new Mock<IWorkContext>();
            _workContext.Setup(p => p.WorkingLanguage).Returns(new Language {Id = 1});
            localizationService.Setup(l => l.GetResource("")).Returns("Invalid");//.IgnoreArguments();
            serviceProvider.Setup(x => x.GetRequiredService(typeof(ILocalizationService))).Returns(_localizationService);
            serviceProvider.Setup(x => x.GetRequiredService(typeof(IWorkContext))).Returns(_workContext);
            nopEngine.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
            nopEngine.Setup(x => x.ResolveUnregistered(typeof(AddressValidator))).Returns(new AddressValidator(localizationService.Object));
            EngineContext.Replace(nopEngine.Object);
            _localizationService = localizationService.Object;
        }
    }
}
