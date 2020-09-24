using System;
using System.Collections.Generic;
using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Shipping;
using NUnit.Framework;

namespace Nop.Services.Tests.Shipping
{
    [TestFixture]
    public class CalculateDimensionsTests : ServiceTest
    {
        private IProductService _productService;
        private IShippingService _shippingService;

        [SetUp]
        public void SetUp()
        {
            _productService = GetService<IProductService>();
            _shippingService = GetService<IShippingService>();
        }

        [Test]
        public void ShouldReturnZeroWithAllZeroDimensions()
        {
            var product = _productService.GetProductBySku("VG_CR_025");
            product.Length = 0;
            product.Width = 0;
            product.Height = 0;

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                    {
                        Quantity = 1,
                        ProductId = product.Id
                    }, product)
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);

            length.Should().Be(0);
            width.Should().Be(0);
            height.Should().Be(0);
            
            items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                    {
                        Quantity = 2,
                        ProductId = product.Id
                    }, product)
            };

            _shippingService.GetDimensions(items, out width, out length, out height);

            length.Should().Be(0);
            width.Should().Be(0);
            height.Should().Be(0);
        }

        [Test]
        public void CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod()
        {
            var product = _productService.GetProductBySku("AP_MBP_13");
            product.Length = 3;
            product.Width = 2;
            product.Height = 2;

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 1,
                    ProductId = product.Id
                }, product)
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);

            length.Should().Be(3);
            width.Should().Be(2);
            height.Should().Be(2);
        }

        [Test]
        public void CanCalculateWithSingleItemAndQty2()
        {
            var product = _productService.GetProductBySku("AP_MBP_13");
            product.Length = 2;
            product.Width = 4;
            product.Height = 4;

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 2,
                    ProductId = product.Id
                }, product)
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);

            length.Should().Be(4);
            width.Should().Be(4);
            height.Should().Be(4);
        }

        [Test]
        public void CanCalculateWithCubicItemAndMultipleQty()
        {
            var product = _productService.GetProductBySku("AP_MBP_13");
            product.Length = 2;
            product.Width = 2;
            product.Height = 2;

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 3,
                    ProductId = product.Id
                }, product)
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);

            Math.Round(length, 2).Should().Be(2.88M);
            Math.Round(width, 2).Should().Be(2.88M);
            Math.Round(height, 2).Should().Be(2.88M);
        }

        [Test]
        public void CanCalculateWithMultipleItems()
        {
            var product1 = _productService.GetProductBySku("AP_MBP_13");
            product1.Length = 2;
            product1.Width = 2;
            product1.Height = 2;

            var product2 = _productService.GetProductBySku("VG_CR_025");
            product2.Length = 3;
            product2.Width = 5;
            product2.Height = 2;

            _productService.InsertProduct(product2);

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                                {
                                    Quantity = 3,
                                    ProductId = product1.Id
                                }, product1),
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                                {
                                    Quantity = 1,
                                    ProductId = product2.Id
                                }, product2)
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);

            Math.Round(length, 2).Should().Be(3.78M);
            Math.Round(width, 2).Should().Be(5);    //preserve max width
            Math.Round(height, 2).Should().Be(3.78M);

            items.Clear();
            //take 8 cubes of 1x1x1 which is "packed" as 2x2x2 
            for (var i = 0; i < 8; i++)
            {
                var product = _productService.GetProductById(i + 1);
                product.Length = 1;
                product.Width = 1;
                product.Height = 1;

                items.Add(new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 1,
                    ProductId = product.Id
                }, product));
            }

            _shippingService.GetDimensions(items, out width, out length, out height);

            Math.Round(length, 2).Should().Be(2);
            Math.Round(width, 2).Should().Be(2);
            Math.Round(height, 2).Should().Be(2);
        }
    }
}
