using SevenSpikes.Nop.Plugins.StoreLocator.Services;
using System.Linq;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;
using Nop.Services.Events;
using Nop.Core.Domain.Seo;
using Nop.Data;
using Nop.Core.Caching;
using SevenSpikes.Nop.EntitySettings.Services.EntitySettings;
using Nop.Core;
using Nop.Services.Stores;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain;
using Nop.Services.Customers;
using Nop.Services.Caching;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Core.Events;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class CustomShopService : ShopService, ICustomShopService
    {
        private readonly IRepository<ShopAbc> _shopAbcRepository;
        private readonly IRepository<Shop> _shopRepository;

        public CustomShopService(
            IRepository<ShopImage> shopImageRepository,
            IRepository<Shop> shopRepository,
            IRepository<UrlRecord> urlRecordRepository,
            IStaticCacheManager staticCacheManager,
            IEntitySettingService entitySettingService,
            IEventPublisher eventPublisher,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IWebHelper webHelper,
            IWorkContext workContext,
            StoreLocatorSettings storeLocatorSettings,
            ICustomerService customerService,
            IRepository<ShopAbc> shopAbcRepository
        ) : base(shopImageRepository, shopRepository, urlRecordRepository,
                 staticCacheManager, entitySettingService, eventPublisher,
                 storeContext, storeMappingService, webHelper, workContext,
                 storeLocatorSettings, customerService)
        {
            _shopAbcRepository = shopAbcRepository;
            _shopRepository = shopRepository;
        }

        public ShopAbc GetShopAbcByShopId(int shopId)
        {
            return _shopAbcRepository.Table.Where(sa => sa.ShopId == shopId).FirstOrDefault();
        }

        public Shop GetShopByAbcBranchId(string branchId)
        {
            var shopAbc = _shopAbcRepository.Table.Where(sa => sa.AbcId == branchId).FirstOrDefault();
            if (shopAbc == null)
            {
                return null;
            }

            var shopId = shopAbc.ShopId;

            return _shopRepository.Table.Where(s => s.Id == shopId).FirstOrDefault();
        }

        public Shop GetShopByName(string name)
        {
            return _shopRepository.Table.Where(s => s.Name == name).FirstOrDefault();
        }
    }
}