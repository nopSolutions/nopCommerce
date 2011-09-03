using System.Collections.Generic;
using Nop.Core.Domain.Payments;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Payments;
using Nop.Tests;
using NUnit.Framework;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Tests.Payments
{
    [TestFixture]
    public class PaymentServiceTests : ServiceTest
    {
        PaymentSettings _paymentSettings;
        ShoppingCartSettings _shoppingCartSettings;
        IPaymentService _paymentService;
        
        [SetUp]
        public new void SetUp()
        {
            _paymentSettings = new PaymentSettings();
            _paymentSettings.ActivePaymentMethodSystemNames = new List<string>();
            _paymentSettings.ActivePaymentMethodSystemNames.Add("Payments.TestMethod");

            var pluginFinder = new PluginFinder(new AppDomainTypeFinder());

            _shoppingCartSettings = new ShoppingCartSettings();

            _paymentService = new PaymentService(_paymentSettings, pluginFinder, _shoppingCartSettings);
        }

        [Test]
        public void Can_load_paymentMethods()
        {
            var srcm = _paymentService.LoadActivePaymentMethods();
            srcm.ShouldNotBeNull();
            (srcm.Count > 0).ShouldBeTrue();
        }

        [Test]
        public void Can_load_paymentMethod_by_systemKeyword()
        {
            var srcm = _paymentService.LoadPaymentMethodBySystemName("Payments.TestMethod");
            srcm.ShouldNotBeNull();
        }

        [Test]
        public void Can_load_active_paymentMethods()
        {
            var srcm = _paymentService.LoadActivePaymentMethods();
            srcm.ShouldNotBeNull();
            (srcm.Count > 0).ShouldBeTrue();
        }

        [Test]
        public void Can_get_masked_credit_card_number()
        {
            _paymentService.GetMaskedCreditCardNumber("").ShouldEqual("");
            _paymentService.GetMaskedCreditCardNumber("123").ShouldEqual("123");
            _paymentService.GetMaskedCreditCardNumber("1234567890123456").ShouldEqual("************3456");
        }
    }
}
