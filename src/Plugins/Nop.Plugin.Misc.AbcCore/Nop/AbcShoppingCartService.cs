using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Services.Catalog;
using Nop.Plugin.Misc.AbcCore.Delivery;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    public class AbcShoppingCartService : ShoppingCartService, IAbcShoppingCartService
    {
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public AbcShoppingCartService(
            CatalogSettings catalogSettings,
            IAclService aclService,
            IActionContextAccessor actionContextAccessor,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IRepository<ShoppingCartItem> sciRepository,
            IShippingService shippingService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings)
            : base(
                catalogSettings,
                aclService,
                actionContextAccessor,
                checkoutAttributeParser,
                checkoutAttributeService,
                currencyService,
                customerService,
                dateRangeService,
                dateTimeHelper,
                genericAttributeService,
                localizationService,
                permissionService,
                priceCalculationService,
                priceFormatter,
                productAttributeParser,
                productAttributeService,
                productService,
                sciRepository,
                shippingService,
                staticCacheManager,
                storeContext,
                storeMappingService,
                urlHelperFactory,
                urlRecordService,
                workContext,
                orderSettings,
                shoppingCartSettings)
        {
            _storeContext = storeContext;
            _workContext = workContext;
        }

        public async Task<IList<ShoppingCartItem>> GetCurrentShoppingCartAsync()
        {
            return GetShoppingCartAsync(
                await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id);
        }
    }
}