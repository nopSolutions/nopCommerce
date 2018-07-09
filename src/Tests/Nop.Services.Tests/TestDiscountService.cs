using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
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
using Nop.Services.Plugins;
using Nop.Tests;

namespace Nop.Services.Tests
{
    public class TestDiscountService : DiscountService
    {
        private List<DiscountForCaching> _discountForCaching;

        public TestDiscountService(IStaticCacheManager cacheManager, IRepository<Discount> discountRepository,
            IRepository<DiscountRequirement> discountRequirementRepository,
            IRepository<DiscountUsageHistory> discountUsageHistoryRepository,
            IRepository<Category> categoryRepository, IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository, IStoreContext storeContext, ICustomerService customerService,
            ILocalizationService localizationService, ICategoryService categoryService, IPluginFinder pluginFinder,
            IEventPublisher eventPublisher) : base(cacheManager, discountRepository, discountRequirementRepository,
            discountUsageHistoryRepository, categoryRepository, manufacturerRepository, productRepository,
            storeContext, customerService, localizationService, categoryService, pluginFinder, eventPublisher)
        {
            this._discountForCaching = new List<DiscountForCaching>();
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
            return _discountForCaching;
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
            var _cacheManager = new TestMemoryCacheManager(new Mock<IMemoryCache>().Object);
            var _discountRepo = new Mock<IRepository<Discount>>();
            var _discountRequirementRepo = new Mock<IRepository<DiscountRequirement>>();
            _discountRequirementRepo.Setup(x => x.Table).Returns(new List<DiscountRequirement>().AsQueryable());
            var _discountUsageHistoryRepo = new Mock<IRepository<DiscountUsageHistory>>();
            var _categoryRepo = new Mock<IRepository<Category>>();
            _categoryRepo.Setup(x => x.Table).Returns(new List<Category>().AsQueryable());
            var _manufacturerRepo = new Mock<IRepository<Manufacturer>>();
            _manufacturerRepo.Setup(x => x.Table).Returns(new List<Manufacturer>().AsQueryable());
            var _productRepo = new Mock<IRepository<Product>>();
            _productRepo.Setup(x => x.Table).Returns(new List<Product>().AsQueryable());
            var _customerService = new Mock<ICustomerService>();
            var _localizationService = new Mock<ILocalizationService>();
            var _eventPublisher = new Mock<IEventPublisher>();
            var pluginFinder = new PluginFinder(_eventPublisher.Object);
            var _categoryService = new Mock<ICategoryService>();

            var _store = new Store {Id = 1};
            var _storeContext = new Mock<IStoreContext>();
            _storeContext.Setup(x => x.CurrentStore).Returns(_store);

            var discountService = new TestDiscountService(_cacheManager, _discountRepo.Object,
                _discountRequirementRepo.Object,
                _discountUsageHistoryRepo.Object, _categoryRepo.Object, _manufacturerRepo.Object, _productRepo.Object,
                _storeContext.Object,
                _customerService.Object, _localizationService.Object, _categoryService.Object, pluginFinder,
                _eventPublisher.Object);

            return discountService;
        }
    }
}
