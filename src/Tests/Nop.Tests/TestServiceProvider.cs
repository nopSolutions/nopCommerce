using Moq;
using Nop.Core;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Seo;

namespace Nop.Tests
{
    public class TestServiceProvider : IServiceProvider
    {
        public TestServiceProvider()
        {
            LocalizationService = new Mock<ILocalizationService>();
            GenericAttributeService = new Mock<IGenericAttributeService>();
            WorkContext = new Mock<IWorkContext>();
            
            PriceCalculationService = new PriceCalculationService(new CatalogSettings(), new CurrencySettings(), 
                new Mock<ICategoryService>().Object, new Mock<ICurrencyService>().Object, new Mock<IDiscountService>().Object,
                new Mock<IManufacturerService>().Object, new Mock<IProductAttributeParser>().Object,
                new Mock<IProductService>().Object, new MemoryCacheManager(new Mock<IMemoryCache>().Object), new Mock<IStoreContext>().Object, WorkContext.Object, new ShoppingCartSettings());

            LocalizationService.Setup(l => l.GetResource(It.IsAny<string>())).Returns("Invalid");
            WorkContext.Setup(p => p.WorkingLanguage).Returns(new Language {Id = 1});
            WorkContext.Setup(w => w.WorkingCurrency).Returns(new Currency { RoundingType = RoundingType.Rounding001 });

            CurrencyService = new Mock<ICurrencyService>();
            CurrencyService.Setup(x => x.GetCurrencyById(1, true)).Returns(new Currency {Id = 1, RoundingTypeId = 0});

            GenericAttributeService.Setup(p => p.GetAttribute<bool>(It.IsAny<Customer>(), "product-advanced-mode", It.IsAny<int>()))
                .Returns(true);

            GenericAttributeService.Setup(p => p.GetAttribute<bool>(It.IsAny<Customer>(), "manufacturer-advanced-mode", It.IsAny<int>()))
                .Returns(true);

            GenericAttributeService.Setup(p => p.GetAttribute<bool>(It.IsAny<Customer>(), "category-advanced-mode", It.IsAny<int>()))
                .Returns(true);
        }

        public Mock<ILocalizationService> LocalizationService { get; }
        public Mock<IWorkContext> WorkContext { get; }
        public Mock<IGenericAttributeService> GenericAttributeService { get; }
        public IPriceCalculationService PriceCalculationService { get; }
        public Mock<ICurrencyService> CurrencyService { get; }

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
                return CurrencyService.Object;

            if (serviceType == typeof(IUrlRecordService))
                return new Mock<IUrlRecordService>().Object;

            if (serviceType == typeof(IGenericAttributeService))
                return GenericAttributeService.Object;

            if (serviceType == typeof(IPriceCalculationService))
                return PriceCalculationService;

            return null;
        }
    }
}
