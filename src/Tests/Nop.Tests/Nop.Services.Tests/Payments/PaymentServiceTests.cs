using AwesomeAssertions;
using Nop.Services.Payments;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Payments;

[TestFixture]
public class PaymentServiceTests : ServiceTest
{
    private IPaymentPluginManager _paymentPluginManager;

    [OneTimeSetUp]
    public void SetUp()
    {
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
}