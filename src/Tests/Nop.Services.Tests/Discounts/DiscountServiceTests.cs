using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Data;
using Nop.Services.Security;
using Nop.Tests;
using NUnit.Framework;
using Nop.Core;
using Nop.Services.Discounts;
using Nop.Core.Domain.Discounts;
using Rhino.Mocks;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;

namespace Nop.Services.Tests.Discounts
{
    [TestFixture]
    public class DiscountServiceTests
    {
        IRepository<Discount> _discountRepo;
        IDiscountService _discountService;

        [SetUp]
        public void SetUp()
        {
            _discountRepo = MockRepository.GenerateMock<IRepository<Discount>>();
            var discount1 = new Discount
            {
                DiscountType = DiscountType.AssignedToCategories,
                Name = "Discount 1",
                UsePercentage = true,
                DiscountPercentage = 10,
                DiscountAmount =0,
                DiscountLimitation = DiscountLimitationType.Unlimited,
                LimitationTimes = 0,
            };
            var discount2 = new Discount
            {
                DiscountType = DiscountType.AssignedToSkus,
                Name = "Discount 2",
                UsePercentage = false,
                DiscountPercentage = 0,
                DiscountAmount = 5,
                RequiresCouponCode = true,
                CouponCode = "SecretCode",
                DiscountLimitation = DiscountLimitationType.NTimesPerCustomer,
                LimitationTimes = 3,
            };

            _discountRepo.Expect(x => x.Table).Return(new List<Discount>() { discount1, discount2 }.AsQueryable());

            var cacheManager = new NopNullCache();
            _discountService = new DiscountService(cacheManager, _discountRepo, new AppDomainTypeFinder());
        }

        [Test]
        public void Can_get_all_discount()
        {
            var discounts = _discountService.GetAllDiscounts(null);
            discounts.ShouldNotBeNull();
            (discounts.Count > 0).ShouldBeTrue();
        }

        //TODO uncomment when we'll be able to test plugins
        //[Test]
        //public void Can_load_discountRequirementRules()
        //{
        //    var rules = _discountService.LoadAllDiscountRequirementRules();
        //    rules.ShouldNotBeNull();
        //    (rules.Count > 0).ShouldBeTrue();
        //}

        //[Test]
        //public void Can_load_discountRequirementRuleBySystemKeyword()
        //{
        //    var rule = _discountService.LoadDiscountRequirementRuleBySystemName("BillingCountryIs");
        //    rule.ShouldNotBeNull();
        //}

        [Test]
        public void Can_validate_discount_code()
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
                DiscountLimitation = DiscountLimitationType.Unlimited,
            };
            
            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = "admin@yourStore.com",
                Username = "admin@yourStore.com",
                AdminComment = "",
                DiscountCouponCode = "CouponCode 1",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };

            var result1 = _discountService.IsDiscountValid(discount, customer);
            result1.ShouldEqual(true);

            customer.DiscountCouponCode = "CouponCode 2";
            var result2 = _discountService.IsDiscountValid(discount, customer);
            result2.ShouldEqual(false);
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
                DiscountLimitation = DiscountLimitationType.Unlimited,
            };

            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = "admin@yourStore.com",
                Username = "admin@yourStore.com",
                AdminComment = "",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };

            var result1 = _discountService.IsDiscountValid(discount, customer);
            result1.ShouldEqual(true);

            discount.StartDateUtc = DateTime.UtcNow.AddDays(1);
            var result2 = _discountService.IsDiscountValid(discount, customer);
            result2.ShouldEqual(false);
        }
    }
}
