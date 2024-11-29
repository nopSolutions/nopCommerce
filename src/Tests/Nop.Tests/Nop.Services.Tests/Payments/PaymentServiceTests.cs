using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.Services.Payments;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Payments;

[TestFixture]
public class PaymentServiceTests : ServiceTest
{
    private IPaymentPluginManager _paymentPluginManager;
    private IPaymentService _paymentService;
    private IHttpContextAccessor _httpContextAccessor;

    [OneTimeSetUp]
    public void SetUp()
    {
        _paymentService = GetService<IPaymentService>();
        _paymentPluginManager = GetService<IPaymentPluginManager>();
        _httpContextAccessor = GetService<IHttpContextAccessor>();
    }

    [Test]
    public async Task CanGenerateOrderGuid()
    {
        var request = new ProcessPaymentRequest();
        request.OrderGuid.Should().Be(Guid.Empty);
        await _paymentService.GenerateOrderGuidAsync(request);
        request.OrderGuid.Should().NotBe(Guid.Empty);
        var oldGuid = request.OrderGuid;
        await _paymentService.GenerateOrderGuidAsync(request);
        request.OrderGuid.Should().Be(oldGuid);
        request = new ProcessPaymentRequest();
        await _paymentService.GenerateOrderGuidAsync(request);
        request.OrderGuid.Should().NotBe(oldGuid);
        oldGuid = request.OrderGuid;
        ArgumentNullException.ThrowIfNull(_httpContextAccessor.HttpContext);
        await _httpContextAccessor.HttpContext.Session.SetAsync("OrderPaymentInfo", request);
        await _paymentService.GenerateOrderGuidAsync(request);
        request.OrderGuid.Should().Be(oldGuid);
        request = new ProcessPaymentRequest();
        await _paymentService.GenerateOrderGuidAsync(request);
        request.OrderGuid.Should().Be(oldGuid);
    }

    [Test]
    public async Task CanLoadPaymentMethods()
    {
        var paymentMethods = await _paymentPluginManager.LoadAllPluginsAsync();
        paymentMethods.Should().NotBeNull();
    }

    [Test]
    public async Task CanLoadPaymentMethodBySystemKeyword()
    {
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync("Payments.TestMethod");
        paymentMethod.Should().NotBeNull();
    }

    [Test]
    public async Task CanLoadActivePaymentMethods()
    {
        var paymentMethods = await _paymentPluginManager.LoadActivePluginsAsync(["Payments.TestMethod"]);
        paymentMethods.Should().NotBeNull();
        paymentMethods.Any().Should().BeTrue();
    }

    [Test]
    public void CanGetMaskedCreditCardNumber()
    {
        _paymentService.GetMaskedCreditCardNumber(string.Empty).Should().Be(string.Empty);
        _paymentService.GetMaskedCreditCardNumber("123").Should().Be("123");
        _paymentService.GetMaskedCreditCardNumber("1234567890123456").Should().Be("************3456");
    }

    [Test]
    public void CanDeserializeEmptyString()
    {
        var deserialized = _paymentService.DeserializeCustomValues(new Order { CustomValuesXml = string.Empty });

        deserialized.Should().NotBeNull();
        deserialized.Count.Should().Be(0);
    }

    [Test]
    public void CanDeserializeNullString()
    {
        var deserialized = _paymentService.DeserializeCustomValues(new Order { CustomValuesXml = null });

        deserialized.Should().NotBeNull();
        deserialized.Count.Should().Be(0);
    }

    [Test]
    public void CanSerializeAndDeserializeEmptyCustomValues()
    {
        var processPaymentRequest = new ProcessPaymentRequest();
        var serializedXml = _paymentService.SerializeCustomValues(processPaymentRequest);
        var deserialized = _paymentService.DeserializeCustomValues(new Order { CustomValuesXml = serializedXml });

        deserialized.Should().NotBeNull();
        deserialized.Count.Should().Be(0);
    }

    [Test]
    public void CanSerializeAndDeserializeCustomValues()
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
        deserialized["key2"].Should().Be(string.Empty);

        deserialized.ContainsKey("key3").Should().BeTrue();
        deserialized["key3"].Should().Be("3");

        deserialized.ContainsKey("<test key4>").Should().BeTrue();
        deserialized["<test key4>"].Should().Be("<test value 4>");
    }
}