using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Tests;

namespace Nop.Services.Tests
{
    public class TestDiscountService : DiscountService
    {
        private readonly List<Discount> _discounts;

        public TestDiscountService(ICacheKeyService cacheKeyService,
            ICustomerService customerService,
            IDiscountPluginManager discountPluginManager,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IProductService productService,
            IRepository<Discount> discountRepository,
            IRepository<DiscountRequirement> discountRequirementRepository,
            IRepository<DiscountUsageHistory> discountUsageHistoryRepository,
            IRepository<Order> orderRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext) : base(
            cacheKeyService,
            customerService,
            discountPluginManager,
            eventPublisher,
            localizationService,
            productService,
            discountRepository,
            discountRequirementRepository,
            discountUsageHistoryRepository,
            orderRepository,
            staticCacheManager,
            storeContext)
        {
            _discounts = new List<Discount>();
        }

        public override DiscountValidationResult ValidateDiscount(Discount discount, Customer customer)
        {
            return new DiscountValidationResult { IsValid = true };
        }

        public override IList<Discount> GetAllDiscounts(DiscountType? discountType = null,
            string couponCode = null, string discountName = null, bool showHidden = false,
            DateTime? startDateUtc = null, DateTime? endDateUtc = null)
        {
            return _discounts
                .Where(x => !discountType.HasValue || x.DiscountType == discountType.Value)
                .Where(x => string.IsNullOrEmpty(couponCode) || x.CouponCode == couponCode)
                .ToList();
        }

        public void AddDiscount(DiscountType discountType)
        {
            _discounts.Clear();

            //discounts
            var discount = new Discount
            {
                Id = 1,
                Name = "Discount 1",
                DiscountType = discountType,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited
            };

            _discounts.Add(discount);
        }

        public void ClearDiscount()
        {
            _discounts.Clear();
        }

        public static IDiscountService Init(IQueryable<Discount> discounts = default, IQueryable<DiscountProductMapping> productDiscountMapping = null)
        {
            var staticCacheManager = new TestCacheManager();
            var discountRepo = new Mock<IRepository<Discount>>();

            discountRepo.Setup(r => r.Table).Returns(discounts);

            var discountRequirementRepo = new Mock<IRepository<DiscountRequirement>>();
            discountRequirementRepo.Setup(x => x.Table).Returns(new List<DiscountRequirement>().AsQueryable());
            var discountUsageHistoryRepo = new Mock<IRepository<DiscountUsageHistory>>();

            var discountMappingRepo = new Mock<IRepository<DiscountMapping>>();

            discountMappingRepo.Setup(x => x.Table).Returns(productDiscountMapping);

            var customerService = new Mock<ICustomerService>();
            var localizationService = new Mock<ILocalizationService>();
            var productService = new Mock<IProductService>();

            var eventPublisher = new Mock<IEventPublisher>();

            var pluginService = new FakePluginService();

            var discountPluginManager = new DiscountPluginManager(customerService.Object, pluginService);
            var store = new Store { Id = 1 };
            var storeContext = new Mock<IStoreContext>();
            storeContext.Setup(x => x.CurrentStore).Returns(store);

            var orderRepo = new Mock<IRepository<Order>>();

            var discountService = new TestDiscountService(
                new FakeCacheKeyService(),
                customerService.Object,
                discountPluginManager,
                eventPublisher.Object,
                localizationService.Object,
                productService.Object,
                discountRepo.Object,
                discountRequirementRepo.Object,
                discountUsageHistoryRepo.Object,
                orderRepo.Object,
                staticCacheManager,
                storeContext.Object);

            return discountService;
        }
    }
}
