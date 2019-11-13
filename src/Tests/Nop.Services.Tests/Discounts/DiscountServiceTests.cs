using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Discounts
{
    [TestFixture]
    public class DiscountServiceTests : ServiceTest
    {
        private Mock<IEventPublisher> _eventPublisher = new Mock<IEventPublisher>();
        private Mock<ILocalizationService> _localizationService = new Mock<ILocalizationService>();
        private IDiscountPluginManager _discountPluginManager;
        private IDiscountService _discountService;
        private Mock<IStoreContext> _storeContext = new Mock<IStoreContext>();
        private Mock<ICustomerService> _customerService = new Mock<ICustomerService>();
        private Mock<IProductService> _productService = new Mock<IProductService>();
        private Mock<IRepository<Category>> _categoryRepo = new Mock<IRepository<Category>>();
        private Mock<IRepository<Discount>> _discountRepo = new Mock<IRepository<Discount>>();
        private Mock<IRepository<DiscountRequirement>> _discountRequirementRepo = new Mock<IRepository<DiscountRequirement>>();
        private Mock<IRepository<DiscountUsageHistory>> _discountUsageHistoryRepo = new Mock<IRepository<DiscountUsageHistory>>();
        private Mock<IRepository<Manufacturer>> _manufacturerRepo = new Mock<IRepository<Manufacturer>>();
        private Mock<IRepository<Order>> _orderRepo = new Mock<IRepository<Order>>();
        private Mock<IRepository<Product>> _productRepo = new Mock<IRepository<Product>>();        
        private CatalogSettings _catalogSettings = new CatalogSettings();

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

            _discountRepo.Setup(x => x.Table).Returns(new List<Discount> { discount1, discount2 }.AsQueryable());

            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));
            _categoryRepo.Setup(x => x.Table).Returns(new List<Category>().AsQueryable());
            _manufacturerRepo.Setup(x => x.Table).Returns(new List<Manufacturer>().AsQueryable());
            _productRepo.Setup(x => x.Table).Returns(new List<Product>().AsQueryable());

            var cacheManager = new TestCacheManager();
            _discountRequirementRepo.Setup(x => x.Table).Returns(new List<DiscountRequirement>().AsQueryable());


            var loger = new Mock<ILogger>();
            var webHelper = new Mock<IWebHelper>();

            var pluginService = new PluginService(_catalogSettings, _customerService.Object, loger.Object, CommonHelper.DefaultFileProvider, webHelper.Object);

            var discountMappingRepo = new Mock<IRepository<DiscountMapping>>();

            _discountPluginManager = new DiscountPluginManager(pluginService);
            _discountService = new DiscountService(
                _customerService.Object,
                _discountPluginManager,
                _eventPublisher.Object,
                _localizationService.Object,
                _productService.Object,
                _discountRepo.Object,
                discountMappingRepo.Object,
                _discountRequirementRepo.Object,
                _discountUsageHistoryRepo.Object,
                _orderRepo.Object,
                cacheManager,
                _storeContext.Object);
        }

        [Test]
        public void Can_get_all_discount()
        {
            var discounts = _discountService.GetAllDiscounts();
            discounts.ShouldNotBeNull();
            discounts.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_load_discountRequirementRules()
        {
            var rules = _discountPluginManager.LoadAllPlugins();
            rules.ShouldNotBeNull();
            rules.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_load_discountRequirementRuleBySystemKeyword()
        {
            var rule = _discountPluginManager.LoadPluginBySystemName("TestDiscountRequirementRule");
            rule.ShouldNotBeNull();
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


            //UNDONE: little workaround here
            //we have to register "nop_cache_static" cache manager (null manager) from DependencyRegistrar.cs
            //because DiscountService right now dynamically Resolve<ICacheManager>("nop_cache_static")
            //we cannot inject it because DiscountService already has "per-request" cache manager injected 
            //EngineContext.Initialize(false);

            _discountService.ValidateDiscount(discount, customer, new[] { "CouponCode 1" }).IsValid.ShouldEqual(true);
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
            _discountService.ValidateDiscount(discount, customer, new[] { "CouponCode 2" }).IsValid.ShouldEqual(false);
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
            _discountService.ValidateDiscount(discount, customer, null).IsValid.ShouldEqual(true);

            discount.StartDateUtc = DateTime.UtcNow.AddDays(1);
            _discountService.ValidateDiscount(discount, customer, null).IsValid.ShouldEqual(false);
        }

        [Test]
        public void Can_calculate_discount_amount_percentage()
        {
            var discount = new Discount
            {
                UsePercentage = true,
                DiscountPercentage = 30
            };

            discount.GetDiscountAmount(100).ShouldEqual(30);

            discount.DiscountPercentage = 60;
            discount.GetDiscountAmount(200).ShouldEqual(120);
        }

        [Test]
        public void Can_calculate_discount_amount_fixed()
        {
            var discount = new Discount
            {
                UsePercentage = false,
                DiscountAmount = 10
            };

            discount.GetDiscountAmount(100).ShouldEqual(10);

            discount.DiscountAmount = 20;
            discount.GetDiscountAmount(200).ShouldEqual(20);
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

            discount.GetDiscountAmount(100).ShouldEqual(3.4M);

            discount.DiscountPercentage = 60;
            discount.GetDiscountAmount(200).ShouldEqual(3.4M);
            discount.GetDiscountAmount(100).ShouldEqual(3.4M);

            discount.DiscountPercentage = 1;
            discount.GetDiscountAmount(200).ShouldEqual(2);
        }
    }

    public static class DiscountExtensions
    {
        private static readonly DiscountService _discountService;

        static DiscountExtensions()
        {
            _discountService = new DiscountService(null, null, null, null,
                null, null, null, null, null, null, null, null);
        }

        public static decimal GetDiscountAmount(this Discount discount, decimal amount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            return _discountService.GetDiscountAmount(_discountService.MapDiscount(discount), amount);
        }
    }
}