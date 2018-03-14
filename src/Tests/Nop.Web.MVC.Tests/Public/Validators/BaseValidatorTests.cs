using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Validators.Common;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Web.MVC.Tests.Public.Validators
{
    [TestFixture]
    public abstract class BaseValidatorTests
    {
        protected ILocalizationService _localizationService;
        protected IWorkContext _workContext;
        
        [SetUp]
        public void Setup()
        {
            var nopEngine = MockRepository.GenerateMock<NopEngine>();
            var serviceProvider = MockRepository.GenerateMock<IServiceProvider>();
            var httpContextAccessor = MockRepository.GenerateMock<IHttpContextAccessor>();
            serviceProvider.Expect(x => x.GetRequiredService(typeof(IHttpContextAccessor))).Return(httpContextAccessor);
            //set up localziation service used by almost all validators
            _localizationService = MockRepository.GenerateMock<ILocalizationService>();
            _workContext = MockRepository.GenerateMock<IWorkContext>();
            _workContext.Expect(p => p.WorkingLanguage).Return(new Language {Id = 1});
            _localizationService.Expect(l => l.GetResource("")).Return("Invalid").IgnoreArguments();
            serviceProvider.Expect(x => x.GetRequiredService(typeof(ILocalizationService))).Return(_localizationService);
            serviceProvider.Expect(x => x.GetRequiredService(typeof(IWorkContext))).Return(_workContext);
            nopEngine.Expect(x => x.ServiceProvider).Return(serviceProvider);
            nopEngine.Expect(x => x.ResolveUnregistered(typeof(AddressValidator))).Return(new AddressValidator(_localizationService));
            EngineContext.Replace(nopEngine);
        }
    }
}
