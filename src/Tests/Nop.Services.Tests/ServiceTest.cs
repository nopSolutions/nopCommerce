using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Tests.Directory;
using Nop.Services.Tests.Discounts;
using Nop.Services.Tests.Payments;
using Nop.Services.Tests.Shipping;
using Nop.Services.Tests.Tax;
using Nop.Tests;
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
            });
            plugins.Add(new PluginDescriptor(typeof(FixedRateTestShippingRateComputationMethod).Assembly,
                null, typeof(FixedRateTestShippingRateComputationMethod))
            {
                SystemName = "FixedRateTestShippingRateComputationMethod",
                FriendlyName = "Fixed rate test shipping computation method",
            });
            plugins.Add(new PluginDescriptor(typeof(TestPaymentMethod).Assembly,
                null, typeof(TestPaymentMethod))
            {
                SystemName = "Payments.TestMethod",
                FriendlyName = "Test payment method",
            });
            plugins.Add(new PluginDescriptor(typeof(TestDiscountRequirementRule).Assembly,
                null, typeof(TestDiscountRequirementRule))
            {
                SystemName = "TestDiscountRequirementRule",
                FriendlyName = "Test discount requirement rule",
            });
            plugins.Add(new PluginDescriptor(typeof(TestExchangeRateProvider).Assembly,
                null, typeof(TestExchangeRateProvider))
                {
                    SystemName = "CurrencyExchange.TestProvider",
                    FriendlyName = "Test exchange rate provider",
                });
            PluginManager.ReferencedPlugins = plugins;
        }
    }
}
