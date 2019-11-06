using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Tests;

namespace Nop.Services.Tests
{
    public class TestDiscountService : DiscountService
    {
        private readonly List<DiscountForCaching> _discountForCaching;

        public TestDiscountService(
            ICustomerService customerService,
            IDiscountPluginManager discountPluginManager,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IProductService productService,
            IRepository<Discount> discountRepository,
            IRepository<DiscountRequirement> discountRequirementRepository,
            IRepository<DiscountUsageHistory> discountUsageHistoryRepository,
            IRepository<Order> orderRepository,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext) : base(
            customerService,
            discountPluginManager,
            eventPublisher,
            localizationService,
            productService,
            discountRepository,
            discountRequirementRepository,
            discountUsageHistoryRepository,
            orderRepository,
            cacheManager,
            storeContext)
        {
            _discountForCaching = new List<DiscountForCaching>();
        }

        public override DiscountValidationResult ValidateDiscount(Discount discount, Customer customer)
        {
            return new DiscountValidationResult { IsValid = true };
        }

        public override DiscountValidationResult ValidateDiscount(DiscountForCaching discount, Customer customer)
        {
            return new DiscountValidationResult { IsValid = true };
        }

        public override IList<DiscountForCaching> GetAllDiscountsForCaching(DiscountType? discountType = null,
            string couponCode = null, string discountName = null,
            bool showHidden = false)
        {

            return _discountForCaching
                .Where(x => !discountType.HasValue || x.DiscountType == discountType.Value)
                .Where(x => string.IsNullOrEmpty(couponCode) || x.CouponCode == couponCode)
                //UNDONE other filtering such as discountName, showHidden (not actually required in unit tests)
                .ToList();
        }

        public void AddDiscount(DiscountType discountType)
        {
            _discountForCaching.Clear();

            //discounts
            var discount = new DiscountForCaching
            {
                Id = 1,
                Name = "Discount 1",
                DiscountType = discountType,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited
            };

            _discountForCaching.Add(discount);
        }

        public void ClearDiscount()
        {
            _discountForCaching.Clear();
        }

        public static IDiscountService Init(IQueryable<Discount> discounts = default(IQueryable<Discount>), IQueryable<DiscountProductMapping> productDiscountMapping = null)
        {
            var cacheManager = new TestCacheManager();
            var discountRepo = new Mock<IRepository<Discount>>();

            discountRepo.Setup(r => r.Table).Returns(discounts);

            var discountRequirementRepo = new Mock<IRepository<DiscountRequirement>>();
            discountRequirementRepo.Setup(x => x.Table).Returns(new List<DiscountRequirement>().AsQueryable());
            var discountUsageHistoryRepo = new Mock<IRepository<DiscountUsageHistory>>();

            var customerService = new Mock<ICustomerService>();
            var localizationService = new Mock<ILocalizationService>();
            var productService = new Mock<IProductService>();

            var eventPublisher = new Mock<IEventPublisher>();
            var loger = new Mock<ILogger>();
            var webHelper = new Mock<IWebHelper>();

            var pluginService = new PluginService(new CatalogSettings(), customerService.Object, loger.Object, CommonHelper.DefaultFileProvider, webHelper.Object);

            var discountPluginManager = new DiscountPluginManager(pluginService);
            var store = new Store { Id = 1 };
            var storeContext = new Mock<IStoreContext>();
            storeContext.Setup(x => x.CurrentStore).Returns(store);

            var dbContext = new Mock<IDbContext>();
            var orderRepo = new Mock<IRepository<Order>>();

            void setupDbSet<T>(IQueryable<T> result) where T : DiscountMapping
            {
                dbContext.Setup(c => c.Set<T>()).Returns(GetDbSetMock<T>(result).Object);
            }

            setupDbSet(productDiscountMapping);

            var discountService = new TestDiscountService(
                customerService.Object,
                dbContext.Object,
                discountPluginManager,
                eventPublisher.Object,
                localizationService.Object,
                productService.Object,
                discountRepo.Object,
                discountRequirementRepo.Object,
                discountUsageHistoryRepo.Object,
                orderRepo.Object,
                cacheManager,
                storeContext.Object);

            return discountService;
        }

        private static Mock<DbSet<T>> GetDbSetMock<T>(IQueryable<T> items = null) where T : class
        {
            if (items == null)
            {
                items = Enumerable.Empty<T>().AsQueryable();;
            }

            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(items.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(items.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(items.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(items.GetEnumerator());

            return dbSetMock;
        }

    }
}
