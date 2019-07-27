using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Logging;
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
        private IPaymentPluginManager _paymentPluginManager;
        private IPaymentService _paymentService;
        private CatalogSettings _catalogSettings;

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

            var customerService = new Mock<ICustomerService>();
            var loger = new Mock<ILogger>();
            var webHelper = new Mock<IWebHelper>();

            _catalogSettings = new CatalogSettings();
            var pluginService = new PluginService(_catalogSettings, customerService.Object, loger.Object, CommonHelper.DefaultFileProvider, webHelper.Object);

            _shoppingCartSettings = new ShoppingCartSettings();
            _settingService = new Mock<ISettingService>();
            _paymentPluginManager = new PaymentPluginManager(pluginService, _settingService.Object, _paymentSettings);
            _paymentService = new PaymentService(_paymentPluginManager, _paymentSettings, _shoppingCartSettings);
        }

        [Test]
        public void Can_load_paymentMethods()
        {
            var srcm = _paymentPluginManager.LoadAllPlugins();
            srcm.ShouldNotBeNull();
            srcm.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_load_paymentMethod_by_systemKeyword()
        {
            var srcm = _paymentPluginManager.LoadPluginBySystemName("Payments.TestMethod");
            srcm.ShouldNotBeNull();
        }

        [Test]
        public void Can_load_active_paymentMethods()
        {
            var srcm = _paymentPluginManager.LoadActivePlugins();
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

        [Test]
        public void Can_deserialize_empty_string()
        {
            var deserialized = _paymentService.DeserializeCustomValues(new Order { CustomValuesXml = string.Empty });

            deserialized.ShouldNotBeNull();
            deserialized.Count.ShouldEqual(0);
        }

        [Test]
        public void Can_deserialize_null_string()
        {
            var deserialized = _paymentService.DeserializeCustomValues(new Order { CustomValuesXml = null });

            deserialized.ShouldNotBeNull();
            deserialized.Count.ShouldEqual(0);
        }

        [Test]
        public void Can_serialize_and_deserialize_empty_CustomValues()
        {
            var processPaymentRequest = new ProcessPaymentRequest();
            var serializedXml = _paymentService.SerializeCustomValues(processPaymentRequest);
            var deserialized = _paymentService.DeserializeCustomValues(new Order { CustomValuesXml = serializedXml });

            deserialized.ShouldNotBeNull();
            deserialized.Count.ShouldEqual(0);
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

            deserialized.ShouldNotBeNull();
            deserialized.Count.ShouldEqual(4);

            deserialized.ContainsKey("key1").ShouldEqual(true);
            deserialized["key1"].ShouldEqual("value1");

            deserialized.ContainsKey("key2").ShouldEqual(true);
            //deserialized["key2"].ShouldEqual(null);
            deserialized["key2"].ShouldEqual("");

            deserialized.ContainsKey("key3").ShouldEqual(true);
            deserialized["key3"].ShouldEqual("3");

            deserialized.ContainsKey("<test key4>").ShouldEqual(true);
            deserialized["<test key4>"].ShouldEqual("<test value 4>");
        }
    }
}
