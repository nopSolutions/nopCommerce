using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Shipping;
using NUnit.Framework;

namespace Nop.Services.Tests.Shipping
{
    [TestFixture]
    public class ShippingServiceTests : ServiceTest
    {
        #region Fields

        private IShippingPluginManager _shippingPluginManager;
        private IShippingService _shippingService;
        private IProductService _productService;

        #endregion
        
        #region Setup

        [SetUp]
        public void SetUp()
        {
            _shippingPluginManager = GetService<IShippingPluginManager>();
            _shippingService = GetService<IShippingService>();
            _productService = GetService<IProductService>();
        } 

        #endregion

        [Test]
        public void CanLoadShippingRateComputationMethods()
        {
            var shippingRateComputationMethods = _shippingPluginManager.LoadAllPlugins();
            shippingRateComputationMethods.Should().NotBeNull();
            shippingRateComputationMethods.Any().Should().BeTrue();
        }

        [Test]
        public void CanLoadShippingRateComputationMethodBySystemKeyword()
        {
            var shippingRateComputationMethod = _shippingPluginManager.LoadPluginBySystemName("FixedRateTestShippingRateComputationMethod");
            shippingRateComputationMethod.Should().NotBeNull();
        }

        [Test]
        public void CanLoadActiveShippingRateComputationMethods()
        {
            var shippingRateComputationMethods = _shippingPluginManager.LoadActivePlugins(new List<string> { "FixedRateTestShippingRateComputationMethod" });
            shippingRateComputationMethods.Should().NotBeNull();
            shippingRateComputationMethods.Any().Should().BeTrue();
        }

        [Test]
        public void CanGetShoppingCartTotalWeightWithoutAttributes()
        {
            var product1 = _productService.GetProductBySku("AS_551_LP");
            var product2 = _productService.GetProductBySku("FIRST_PRP");

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

            _shippingService.GetTotalWeight(request).Should().Be(29M);
        }
    }
}
