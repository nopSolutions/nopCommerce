using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Catalog
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

        [OneTimeSetUp]
        public void SetUp()
        {
            _customerService = GetService<ICustomerService>();
            _productService = GetService<IProductService>();
            _priceCalcService = GetService<IPriceCalculationService>();
        }

        #endregion
        
        #region Tests

        [Test]
        public async Task CanGetFinalProductPrice()
        {
            var product = await _productService.GetProductBySkuAsync("BP_20_WSP");

            //customer
            var customer = new Customer();

            var (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false);
            finalPrice.Should().Be(79.99M);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
            
            (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false, 2);
            finalPrice.Should().Be(19M);
            finalPriceWithoutDiscounts.Should().Be(finalPriceWithoutDiscounts);
        }

        [Test]
        public async Task CanGetFinalProductPriceWithTierPrices()
        {
            var product = await _productService.GetProductBySkuAsync("BP_20_WSP");

            //customer
            var customer = new Customer();

            var (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false);
            finalPrice.Should().Be(79.99M);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
            (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false, 2);
            finalPrice.Should().Be(19);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
            (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false, 3);
            finalPrice.Should().Be(19);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
            (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false, 5);
            finalPrice.Should().Be(17);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
            (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false, 7);
            finalPrice.Should().Be(17);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
        }

        [Test]
        public async Task CanGetFinalProductPriceWithTierPricesByCustomerRole()
        {
            var product = await _productService.GetProductBySkuAsync("NK_ZSJ_MM");

            //customer
            var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
            var roles = await _customerService.GetAllCustomerRolesAsync();
            var customerRole = roles.FirstOrDefault();

            customerRole.Should().NotBeNull();

            var tierPrices = new List<TierPrice>
            {
                new TierPrice { CustomerRoleId = customerRole.Id, ProductId = product.Id, Quantity = 2, Price = 25 },
                new TierPrice { CustomerRoleId = customerRole.Id, ProductId = product.Id, Quantity = 5, Price = 20 },
                new TierPrice { CustomerRoleId = customerRole.Id, ProductId = product.Id, Quantity = 10, Price = 15 }
            };

            foreach (var tierPrice in tierPrices) 
                await _productService.InsertTierPriceAsync(tierPrice);

            product.HasTierPrices = true;

            var (rezWithoutDiscount1, rez1, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false);
            var (rezWithoutDiscount2, rez2, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false, 2);
            var (rezWithoutDiscount3, rez3, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false, 3);
            var (rezWithoutDiscount4, rez4, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false, 5);
            var (rezWithoutDiscount5, rez5, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false, 10);
            var (rezWithoutDiscount6, rez6, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 0, false, 15);

            foreach (var tierPrice in tierPrices)
                await _productService.DeleteTierPriceAsync(tierPrice);

            rez1.Should().Be(30M);
            rez2.Should().Be(25);
            rez3.Should().Be(25);
            rez4.Should().Be(20);
            rez5.Should().Be(15);
            rez6.Should().Be(15);

            rez1.Should().Be(rezWithoutDiscount1);
            rez2.Should().Be(rezWithoutDiscount2);
            rez3.Should().Be(rezWithoutDiscount3);
            rez4.Should().Be(rezWithoutDiscount4);
            rez5.Should().Be(rezWithoutDiscount5);
            rez6.Should().Be(rezWithoutDiscount6);
        }

        [Test]
        public async Task CanGetFinalProductPriceWithAdditionalFee()
        {
            var product = await _productService.GetProductBySkuAsync("BP_20_WSP");

            //customer
            var customer = new Customer();

            var (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, 5, false);
            finalPrice.Should().Be(84.99M);
            finalPrice.Should().Be(finalPriceWithoutDiscounts);
        }

        [Test]
        public async Task CanGetFinalProductPriceWithDiscount()
        {
            var product = await _productService.GetProductBySkuAsync("BP_20_WSP");
            var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);

            var mapping = new DiscountProductMapping
            {
                DiscountId = 1,
                EntityId = product.Id
            };

            await _productService.InsertDiscountProductMappingAsync(mapping);
            await _customerService.ApplyDiscountCouponCodeAsync(customer, "123");

            //set HasDiscountsApplied property
            product.HasDiscountsApplied = true;
           
            var (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer);

            await _productService.DeleteDiscountProductMappingAsync(mapping);
            await _customerService.RemoveDiscountCouponCodeAsync(customer, "123");

            finalPrice.Should().Be(69.99M);
            finalPriceWithoutDiscounts.Should().Be(79.99M);
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