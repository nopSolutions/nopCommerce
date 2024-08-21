using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Shipping;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class ShippingServiceTests : ServiceTest
{
    #region Fields

    private IShippingPluginManager _shippingPluginManager;
    private IShippingService _shippingService;
    private IProductService _productService;

    #endregion

    #region Setup

    [OneTimeSetUp]
    public void SetUp()
    {
        _shippingPluginManager = GetService<IShippingPluginManager>();
        _shippingService = GetService<IShippingService>();
        _productService = GetService<IProductService>();
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

    [Test]
    public async Task CanGetShoppingCartTotalWeightWithoutAttributes()
    {
        var product1 = await _productService.GetProductBySkuAsync("AS_551_LP");
        var product2 = await _productService.GetProductBySkuAsync("FIRST_PRP");

        var request = new GetShippingOptionRequest
        {
            Items =
            {
                new GetShippingOptionRequest.PackageItem(
                    new ShoppingCartItem { AttributesXml = string.Empty, Quantity = 3, ProductId = product1.Id },
                    product1),
                new GetShippingOptionRequest.PackageItem(
                    new ShoppingCartItem { AttributesXml = string.Empty, Quantity = 4, ProductId = product2.Id },
                    product2)
            }
        };

        var totalWeight = await _shippingService.GetTotalWeightAsync(request);
        totalWeight.Should().Be(29M);
    }
}