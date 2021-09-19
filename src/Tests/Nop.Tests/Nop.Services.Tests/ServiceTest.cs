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
                PluginDescriptors = new List<PluginDescriptor>
                {
                    new PluginDescriptor(typeof(FixedRateTestTaxProvider).Assembly)
                    {
                        PluginType = typeof(FixedRateTestTaxProvider),
                        SystemName = "FixedTaxRateTest",
                        FriendlyName = "Fixed tax test rate provider",
                        Installed = true
                    },
                    new PluginDescriptor(typeof(FixedRateTestShippingRateComputationMethod).Assembly)
                    {
                        PluginType = typeof(FixedRateTestShippingRateComputationMethod),
                        SystemName = "FixedRateTestShippingRateComputationMethod",
                        FriendlyName = "Fixed rate test shipping computation method",
                        Installed = true
                    },
                    new PluginDescriptor(typeof(TestPaymentMethod).Assembly)
                    {
                        PluginType = typeof(TestPaymentMethod),
                        SystemName = "Payments.TestMethod",
                        FriendlyName = "Test payment method",
                        Installed = true
                    },
                    new PluginDescriptor(typeof(TestDiscountRequirementRule).Assembly)
                    {
                        PluginType = typeof(TestDiscountRequirementRule),
                        SystemName = "TestDiscountRequirementRule",
                        FriendlyName = "Test discount requirement rule",
                        Installed = true
                    },
                    new PluginDescriptor(typeof(TestExchangeRateProvider).Assembly)
                    {
                        PluginType = typeof(TestExchangeRateProvider),
                        SystemName = "CurrencyExchange.TestProvider",
                        FriendlyName = "Test exchange rate provider",
                        Installed = true
                    }
                }
            };
        }
    }
}
