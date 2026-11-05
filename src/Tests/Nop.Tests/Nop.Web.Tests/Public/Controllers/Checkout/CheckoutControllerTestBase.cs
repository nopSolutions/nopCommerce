using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Tests.Nop.Services.Tests;
using Nop.Tests.Nop.Services.Tests.Payments;
using Nop.Tests.Nop.Services.Tests.Shipping;
using Nop.Tests.Nop.Services.Tests.Tax;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Controllers.Checkout;

/// <summary>
/// Base class for tests that call <see cref="CheckoutController"/> actions directly (DI controller harness).
/// </summary>
/// <remarks>
/// The controller is built from the shared <see cref="BaseNopTest"/> registrations on the in-memory SQLite store
/// (see <see cref="CreateControllerServiceProvider"/> for why a per-controller provider is used), so every action body and every controller helper runs against real services as the test customer
/// (the admin returned by <see cref="BaseNopTest.TestAuthenticationService"/>).
///
/// Checkout dependencies: the base derives from <see cref="ServiceTest"/> (not <see cref="WebTest"/>) because
/// ServiceTest already registers the test plugins checkout needs: the payment method "Payments.TestMethod"
/// (TestPaymentMethod), the shipping rate computation method "FixedRateTestShippingRateComputationMethod"
/// and the pickup point provider "PickupPoint.TestProvider". ServiceTest and WebTest both replace the global
/// plugin list when a fixture is constructed, so [SetUp] installs the checkout plugin list (those test plugins plus
/// the tax provider "FixedTaxRateTest" and <see cref="SingleOptionTestShippingRateComputationMethod"/>, the only
/// plugin added for checkout tests) and [TearDown] puts the previous list back.
///
/// Shared state: the store, ServiceProvider and HttpContext are shared by the whole test run, so every test
/// gets a fresh <see cref="CheckoutTestHelpers"/> in [SetUp] that snapshots settings, the customer's cart and
/// the customer's generic attributes, and restores them in [TearDown].
///
/// Known gaps: no routing, no model binding, no filters and no view engine. Rendered partials and view
/// components are replaced by marker strings (see <see cref="TestableCheckoutController"/>).
/// </remarks>
[TestFixture]
public abstract class CheckoutControllerTestBase : ServiceTest
{
    /// <summary>
    /// Arrange/restore helpers for the current test
    /// </summary>
    protected CheckoutTestHelpers Checkout { get; private set; }

    private IPluginsInfo _previousPluginsInfo;

    static CheckoutControllerTestBase()
    {
        //register the type converters the application registers at startup (BaseNopTest registers only the list ones);
        //checkout stores ShippingOption, List<ShippingOption> and PickupPoint values in generic attributes through them;
        //the registration is process-wide and never undone, so fixtures that run after any checkout fixture see these
        //converters too (harmless, it matches production startup, but results of such fixtures may depend on run order)
        new TypeConverterRegistrationStartUpTask().Execute();
    }

    [SetUp]
    public async Task CheckoutControllerSetUpAsync()
    {
        _previousPluginsInfo = Singleton<IPluginsInfo>.Instance;
        Singleton<IPluginsInfo>.Instance = CreateCheckoutPluginsInfo();

        Checkout = new CheckoutTestHelpers(ServiceProvider);
        await Checkout.CaptureStateAsync();
    }

    [TearDown]
    public async Task CheckoutControllerTearDownAsync()
    {
        try
        {
            if (Checkout != null)
                await Checkout.RestoreStateAsync();

            Checkout = null;
        }
        finally
        {
            Singleton<IPluginsInfo>.Instance = _previousPluginsInfo;
            _previousPluginsInfo = null;
        }
    }

    /// <summary>
    /// The plugin list checkout tests need; a plugin still has to be active in the settings to be used
    /// </summary>
    private static PluginsInfo CreateCheckoutPluginsInfo()
    {
        static (PluginDescriptor, bool) Descriptor(Type pluginType, string systemName) =>
            (new PluginDescriptor
            {
                PluginType = pluginType,
                SystemName = systemName,
                FriendlyName = systemName,
                Installed = true,
                ReferencedAssembly = pluginType.Assembly
            }, true);

        return new PluginsInfo(CommonHelper.DefaultFileProvider)
        {
            PluginDescriptors = new List<(PluginDescriptor, bool)>
            {
                Descriptor(typeof(FixedRateTestTaxProvider), "FixedTaxRateTest"),
                Descriptor(typeof(FixedRateTestShippingRateComputationMethod), FixedRateShippingMethodSystemName),
                Descriptor(typeof(SingleOptionTestShippingRateComputationMethod), SingleOptionTestShippingRateComputationMethod.SystemName),
                Descriptor(typeof(TestPaymentMethod), TestPaymentMethodSystemName),
                Descriptor(typeof(PickupPointTestProvider), PickupPointProviderSystemName)
            }
        };
    }

    /// <summary>
    /// System name of the test shipping rate computation method that offers "Shipping option 1" and "Shipping option 2"
    /// </summary>
    protected const string FixedRateShippingMethodSystemName = "FixedRateTestShippingRateComputationMethod";

    /// <summary>
    /// System name of the test payment method
    /// </summary>
    protected const string TestPaymentMethodSystemName = "Payments.TestMethod";

    /// <summary>
    /// System name of the test pickup point provider (pickup points "1", "2" and "3")
    /// </summary>
    protected const string PickupPointProviderSystemName = "PickupPoint.TestProvider";

    /// <summary>
    /// Create a service provider that builds the controller's transient dependency graph
    /// and takes every singleton and scoped service from the shared <see cref="BaseNopTest"/> provider.
    /// </summary>
    /// <remarks>
    /// Resolving the controller's deep, all-transient graph (e.g. ICheckoutModelFactory) repeatedly from the shared
    /// provider makes the container switch to its compiled (IL-emitted) resolvers, and building the graph with them
    /// overflows the test worker stack, also in other fixtures that resolve the same services afterwards.
    /// A fresh provider per controller resolves each service once, so it stays on the runtime resolver
    /// and leaves the shared provider untouched. Singleton and scoped services are forwarded to the shared provider,
    /// so caches, the data provider, the work context and the store context are the same instances
    /// tests arrange through (in BaseNopTest the root provider plays the role of the request scope).
    /// The provider is not disposed: it would dispose the forwarded shared instances.
    /// </remarks>
    private IServiceProvider CreateControllerServiceProvider()
    {
        var root = ServiceProvider;
        var rootServices = GetService<IServiceCollection>();

        var sharedRegistrationCounts = rootServices
            .Where(descriptor => descriptor.Lifetime != ServiceLifetime.Transient && !descriptor.IsKeyedService)
            .GroupBy(descriptor => descriptor.ServiceType)
            .ToDictionary(group => group.Key, group => group.Count());

        IServiceCollection services = new ServiceCollection();

        foreach (var descriptor in rootServices)
        {
            var copyAsIs = descriptor.Lifetime == ServiceLifetime.Transient ||
                           descriptor.IsKeyedService ||
                           descriptor.ServiceType.IsGenericTypeDefinition ||
                           descriptor.ImplementationInstance != null ||
                           sharedRegistrationCounts[descriptor.ServiceType] > 1;

            if (copyAsIs)
            {
                services.Add(descriptor);
                continue;
            }

            var serviceType = descriptor.ServiceType;
            services.AddSingleton(serviceType, _ => root.GetRequiredService(serviceType));
        }

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Resolve a service from a fresh per-call provider (see <see cref="CreateControllerServiceProvider"/>),
    /// for services with a deep transient graph such as IOrderProcessingService
    /// </summary>
    protected T ResolveFresh<T>()
    {
        return CreateControllerServiceProvider().GetRequiredService<T>();
    }

    /// <summary>
    /// Create a fresh checkout controller. Call it after arranging settings,
    /// because the controller receives the settings objects when it is created.
    /// </summary>
    protected TestableCheckoutController CreateController()
    {
        var httpContext = GetService<IHttpContextAccessor>().HttpContext;

        var controller = ActivatorUtilities.CreateInstance<TestableCheckoutController>(CreateControllerServiceProvider());

        controller.ControllerContext = new ControllerContext(new ActionContext(httpContext,
            httpContext.GetRouteData() ?? new RouteData(),
            new ControllerActionDescriptor { ControllerName = "Checkout" }));
        controller.Url = GetService<IUrlHelperFactory>().GetUrlHelper(controller.ControllerContext);
        controller.TempData = GetService<ITempDataDictionaryFactory>().GetTempData(httpContext);

        return controller;
    }

    /// <summary>
    /// Test-only checkout controller. It overrides only the two rendering helpers of BaseController
    /// and returns marker strings instead of rendered HTML; every action and helper runs as in production.
    /// </summary>
    public class TestableCheckoutController : CheckoutController
    {
        public TestableCheckoutController(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
            ICheckoutModelFactory checkoutModelFactory,
            ICountryService countryService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWebHelper webHelper,
            IWorkContext workContext,
            OrderSettings orderSettings,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings) : base(addressSettings,
            captchaSettings,
            customerSettings,
            addressModelFactory,
            addressService,
            addressAttributeParser,
            checkoutModelFactory,
            countryService,
            customerService,
            genericAttributeService,
            localizationService,
            logger,
            orderProcessingService,
            orderService,
            paymentPluginManager,
            paymentService,
            productService,
            shippingService,
            shoppingCartService,
            storeContext,
            taxService,
            webHelper,
            workContext,
            orderSettings,
            paymentSettings,
            rewardPointsSettings,
            shippingSettings,
            taxSettings)
        {
        }

        protected override Task<string> RenderPartialViewToStringAsync(string viewName, object model)
        {
            return Task.FromResult($"<partial:{viewName}>");
        }

        protected override Task<string> RenderViewComponentToStringAsync(Type componentType, object arguments = null)
        {
            return Task.FromResult($"<component:{componentType.Name}>");
        }
    }
}
