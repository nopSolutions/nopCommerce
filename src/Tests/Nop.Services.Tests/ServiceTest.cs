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
            var plugins = new List<PluginDescriptor>();
            plugins.Add(new PluginDescriptor(typeof(FixedRateTestTaxProvider).Assembly,
                null, typeof(FixedRateTestTaxProvider))
            {
                SystemName = "FixedTaxRateTest",
                FriendlyName = "Fixed tax test rate provider",
                Installed = true,
            });
            plugins.Add(new PluginDescriptor(typeof(FixedRateTestShippingRateComputationMethod).Assembly,
                null, typeof(FixedRateTestShippingRateComputationMethod))
            {
                SystemName = "FixedRateTestShippingRateComputationMethod",
                FriendlyName = "Fixed rate test shipping computation method",
                Installed = true,
            });
            plugins.Add(new PluginDescriptor(typeof(TestPaymentMethod).Assembly,
                null, typeof(TestPaymentMethod))
            {
                SystemName = "Payments.TestMethod",
                FriendlyName = "Test payment method",
                Installed = true,
            });
            plugins.Add(new PluginDescriptor(typeof(TestDiscountRequirementRule).Assembly,
                null, typeof(TestDiscountRequirementRule))
            {
                SystemName = "TestDiscountRequirementRule",
                FriendlyName = "Test discount requirement rule",
                Installed = true,
            });
            plugins.Add(new PluginDescriptor(typeof(TestExchangeRateProvider).Assembly,
                null, typeof(TestExchangeRateProvider))
                {
                    SystemName = "CurrencyExchange.TestProvider",
                    FriendlyName = "Test exchange rate provider",
                    Installed = true,
                });
            PluginManager.ReferencedPlugins = plugins;
        }
    }
}
