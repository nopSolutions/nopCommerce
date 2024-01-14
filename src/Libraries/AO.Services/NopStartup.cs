using AO.Services.Emails;
using AO.Services.Products;
using AO.Services.Services;
using AO.Services.Services.SyncjobServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace AO.Services
{
    public class NopStartup : INopStartup
    {
        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2; }
        }

        void INopStartup.Configure(IApplicationBuilder application)
        {
            
        }

        void INopStartup.ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAOProductService, AOProductService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IAOInvoiceService, AOInvoiceService>();
            services.AddScoped<IAOMessageTokenProvider, AOMessageTokenProvider>();
            services.AddScoped<IAOCreateProductService, AOCreateProductService>();
            services.AddScoped<IAOInvoiceStatisticsService, AOInvoiceStatisticsService>();
            services.AddScoped<IProductStatusService, ProductStatusService>();
            services.AddScoped<IStockHistoryService, StockHistoryService>();
            services.AddScoped<ICommonReOrderService, CommonReOrderService>();
            services.AddScoped<IManageStockService, ManageStockService>();
            services.AddScoped<IInventoryListService, InventoryListService>();
            services.AddScoped<IAONopProductService, AONopProductService>();
            services.AddScoped<IImageEditingService, ImageEditingService>();
            services.AddScoped<IManufacturerCategoryService, ManufacturerCategoryService>();
        }
    }
}
