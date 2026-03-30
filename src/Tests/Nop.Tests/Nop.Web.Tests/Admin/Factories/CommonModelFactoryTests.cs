using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Mvc.Routing;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Factories;

[TestFixture]
public class CommonModelFactoryTests : BaseNopTest
{
    private TestCommonModelFactory _commonModelFactory;

    [OneTimeSetUp]
    public void SetUp()
    {
        _commonModelFactory = (TestCommonModelFactory)GetService<ICommonModelFactory>();
    }

    [Test]
    public async Task TestGetNopLatestVersion()
    {
        var nopLatestVersion = await _commonModelFactory.GetNopLatestVersionAsync();
        nopLatestVersion.Should().NotBeNullOrEmpty();
    }

    public class TestCommonModelFactory : CommonModelFactory
    {
        public new async Task<string> GetNopLatestVersionAsync()
        {
            return await base.GetNopLatestVersionAsync();
        }

        public TestCommonModelFactory(AppSettings appSettings, CatalogSettings catalogSettings, CurrencySettings currencySettings, IAuthenticationPluginManager authenticationPluginManager, IBaseAdminModelFactory baseAdminModelFactory, IBlogService blogService, ICategoryService categoryService, ICurrencyService currencyService, ICustomerService customerService, IDateTimeHelper dateTimeHelper, IEventPublisher eventPublisher, IExchangeRatePluginManager exchangeRatePluginManager, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILanguageService languageService, ILocalizationService localizationService, ILogger logger, IMaintenanceService maintenanceService, IManufacturerService manufacturerService, IMeasureService measureService, IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager, INopDataProvider dataProvider, INopFileProvider fileProvider, INopUrlHelper nopUrlHelper, IOrderService orderService, IPaymentPluginManager paymentPluginManager, IPickupPluginManager pickupPluginManager, IPluginService pluginService, IProductService productService, IReturnRequestService returnRequestService, ISearchTermService searchTermService, IServiceCollection serviceCollection, IShippingPluginManager shippingPluginManager, IStaticCacheManager staticCacheManager, IStoreContext storeContext, IStoreService storeService, ITaxPluginManager taxPluginManager, IThumbService thumbService, ITopicService topicService, IUrlRecordService urlRecordService, IWebHelper webHelper, IWidgetPluginManager widgetPluginManager, IWorkContext workContext, LinkGenerator linkGenerator, MeasureSettings measureSettings, NopHttpClient nopHttpClient, ProxySettings proxySettings) : base(appSettings, catalogSettings, currencySettings, authenticationPluginManager, baseAdminModelFactory, blogService, categoryService, currencyService, customerService, dateTimeHelper, eventPublisher, exchangeRatePluginManager, httpClientFactory, httpContextAccessor, languageService, localizationService, logger, maintenanceService, manufacturerService, measureService, multiFactorAuthenticationPluginManager, dataProvider, fileProvider, nopUrlHelper, orderService, paymentPluginManager, pickupPluginManager, pluginService, productService, returnRequestService, searchTermService, serviceCollection, shippingPluginManager, staticCacheManager, storeContext, storeService, taxPluginManager, thumbService, topicService, urlRecordService, webHelper, widgetPluginManager, workContext, linkGenerator, measureSettings, nopHttpClient, proxySettings)
        {
        }
    }
}
