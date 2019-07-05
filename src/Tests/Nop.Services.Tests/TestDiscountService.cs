using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
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

        public TestDiscountService(ICategoryService categoryService,
            ICustomerService customerService,
            IDiscountPluginManager discountPluginManager,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IRepository<Category> categoryRepository,
            IRepository<Discount> discountRepository,
            IRepository<DiscountRequirement> discountRequirementRepository,
            IRepository<DiscountUsageHistory> discountUsageHistoryRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext) : base(categoryService,
                customerService,
                discountPluginManager,
                eventPublisher,
                localizationService,
                categoryRepository,
                discountRepository,
                discountRequirementRepository,
                discountUsageHistoryRepository,
                manufacturerRepository,
                productRepository,
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

        public static IDiscountService Init()
        {
            var cacheManager = new TestCacheManager();
            var discountRepo = new Mock<IRepository<Discount>>();
            var discountRequirementRepo = new Mock<IRepository<DiscountRequirement>>();
            discountRequirementRepo.Setup(x => x.Table).Returns(new List<DiscountRequirement>().AsQueryable());
            var discountUsageHistoryRepo = new Mock<IRepository<DiscountUsageHistory>>();
            var categoryRepo = new Mock<IRepository<Category>>();
            categoryRepo.Setup(x => x.Table).Returns(new List<Category>().AsQueryable());
            var manufacturerRepo = new Mock<IRepository<Manufacturer>>();
            manufacturerRepo.Setup(x => x.Table).Returns(new List<Manufacturer>().AsQueryable());
            var productRepo = new Mock<IRepository<Product>>();
            productRepo.Setup(x => x.Table).Returns(new List<Product>().AsQueryable());
            var customerService = new Mock<ICustomerService>();
            var localizationService = new Mock<ILocalizationService>();
            var eventPublisher = new Mock<IEventPublisher>();
            var loger = new Mock<ILogger>();
            var webHelper = new Mock<IWebHelper>();

            var pluginService = new PluginService(new CatalogSettings(), customerService.Object, loger.Object, CommonHelper.DefaultFileProvider, webHelper.Object);
            var categoryService = new Mock<ICategoryService>();
            var discountPluginManager = new DiscountPluginManager(pluginService);
            var store = new Store { Id = 1 };
            var storeContext = new Mock<IStoreContext>();
            storeContext.Setup(x => x.CurrentStore).Returns(store);

            var discountService = new TestDiscountService(categoryService.Object,
                customerService.Object,
                discountPluginManager,
                eventPublisher.Object,
                localizationService.Object,
                categoryRepo.Object,
                discountRepo.Object,
                discountRequirementRepo.Object,
                discountUsageHistoryRepo.Object,
                manufacturerRepo.Object,
                productRepo.Object,
                cacheManager,
                storeContext.Object);

            return discountService;
        }
    }
}
