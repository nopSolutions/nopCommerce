using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Discounts
{
    [TestFixture]
    public class DiscountServiceTests : ServiceTest
    {

        private readonly Mock<ILocalizationService> _localizationService = new Mock<ILocalizationService>();
        private IDiscountPluginManager _discountPluginManager;
        private IDiscountService _discountService;
        private FakeRepository<Discount> _discountRepo;
        private readonly Mock<IStoreContext> _storeContext = new Mock<IStoreContext>();
        private readonly Mock<ICustomerService> _customerService = new Mock<ICustomerService>();
        private readonly Mock<IProductService> _productService = new Mock<IProductService>();
        private readonly FakeRepository<DiscountRequirement> _discountRequirementRepo = new FakeRepository<DiscountRequirement>();
        private readonly FakeRepository<DiscountUsageHistory> _discountUsageHistoryRepo = new FakeRepository<DiscountUsageHistory>();
        private readonly FakeRepository<Order> _orderRepo = new FakeRepository<Order>();

        [SetUp]
        public new void SetUp()
        {
            var discount1 = new Discount
            {
                Id = 1,
                DiscountType = DiscountType.AssignedToCategories,
                Name = "Discount 1",
                UsePercentage = true,
                DiscountPercentage = 10,
                DiscountAmount = 0,
                DiscountLimitation = DiscountLimitationType.Unlimited,
                LimitationTimes = 0
            };
            var discount2 = new Discount
            {
                Id = 2,
                DiscountType = DiscountType.AssignedToSkus,
                Name = "Discount 2",
                UsePercentage = false,
                DiscountPercentage = 0,
                DiscountAmount = 5,
                RequiresCouponCode = true,
                CouponCode = "SecretCode",
                DiscountLimitation = DiscountLimitationType.NTimesPerCustomer,
                LimitationTimes = 3
            };

            _discountRepo = new FakeRepository<Discount>(new List<Discount> { discount1, discount2 });

            var staticCacheManager = new TestCacheManager();
            
            var pluginService = new FakePluginService();

            _discountPluginManager = new DiscountPluginManager(new Mock<ICustomerService>().Object, pluginService);
            _discountService = new DiscountService(
                _customerService.Object,
                _discountPluginManager,
                _localizationService.Object,
                _productService.Object,
                _discountRepo.GetRepository(),
                _discountRequirementRepo,
                _discountUsageHistoryRepo,
                _orderRepo,
                staticCacheManager,
                _storeContext.Object);
        }

        [Test]
        public void Can_get_all_discount()
        {
            RunWithTestServiceProvider(() =>
            {
                var discounts = _discountService.GetAllDiscounts();
                discounts.Should().NotBeNull();
                discounts.Any().Should().BeTrue();
            });
        }

        [Test]
        public void Can_load_discountRequirementRules()
        {
            RunWithTestServiceProvider(() =>
            {
                var rules = _discountPluginManager.LoadAllPlugins();
                rules.Should().NotBeNull();
                rules.Any().Should().BeTrue();
            });
        }

        [Test]
        public void Can_load_discountRequirementRuleBySystemKeyword()
        {
            RunWithTestServiceProvider(() =>
            {
                var rule = _discountPluginManager.LoadPluginBySystemName("TestDiscountRequirementRule");
                rule.Should().NotBeNull();
            });
        }

        [Test]
        public void Should_accept_valid_discount_code()
        {
            var discount = new Discount
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

            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };

            _discountService.ValidateDiscount(discount, customer, new[] { "CouponCode 1" }).IsValid.Should().BeTrue();
        }


        [Test]
        public void Should_not_accept_wrong_discount_code()
        {
            var discount = new Discount
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

            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
            _discountService.ValidateDiscount(discount, customer, new[] { "CouponCode 2" }).IsValid.Should().BeFalse();
        }

        [Test]
        public void Can_validate_discount_dateRange()
        {
            var discount = new Discount
            {
                DiscountType = DiscountType.AssignedToSkus,
                Name = "Discount 2",
                UsePercentage = false,
                DiscountPercentage = 0,
                DiscountAmount = 5,
                StartDateUtc = DateTime.UtcNow.AddDays(-1),
                EndDateUtc = DateTime.UtcNow.AddDays(1),
                RequiresCouponCode = false,
                DiscountLimitation = DiscountLimitationType.Unlimited
            };

            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
            _discountService.ValidateDiscount(discount, customer, null).IsValid.Should().BeTrue();

            discount.StartDateUtc = DateTime.UtcNow.AddDays(1);
            _discountService.ValidateDiscount(discount, customer, null).IsValid.Should().BeFalse();
        }

        [Test]
        public void Can_calculate_discount_amount_percentage()
        {
            var discount = new Discount
            {
                UsePercentage = true,
                DiscountPercentage = 30
            };

            discount.GetDiscountAmount(100).Should().Be(30);

            discount.DiscountPercentage = 60;
            discount.GetDiscountAmount(200).Should().Be(120);
        }

        [Test]
        public void Can_calculate_discount_amount_fixed()
        {
            var discount = new Discount
            {
                UsePercentage = false,
                DiscountAmount = 10
            };

            discount.GetDiscountAmount(100).Should().Be(10);

            discount.DiscountAmount = 20;
            discount.GetDiscountAmount(200).Should().Be(20);
        }

        [Test]
        public void Maximum_discount_amount_is_used()
        {
            var discount = new Discount
            {
                UsePercentage = true,
                DiscountPercentage = 30,
                MaximumDiscountAmount = 3.4M
            };

            discount.GetDiscountAmount(100).Should().Be(3.4M);

            discount.DiscountPercentage = 60;
            discount.GetDiscountAmount(200).Should().Be(3.4M);
            discount.GetDiscountAmount(100).Should().Be(3.4M);

            discount.DiscountPercentage = 1;
            discount.GetDiscountAmount(200).Should().Be(2);
        }
    }

    public static class DiscountExtensions
    {
        private static readonly DiscountService _discountService;

        static DiscountExtensions()
        {
            _discountService = new DiscountService(null, null,
                null, null, null, null, null, null, null, null);
        }

        public static decimal GetDiscountAmount(this Discount discount, decimal amount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            return _discountService.GetDiscountAmount(discount, amount);
        }
    }
}