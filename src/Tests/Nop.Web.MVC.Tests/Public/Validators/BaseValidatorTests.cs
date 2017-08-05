using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Web.MVC.Tests.Public.Validators
{
    [TestFixture]
    public abstract class BaseValidatorTests
    {
        protected ILocalizationService _localizationService;
        
        [SetUp]
        public void Setup()
        {
            var nopEngine = MockRepository.GenerateMock<NopEngine>();
            var serviceProvider = MockRepository.GenerateMock<IServiceProvider>();
            var httpContextAccessor = MockRepository.GenerateMock<IHttpContextAccessor>();
            serviceProvider.Expect(x => x.GetRequiredService(typeof(IHttpContextAccessor))).Return(httpContextAccessor);
            //set up localziation service used by almost all validators
            _localizationService = MockRepository.GenerateMock<ILocalizationService>();
            _localizationService.Expect(l => l.GetResource("")).Return("Invalid").IgnoreArguments();
            serviceProvider.Expect(x => x.GetRequiredService(typeof(ILocalizationService))).Return(_localizationService);
            nopEngine.Expect(x => x.ServiceProvider).Return(serviceProvider);
            EngineContext.Replace(nopEngine);
        }
    }
}
