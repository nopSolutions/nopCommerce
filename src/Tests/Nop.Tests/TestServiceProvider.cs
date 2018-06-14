using Moq;
using Nop.Core;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Seo;

namespace Nop.Tests
{
    public class TestServiceProvider : IServiceProvider
    {
        private readonly Mock<ICurrencyService> _currencyService;

        public TestServiceProvider()
        {
            LocalizationService = new Mock<ILocalizationService>();
            GenericAttributeService = new Mock<IGenericAttributeService>();
            WorkContext = new Mock<IWorkContext>();

            LocalizationService.Setup(l => l.GetResource(It.IsAny<string>())).Returns("Invalid");
            WorkContext.Setup(p => p.WorkingLanguage).Returns(new Language {Id = 1});
            WorkContext.Setup(w => w.WorkingCurrency).Returns(new Currency { RoundingType = RoundingType.Rounding001 });

            _currencyService = new Mock<ICurrencyService>();
            _currencyService.Setup(x => x.GetCurrencyById(1, true)).Returns(new Currency {Id = 1, RoundingTypeId = 0});

            GenericAttributeService.Setup(p => p.GetAttributesForEntity(1, "Customer"))
                .Returns(new List<GenericAttribute>
                {
                    new GenericAttribute
                    {
                        EntityId = 1,
                        Key = "manufacturer-advanced-mode",
                        KeyGroup = "Customer",
                        StoreId = 0,
                        Value = "true"
                    }
                });
        }

        public Mock<ILocalizationService> LocalizationService { get; }
        public Mock<IWorkContext> WorkContext { get; }
        public Mock<IGenericAttributeService> GenericAttributeService { get; }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IHttpContextAccessor))
                return new Mock<IHttpContextAccessor>().Object;

            if (serviceType == typeof(ILocalizationService))
                return LocalizationService.Object;

            if (serviceType == typeof(IWorkContext))
                return WorkContext.Object;

            if (serviceType == typeof(CurrencySettings))
                return new CurrencySettings {PrimaryStoreCurrencyId = 1};

            if (serviceType == typeof(ICurrencyService))
                return _currencyService.Object;

            if (serviceType == typeof(IUrlRecordService))
                return new Mock<IUrlRecordService>().Object;

            if (serviceType == typeof(IGenericAttributeService))
                return GenericAttributeService.Object;

            return null;
        }
    }
}
