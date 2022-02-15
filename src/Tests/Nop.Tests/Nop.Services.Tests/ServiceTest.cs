using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Configuration;
using Nop.Services.Plugins;
using Nop.Tests.Nop.Services.Tests.Directory;
using Nop.Tests.Nop.Services.Tests.Discounts;
using Nop.Tests.Nop.Services.Tests.Payments;
using Nop.Tests.Nop.Services.Tests.Shipping;
using Nop.Tests.Nop.Services.Tests.Tax;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests
{
    [TestFixture]
    public abstract class ServiceTest : BaseNopTest
    {
        protected ServiceTest()
        {
            //init plugins
            InitPlugins();
        }

        private static void InitPlugins()
        {
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
            webHostEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
            CommonHelper.DefaultFileProvider = new NopFileProvider(webHostEnvironment.Object);
            
            Environment.SetEnvironmentVariable("ConnectionStrings", Singleton<DataConfig>.Instance.ConnectionString);

            Singleton<IPluginsInfo>.Instance = new PluginsInfo(CommonHelper.DefaultFileProvider)
            {
                PluginDescriptors = new List<(PluginDescriptor, bool)>
                {
                    (new PluginDescriptor
                    {
                        PluginType = typeof(FixedRateTestTaxProvider),
                        SystemName = "FixedTaxRateTest",
                        FriendlyName = "Fixed tax test rate provider",
                        Installed = true,
                        ReferencedAssembly = typeof(FixedRateTestTaxProvider).Assembly
                    }, true),
                    (new PluginDescriptor
                    {
                        PluginType = typeof(FixedRateTestShippingRateComputationMethod),
                        SystemName = "FixedRateTestShippingRateComputationMethod",
                        FriendlyName = "Fixed rate test shipping computation method",
                        Installed = true,
                        ReferencedAssembly = typeof(FixedRateTestShippingRateComputationMethod).Assembly
                    }, true),
                    (new PluginDescriptor
                    {
                        PluginType = typeof(TestPaymentMethod),
                        SystemName = "Payments.TestMethod",
                        FriendlyName = "Test payment method",
                        Installed = true,
                        ReferencedAssembly = typeof(TestPaymentMethod).Assembly
                    }, true),
                    (new PluginDescriptor
                    {
                        PluginType = typeof(TestDiscountRequirementRule),
                        SystemName = "TestDiscountRequirementRule",
                        FriendlyName = "Test discount requirement rule",
                        Installed = true,
                        ReferencedAssembly = typeof(TestDiscountRequirementRule).Assembly
                    }, true),
                    (new PluginDescriptor
                    {
                        PluginType = typeof(TestExchangeRateProvider),
                        SystemName = "CurrencyExchange.TestProvider",
                        FriendlyName = "Test exchange rate provider",
                        Installed = true,
                        ReferencedAssembly = typeof(TestExchangeRateProvider).Assembly
                    }, true)
                }
            };
        }
    }
}
