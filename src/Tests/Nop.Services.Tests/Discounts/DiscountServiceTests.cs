using System;
using System.Linq;
using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Services.Discounts;
using NUnit.Framework;

namespace Nop.Services.Tests.Discounts
{
    [TestFixture]
    public class DiscountServiceTests : ServiceTest
    {
        private IDiscountPluginManager _discountPluginManager;
        private IDiscountService _discountService;

        [SetUp]
        public void SetUp()
        {
            _discountPluginManager = GetService<IDiscountPluginManager>();
            _discountService = GetService<IDiscountService>();
        }

        [Test]
        public void CanGetAllDiscount()
        {
            var discounts = _discountService.GetAllDiscounts();
            discounts.Should().NotBeNull();
            discounts.Any().Should().BeTrue();
        }

        [Test]
        public void CanLoadDiscountRequirementRules()
        {
            var rules = _discountPluginManager.LoadAllPlugins();
            rules.Should().NotBeNull();
            rules.Any().Should().BeTrue();
        }

        [Test]
        public void CanLoadDiscountRequirementRuleBySystemKeyword()
        {
            var rule = _discountPluginManager.LoadPluginBySystemName("TestDiscountRequirementRule");
            rule.Should().NotBeNull();
        }

        [Test]
        public void ShouldAcceptValidDiscountCode()
        {
            var discount = CreateDiscount();
            var customer = CreateCustomer();

            _discountService.ValidateDiscount(discount, customer, new[] { "CouponCode 1" }).IsValid.Should().BeTrue();
        }

        private static Customer CreateCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = string.Empty,
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }

        private static Discount CreateDiscount()
        {
            return new Discount
            {
                DiscountType = DiscountType.AssignedToSkus,
                Name = "Discount 2",
                UsePercentage = false,
                DiscountPercentage = 0,
                DiscountAmount = 5,
                RequiresCouponCode = true,
                CouponCode = "CouponCode 1",
                DiscountLimitation = DiscountLimitationType.Unlimited
            };
        }

        [Test]
        public void ShouldNotAcceptWrongDiscountCode()
        {
            var discount = CreateDiscount();
            var customer = CreateCustomer();

            _discountService.ValidateDiscount(discount, customer, new[] { "CouponCode 2" }).IsValid.Should().BeFalse();
        }

        [Test]
        public void CanValidateDiscountDateRange()
        {
            var discount = CreateDiscount();
            discount.RequiresCouponCode = false;

            var customer = CreateCustomer();

            _discountService.ValidateDiscount(discount, customer, null).IsValid.Should().BeTrue();

            discount.StartDateUtc = DateTime.UtcNow.AddDays(1);
            _discountService.ValidateDiscount(discount, customer, null).IsValid.Should().BeFalse();
        }

        [Test]
        public void CanCalculateDiscountAmountPercentage()
        {
            var discount = new Discount
            {
                UsePercentage = true,
                DiscountPercentage = 30
            };

            _discountService.GetDiscountAmount(discount, 100).Should().Be(30);

            discount.DiscountPercentage = 60;
            _discountService.GetDiscountAmount(discount, 200).Should().Be(120);
        }

        [Test]
        public void CanCalculateDiscountAmountFixed()
        {
            var discount = new Discount
            {
                UsePercentage = false,
                DiscountAmount = 10
            };

            _discountService.GetDiscountAmount(discount, 100).Should().Be(10);

            discount.DiscountAmount = 20;
            _discountService.GetDiscountAmount(discount, 200).Should().Be(20);
        }

        [Test]
        public void MaximumDiscountAmountIsUsed()
        {
            var discount = new Discount
            {
                UsePercentage = true,
                DiscountPercentage = 30,
                MaximumDiscountAmount = 3.4M
            };

            _discountService.GetDiscountAmount(discount, 100).Should().Be(3.4M);

            discount.DiscountPercentage = 60;
            _discountService.GetDiscountAmount(discount, 200).Should().Be(3.4M);
            _discountService.GetDiscountAmount(discount, 100).Should().Be(3.4M);

            discount.DiscountPercentage = 1;
            _discountService.GetDiscountAmount(discount, 200).Should().Be(2);
        }
    }
}