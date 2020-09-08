using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class PriceCalculationServiceTests : ServiceTest
    {
        #region Fields

        private ICustomerService _customerService;
        private IProductService _productService;
        private IPriceCalculationService _priceCalcService;

        #endregion

        #region SetUp

        [SetUp]
        public void SetUp()
        {
            _customerService = GetService<ICustomerService>();
            _productService = GetService<IProductService>();
            _priceCalcService = GetService<IPriceCalculationService>();
        }

        #endregion

        #region Tests

        [Test]
        public void CanGetFinalProductPrice()
        {
            var product = _productService.GetProductBySku("BP_20_WSP");

            //customer
            var customer = new Customer();

            _priceCalcService.GetFinalPrice(product, customer, 0, false).Should().Be(79.99M);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 2).Should().Be(19M);
        }

        [Test]
        public void CanGetFinalProductPriceWithTierPrices()
        {
            var product = _productService.GetProductBySku("BP_20_WSP");

            //customer
            var customer = new Customer();

            _priceCalcService.GetFinalPrice(product, customer, 0, false).Should().Be(79.99M);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 2).Should().Be(19);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 3).Should().Be(19);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 5).Should().Be(17);
            _priceCalcService.GetFinalPrice(product, customer, 0, false, 7).Should().Be(17);
        }

        [Test]
        public void CanGetFinalProductPriceWithTierPricesByCustomerRole()
        {
            var product = _productService.GetProductBySku("NK_ZSJ_MM");

            //customer
            var customer = _customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail);
            var customerRole = _customerService.GetAllCustomerRoles().FirstOrDefault();

            customerRole.Should().NotBeNull();

            var tierPrices = new List<TierPrice>
            {
                new TierPrice { CustomerRoleId = customerRole.Id, ProductId = product.Id, Quantity = 2, Price = 25 },
                new TierPrice { CustomerRoleId = customerRole.Id, ProductId = product.Id, Quantity = 5, Price = 20 },
                new TierPrice { CustomerRoleId = customerRole.Id, ProductId = product.Id, Quantity = 10, Price = 15 }
            };

            foreach (var tierPrice in tierPrices) 
                _productService.InsertTierPrice(tierPrice);

            product.HasTierPrices = true;

            var rez1 = _priceCalcService.GetFinalPrice(product, customer, 0, false);
            var rez2 = _priceCalcService.GetFinalPrice(product, customer, 0, false, 2);
            var rez3 = _priceCalcService.GetFinalPrice(product, customer, 0, false, 3);
            var rez4 = _priceCalcService.GetFinalPrice(product, customer, 0, false, 5);
            var rez5 = _priceCalcService.GetFinalPrice(product, customer, 0, false, 10);
            var rez6 = _priceCalcService.GetFinalPrice(product, customer, 0, false, 15);

            foreach (var tierPrice in tierPrices)
                _productService.DeleteTierPrice(tierPrice);

            rez1.Should().Be(30M);
            rez2.Should().Be(25);
            rez3.Should().Be(25);
            rez4.Should().Be(20);
            rez5.Should().Be(15);
            rez6.Should().Be(15);
        }

        [Test]
        public void CanGetFinalProductPriceWithAdditionalFee()
        {
            var product = _productService.GetProductBySku("BP_20_WSP");

            //customer
            var customer = new Customer();

            _priceCalcService.GetFinalPrice(product, customer, 5, false).Should().Be(84.99M);
        }

        [Test]
        public void CanGetFinalProductPriceWithDiscount()
        {
            var product = _productService.GetProductBySku("BP_20_WSP");
            var customer = _customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail);

            var mapping = new DiscountProductMapping
            {
                DiscountId = 1,
                EntityId = product.Id
            };

            _productService.InsertDiscountProductMapping(mapping);
            _customerService.ApplyDiscountCouponCode(customer, "123");

            //set HasDiscountsApplied property
            product.HasDiscountsApplied = true;
           
            var finalPrice = _priceCalcService.GetFinalPrice(product, customer);

            _productService.DeleteDiscountProductMapping(mapping);
            _customerService.RemoveDiscountCouponCode(customer, "123");

            finalPrice.Should().Be(69.99M);
        }

        [TestCase(12.366, 12.37, RoundingType.Rounding001)]
        [TestCase(12.363, 12.36, RoundingType.Rounding001)]
        [TestCase(12.000, 12.00, RoundingType.Rounding001)]
        [TestCase(12.001, 12.00, RoundingType.Rounding001)]
        [TestCase(12.34, 12.35, RoundingType.Rounding005Up)]
        [TestCase(12.36, 12.40, RoundingType.Rounding005Up)]
        [TestCase(12.35, 12.35, RoundingType.Rounding005Up)]
        [TestCase(12.00, 12.00, RoundingType.Rounding005Up)]
        [TestCase(12.05, 12.05, RoundingType.Rounding005Up)]
        [TestCase(12.20, 12.20, RoundingType.Rounding005Up)]
        [TestCase(12.001, 12.00, RoundingType.Rounding005Up)]
        [TestCase(12.34, 12.30, RoundingType.Rounding005Down)]
        [TestCase(12.36, 12.35, RoundingType.Rounding005Down)]
        [TestCase(12.00, 12.00, RoundingType.Rounding005Down)]
        [TestCase(12.05, 12.05, RoundingType.Rounding005Down)]
        [TestCase(12.20, 12.20, RoundingType.Rounding005Down)]
        [TestCase(12.35, 12.40, RoundingType.Rounding01Up)]
        [TestCase(12.36, 12.40, RoundingType.Rounding01Up)]
        [TestCase(12.00, 12.00, RoundingType.Rounding01Up)]
        [TestCase(12.10, 12.10, RoundingType.Rounding01Up)]
        [TestCase(12.35, 12.30, RoundingType.Rounding01Down)]
        [TestCase(12.36, 12.40, RoundingType.Rounding01Down)]
        [TestCase(12.00, 12.00, RoundingType.Rounding01Down)]
        [TestCase(12.10, 12.10, RoundingType.Rounding01Down)]
        [TestCase(12.24, 12.00, RoundingType.Rounding05)]
        [TestCase(12.49, 12.50, RoundingType.Rounding05)]
        [TestCase(12.74, 12.50, RoundingType.Rounding05)]
        [TestCase(12.99, 13.00, RoundingType.Rounding05)]
        [TestCase(12.00, 12.00, RoundingType.Rounding05)]
        [TestCase(12.50, 12.50, RoundingType.Rounding05)]
        [TestCase(12.49, 12.00, RoundingType.Rounding1)]
        [TestCase(12.50, 13.00, RoundingType.Rounding1)]
        [TestCase(12.00, 12.00, RoundingType.Rounding1)]
        [TestCase(12.01, 13.00, RoundingType.Rounding1Up)]
        [TestCase(12.99, 13.00, RoundingType.Rounding1Up)]
        [TestCase(12.00, 12.00, RoundingType.Rounding1Up)]
        public void CanRound(decimal valueToRounding, decimal roundedValue, RoundingType roundingType)
        {
            _priceCalcService.Round(valueToRounding, roundingType).Should().Be(roundedValue);
        }

        #endregion
    }
}