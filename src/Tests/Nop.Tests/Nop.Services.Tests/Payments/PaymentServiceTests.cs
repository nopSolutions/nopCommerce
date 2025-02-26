using FluentAssertions;
using Nop.Services.Payments;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Payments;

[TestFixture]
public class PaymentServiceTests : ServiceTest
{
    private IPaymentPluginManager _paymentPluginManager;
    private IPaymentService _paymentService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _paymentService = GetService<IPaymentService>();
        _paymentPluginManager = GetService<IPaymentPluginManager>();
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
}