using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
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
        private readonly IRepository<Discount> _discountRepository;

        public TestDiscountService(
            ICustomerService customerService,
            IDiscountPluginManager discountPluginManager,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IProductService productService,
            IRepository<Discount> discountRepository,
            IRepository<DiscountMapping> discountMappingRepository,
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
            discountMappingRepository,
            discountRequirementRepository,
            discountUsageHistoryRepository,
            orderRepository,
            cacheManager,
            storeContext)
        {
            _discountForCaching = new List<DiscountForCaching>();
            _discountRepository = discountRepository;
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

            var discountMappingRepo = new Mock<IRepository<DiscountMapping>>();

            discountMappingRepo.Setup(x => x.Table).Returns(productDiscountMapping);

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

            var orderRepo = new Mock<IRepository<Order>>();

            var discountService = new TestDiscountService(
                customerService.Object,
                discountPluginManager,
                eventPublisher.Object,
                localizationService.Object,
                productService.Object,
                discountRepo.Object,
                discountMappingRepo.Object,
                discountRequirementRepo.Object,
                discountUsageHistoryRepo.Object,
                orderRepo.Object,
                cacheManager,
                storeContext.Object);

            return discountService;
        }
    }
}
