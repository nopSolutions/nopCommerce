using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Plugin.Misc.AbcCore.HomeDelivery;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Plugin.Misc.AbcCore.Services.Custom;
using Nop.Plugin.Misc.AbcCore.Data;
using Nop.Plugin.Misc.AbcCore.Factories;
using Nop.Plugin.Misc.AbcCore.Mattresses;
using Nop.Plugin.Misc.AbcCore.Nop;
using Nop.Web.Factories;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Logging;
using SevenSpikes.Nop.Services.Catalog;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return int.MaxValue;
            }
        }

        public void Register(
               IServiceCollection services,
               ITypeFinder typeFinder,
               AppSettings appSettings
        ) {
            services.AddScoped<IAbcMattressProductService, AbcMattressProductService>();
            services.AddScoped<IAbcMattressModelService, AbcMattressModelService>();
            services.AddScoped<IAbcMattressEntryService, AbcMattressEntryService>();
            services.AddScoped<IAbcMattressPackageService, AbcMattressPackageService>();
            services.AddScoped<IAbcMattressBaseService, AbcMattressBaseService>();
            services.AddScoped<IAbcMattressGiftService, AbcMattressGiftService>();
            services.AddScoped<IAbcMattressProtectorService, AbcMattressProtectorService>();
            services.AddScoped<IAbcMattressFrameService, AbcMattressFrameService>();
            services.AddScoped<IAbcMattressListingPriceService, AbcMattressListingPriceService>();
            services.AddScoped<IAbcProductModelFactory, AbcProductModelFactory>();
            services.AddScoped<IProductModelFactory, AbcProductModelFactory>();
            services.AddScoped<IBackendStockService, BackendStockService>();
            services.AddScoped<IAttributeUtilities, AttributeUtilities>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<FrontEndService>();
            services.AddScoped<IAbcPromoService, AbcPromoService>();
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<IIsamGiftCardService, IsamGiftCardService>();
            services.AddScoped<ICustomerShopService, CustomerShopService>();
            services.AddScoped<ICustomShopService, CustomShopService>();
            services.AddScoped<IProductAbcDescriptionService, ProductAbcDescriptionService>();
            services.AddScoped<IHomeDeliveryCostService, HomeDeliveryCostService>();
            services.AddScoped<IAbcProductService, AbcProductService>();
            services.AddScoped<ITermLookupService, TermLookupService>();
            services.AddScoped<ICardCheckService, CardCheckService>();
            services.AddScoped<IProductAbcFinanceService, ProductAbcFinanceService>();
            services.AddScoped<IImportUtilities, ImportUtilities>();
            services.AddScoped<ICustomManufacturerService, CustomManufacturerService>();
            services.AddScoped<ICustomNopDataProvider, CustomMsSqlDataProvider>();
            services.AddScoped<ArchiveService>();
            services.AddScoped<IAbcDeliveryService, AbcDeliveryService>();
            services.AddScoped<IAbcProductAttributeService, AbcProductAttributeService>();
            services.AddScoped<IGeocodeService, GeocodeService>();
            services.AddScoped<ICheckoutModelFactory, AbcCheckoutModelFactory>();
            services.AddScoped<IShoppingCartModelFactory, AbcShoppingCartModelFactory>();
            services.AddScoped<IAbcCategoryService, AbcCategoryService>();
            // Need both to cover base interface
            services.AddScoped<IShoppingCartService, AbcShoppingCartService>();
            services.AddScoped<IAbcShoppingCartService, AbcShoppingCartService>();
            services.AddScoped<IShippingService, AbcShippingService>();
            services.AddScoped<ILogger, AbcLogger>();
            services.AddScoped<IAbcLogger, AbcLogger>();
            // Overrides AJAX filter functionality
            services.AddScoped<IManufacturerService7Spikes, AbcManufacturerService7Spikes>();
        }
    }
}
