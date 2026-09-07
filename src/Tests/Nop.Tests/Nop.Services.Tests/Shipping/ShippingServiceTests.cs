using AwesomeAssertions;
using Moq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Shipping;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class ShippingServiceTests : ServiceTest
{
    #region Fields

    private IShippingService _shippingService;
    private FragileShippingService _fragileShippingService;
    private IProductService _productService;

    #endregion

    #region Setup

    [OneTimeSetUp]
    public void SetUp()
    {
        _shippingService = GetService<IShippingService>();
        _productService = GetService<IProductService>();
        _fragileShippingService = GetService<FragileShippingService>();
    }

    #endregion



    [Test]
    public async Task GetShippingOptions_ReturnsFragileShippingOption_WhenCartContainsFragileProduct()
    {
        // Arrange
        var product1 = await _productService.GetProductBySkuAsync("AS_551_LP");
        var product2 = await _productService.GetProductBySkuAsync("FIRST_PRP");

        var shoppingCart = new List<ShoppingCartItem>
        {
            new ShoppingCartItem { AttributesXml = string.Empty, Quantity = 11, ProductId = product1.Id },
            new ShoppingCartItem { AttributesXml = string.Empty, Quantity = 1, ProductId = product2.Id }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService.Setup(x => x.GetProductByIdAsync(product1.Id))
            .ReturnsAsync(new Product { Id = product1.Id, Weight = 1m, IsFragile = true });
        mockProductService.Setup(x => x.GetProductByIdAsync(product2.Id))
            .ReturnsAsync(new Product { Id = product2.Id, Weight = 5m, IsFragile = false });

        var sut = new ShippingServiceDecorator(_shippingService, _fragileShippingService, mockProductService.Object);
        var address = new Address();

        // Act
        var shippingOptions = await sut.GetShippingOptionsAsync(shoppingCart, address);
        
        // Assert
        shippingOptions.ShippingOptions.Should().ContainSingle(o =>
            o.Name == "Fragile Shipping"
            && o.Rate == 1.1m
            && o.Description == "Specialized Oversize Courier");
    }

    [Test]
    public async Task GetShippingOptions_ReturnsStandardOptions_WhenContainsLightFragileProducts()
    {
        // Arrange
        var product1 = await _productService.GetProductBySkuAsync("AS_551_LP");
        var product2 = await _productService.GetProductBySkuAsync("FIRST_PRP");

        var shoppingCart = new List<ShoppingCartItem>
        {
            new ShoppingCartItem { AttributesXml = string.Empty, Quantity = 10, ProductId = product1.Id },
            new ShoppingCartItem { AttributesXml = string.Empty, Quantity = 1, ProductId = product2.Id }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService.Setup(x => x.GetProductByIdAsync(product1.Id))
            .ReturnsAsync(new Product { Id = product1.Id, Weight = 1m, IsFragile = false });
        mockProductService.Setup(x => x.GetProductByIdAsync(product2.Id))
            .ReturnsAsync(new Product { Id = product2.Id, Weight = 5m, IsFragile = false });

        var sut = new ShippingServiceDecorator(_shippingService, _fragileShippingService, mockProductService.Object);
        var address = new Address();

        // Act
        var shippingOptions = await sut.GetShippingOptionsAsync(shoppingCart, address);
        
        // Assert
        shippingOptions.ShippingOptions.Should().BeEmpty();
    }

    [Test]
    public async Task GetShippingOptions_ReturnsStandardOptions_WhenCartDoesNotContainFragileProduct()
    {
        // Arrange
        var product1 = await _productService.GetProductBySkuAsync("AS_551_LP");
        var product2 = await _productService.GetProductBySkuAsync("FIRST_PRP");
        var shoppingCart = new List<ShoppingCartItem>
        {
            new ShoppingCartItem { AttributesXml = string.Empty, Quantity = 11, ProductId = product1.Id },
            new ShoppingCartItem { AttributesXml = string.Empty, Quantity = 1, ProductId = product2.Id }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService.Setup(x => x.GetProductByIdAsync(product1.Id))
            .ReturnsAsync(new Product { Id = product1.Id, Weight = 1m, IsFragile = false });
        mockProductService.Setup(x => x.GetProductByIdAsync(product2.Id))
            .ReturnsAsync(new Product { Id = product2.Id, Weight = 5m, IsFragile = false });

        var sut = new ShippingServiceDecorator(_shippingService, _fragileShippingService, mockProductService.Object);
        var address = new Address();

        // Act
        var shippingOptions = await sut.GetShippingOptionsAsync(shoppingCart, address);
        
        // Assert
        shippingOptions.ShippingOptions.Should().BeEmpty();
    }
}