using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Tests;
using NUnit.Framework;


namespace Nop.Services.Tests.Payments
{
    [TestFixture]
    public class PaymentServiceTests : ServiceTest
    {
        private PaymentSettings _paymentSettings;
        private ShoppingCartSettings _shoppingCartSettings;
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<ISettingService> _settingService;
        private IPaymentService _paymentService;
        
        [SetUp]
        public new void SetUp()
        {
            _paymentSettings = new PaymentSettings
            {
                ActivePaymentMethodSystemNames = new List<string>()
            };
            _paymentSettings.ActivePaymentMethodSystemNames.Add("Payments.TestMethod");

            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            var pluginFinder = new PluginFinder(_eventPublisher.Object);

            _shoppingCartSettings = new ShoppingCartSettings();
            _settingService = new Mock<ISettingService>();

            _paymentService = new PaymentService(_paymentSettings, pluginFinder, _settingService.Object, _shoppingCartSettings);
        }

        [Test]
        public void Can_load_paymentMethods()
        {
            var srcm = _paymentService.LoadAllPaymentMethods();
            srcm.ShouldNotBeNull();
            srcm.Any().ShouldBeTrue();
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
            srcm.Any().ShouldBeTrue();
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
