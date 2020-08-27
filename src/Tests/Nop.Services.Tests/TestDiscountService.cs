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
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Tests;

namespace Nop.Services.Tests
{
    public class TestDiscountService : DiscountService
    {
        private readonly List<Discount> _discounts;

        public TestDiscountService(ICustomerService customerService,
            IDiscountPluginManager discountPluginManager,
            ILocalizationService localizationService,
            IProductService productService,
            IRepository<Discount> discountRepository,
            IRepository<DiscountRequirement> discountRequirementRepository,
            IRepository<DiscountUsageHistory> discountUsageHistoryRepository,
            IRepository<Order> orderRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext) : base(
            customerService,
            discountPluginManager,
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
            var discountRepo = new FakeRepository<Discount>(discounts.ToList());
            
            var discountRequirementRepo = new FakeRepository<DiscountRequirement>();
            var discountUsageHistoryRepo = new FakeRepository<DiscountUsageHistory>();

            var customerService = new Mock<ICustomerService>();
            var localizationService = new Mock<ILocalizationService>();
            var productService = new Mock<IProductService>();

            var pluginService = new FakePluginService();

            var discountPluginManager = new DiscountPluginManager(customerService.Object, pluginService);
            var store = new Store { Id = 1 };
            var storeContext = new Mock<IStoreContext>();
            storeContext.Setup(x => x.CurrentStore).Returns(store);

            var orderRepo = new FakeRepository<Order>();

            var discountService = new TestDiscountService(
                customerService.Object,
                discountPluginManager,
                localizationService.Object,
                productService.Object,
                discountRepo,
                discountRequirementRepo,
                discountUsageHistoryRepo,
                orderRepo,
                staticCacheManager,
                storeContext.Object);

            return discountService;
        }
    }
}
