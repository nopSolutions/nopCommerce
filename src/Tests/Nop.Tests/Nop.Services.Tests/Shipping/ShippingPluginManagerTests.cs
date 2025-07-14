using FluentAssertions;
using Nop.Services.Shipping;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class ShippingPluginManagerTests : ServiceTest
{
    #region Fields

    private IShippingPluginManager _shippingPluginManager;

    #endregion

    #region Setup

    [OneTimeSetUp]
    public void SetUp()
    {
        _shippingPluginManager = GetService<IShippingPluginManager>();
    }

    #endregion

    [Test]
    public async Task CanLoadShippingRateComputationMethods()
    {
        var shippingRateComputationMethods = await _shippingPluginManager.LoadAllPluginsAsync();
        shippingRateComputationMethods.Should().NotBeNull();
        shippingRateComputationMethods.Any().Should().BeTrue();
    }

    [Test]
    public async Task CanLoadShippingRateComputationMethodBySystemKeyword()
    {
        var shippingRateComputationMethod = await _shippingPluginManager.LoadPluginBySystemNameAsync("FixedRateTestShippingRateComputationMethod");
        shippingRateComputationMethod.Should().NotBeNull();
    }

    [Test]
    public async Task CanLoadActiveShippingRateComputationMethods()
    {
        var shippingRateComputationMethods = await _shippingPluginManager.LoadActivePluginsAsync(["FixedRateTestShippingRateComputationMethod"]);
        shippingRateComputationMethods.Should().NotBeNull();
        shippingRateComputationMethods.Any().Should().BeTrue();
    }
}