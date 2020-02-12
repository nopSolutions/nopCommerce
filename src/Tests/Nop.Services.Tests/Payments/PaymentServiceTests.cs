using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Payments;
using Nop.Services.Tests.FakeServices;
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
        private IPaymentPluginManager _paymentPluginManager;
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

            var pluginService = new FakePluginService();

            _shoppingCartSettings = new ShoppingCartSettings();
            _settingService = new Mock<ISettingService>();
            _paymentPluginManager = new PaymentPluginManager(pluginService, _settingService.Object, _paymentSettings);
            _paymentService = new PaymentService(_paymentPluginManager, _paymentSettings, _shoppingCartSettings);
        }

        [Test]
        public void Can_load_paymentMethods()
        {
            var srcm = _paymentPluginManager.LoadAllPlugins();
            srcm.Should().NotBeNull();
            srcm.Any().Should().BeTrue();
        }

        [Test]
        public void Can_load_paymentMethod_by_systemKeyword()
        {
            var srcm = _paymentPluginManager.LoadPluginBySystemName("Payments.TestMethod");
            srcm.Should().NotBeNull();
        }

        [Test]
        public void Can_load_active_paymentMethods()
        {
            var srcm = _paymentPluginManager.LoadActivePlugins();
            srcm.Should().NotBeNull();
            srcm.Any().Should().BeTrue();
        }

        [Test]
        public void Can_get_masked_credit_card_number()
        {
            _paymentService.GetMaskedCreditCardNumber("").Should().Be("");
            _paymentService.GetMaskedCreditCardNumber("123").Should().Be("123");
            _paymentService.GetMaskedCreditCardNumber("1234567890123456").Should().Be("************3456");
        }

        [Test]
        public void Can_deserialize_empty_string()
        {
            var deserialized = _paymentService.DeserializeCustomValues(new Order { CustomValuesXml = string.Empty });

            deserialized.Should().NotBeNull();
            deserialized.Count.Should().Be(0);
        }

        [Test]
        public void Can_deserialize_null_string()
        {
            var deserialized = _paymentService.DeserializeCustomValues(new Order { CustomValuesXml = null });

            deserialized.Should().NotBeNull();
            deserialized.Count.Should().Be(0);
        }

        [Test]
        public void Can_serialize_and_deserialize_empty_CustomValues()
        {
            var processPaymentRequest = new ProcessPaymentRequest();
            var serializedXml = _paymentService.SerializeCustomValues(processPaymentRequest);
            var deserialized = _paymentService.DeserializeCustomValues(new Order { CustomValuesXml = serializedXml });

            deserialized.Should().NotBeNull();
            deserialized.Count.Should().Be(0);
        }

        [Test]
        public void Can_serialize_and_deserialize_CustomValues()
        {
            var processPaymentRequest = new ProcessPaymentRequest();
            processPaymentRequest.CustomValues.Add("key1", "value1");
            processPaymentRequest.CustomValues.Add("key2", null);
            processPaymentRequest.CustomValues.Add("key3", 3);
            processPaymentRequest.CustomValues.Add("<test key4>", "<test value 4>");
            var serializedXml = _paymentService.SerializeCustomValues(processPaymentRequest);
            var deserialized = _paymentService.DeserializeCustomValues(new Order { CustomValuesXml = serializedXml });

            deserialized.Should().NotBeNull();
            deserialized.Count.Should().Be(4);

            deserialized.ContainsKey("key1").Should().BeTrue();
            deserialized["key1"].Should().Be("value1");

            deserialized.ContainsKey("key2").Should().BeTrue();
            //deserialized["key2"].Should().Be(null);
            deserialized["key2"].Should().Be("");

            deserialized.ContainsKey("key3").Should().BeTrue();
            deserialized["key3"].Should().Be("3");

            deserialized.ContainsKey("<test key4>").Should().BeTrue();
            deserialized["<test key4>"].Should().Be("<test value 4>");
        }
    }
}
