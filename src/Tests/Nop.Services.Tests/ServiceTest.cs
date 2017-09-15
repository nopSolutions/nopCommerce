using System.Collections.Generic;
using Nop.Core.Plugins;
using Nop.Services.Tests.Directory;
using Nop.Services.Tests.Discounts;
using Nop.Services.Tests.Payments;
using Nop.Services.Tests.Shipping;
using Nop.Services.Tests.Tax;
using NUnit.Framework;

namespace Nop.Services.Tests
{
    [TestFixture]
    public abstract class ServiceTest
    {
        [SetUp]
        public void SetUp()
        {
            //init plugins
            InitPlugins();
        }

        private void InitPlugins()
        {
            PluginManager.ReferencedPlugins = new List<PluginDescriptor>
            {
                new PluginDescriptor(typeof(FixedRateTestTaxProvider).Assembly)
                {
                    PluginType = typeof(FixedRateTestTaxProvider),
                    SystemName = "FixedTaxRateTest",
                    FriendlyName = "Fixed tax test rate provider",
                    Installed = true,
                },
                new PluginDescriptor(typeof(FixedRateTestShippingRateComputationMethod).Assembly)
                {
                    PluginType = typeof(FixedRateTestShippingRateComputationMethod),
                    SystemName = "FixedRateTestShippingRateComputationMethod",
                    FriendlyName = "Fixed rate test shipping computation method",
                    Installed = true,
                },
                new PluginDescriptor(typeof(TestPaymentMethod).Assembly)
                {
                    PluginType = typeof(TestPaymentMethod),
                    SystemName = "Payments.TestMethod",
                    FriendlyName = "Test payment method",
                    Installed = true,
                },
                new PluginDescriptor(typeof(TestDiscountRequirementRule).Assembly)
                {
                    PluginType = typeof(TestDiscountRequirementRule),
                    SystemName = "TestDiscountRequirementRule",
                    FriendlyName = "Test discount requirement rule",
                    Installed = true,
                },
                new PluginDescriptor(typeof(TestExchangeRateProvider).Assembly)
                {
                    PluginType = typeof(TestExchangeRateProvider),
                    SystemName = "CurrencyExchange.TestProvider",
                    FriendlyName = "Test exchange rate provider",
                    Installed = true,
                }
            };
        }
    }
}
