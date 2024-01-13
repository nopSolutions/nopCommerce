using System.Collections.Generic;
using Nop.Core;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Nop.Web.Framework.Menu;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Threading.Tasks;
using Nop.Services.Cms;
using Nop.Web.Framework.Infrastructure;
using Nop.Services.Events;
using Nop.Core.Events;
using Nop.Core.Domain.Catalog;
using Nop.Services.Media;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;
using Nop.Core.Caching;
using Nop.Core.Domain.Tasks;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.AbcCore
{
    public class CorePlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin, IWidgetPlugin,
        IConsumer<EntityDeletedEvent<ProductPicture>>,
        IConsumer<EntityUpdatedEvent<HtmlWidget>>
    {
        private readonly string TaskType =
            $"{typeof(UpdateProductTagsTask).Namespace}.{typeof(UpdateProductTagsTask).Name}, " +
            $"{typeof(UpdateProductTagsTask).Assembly.GetName().Name}";

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INopDataProvider _nopDataProvider;
        private readonly INopFileProvider _nopFileProvider;
        private readonly IPictureService _pictureService;
        private readonly IProductAbcDescriptionService _productAbcDescriptionService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStaticCacheManager _cacheManager;

        public CorePlugin(
            IWebHelper webHelper,
            ILocalizationService localizationService,
            ILogger logger,
            INopDataProvider nopDataProvider,
            INopFileProvider nopFileProvider,
            IPictureService pictureService,
            IProductAbcDescriptionService productAbcDescriptionService,
            IScheduleTaskService scheduleTaskService,
            IWebHostEnvironment webHostEnvironment,
            IStaticCacheManager cacheManager
        )
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _logger = logger;
            _nopDataProvider = nopDataProvider;
            _nopFileProvider = nopFileProvider;
            _pictureService = pictureService;
            _productAbcDescriptionService = productAbcDescriptionService;
            _scheduleTaskService = scheduleTaskService;
            _webHostEnvironment = webHostEnvironment;
            _cacheManager = cacheManager;
        }

        public async System.Threading.Tasks.Task HandleEventAsync(EntityDeletedEvent<ProductPicture> eventMessage)
        {
            // Is this an ABC product with an ABC Item Number?
            var pad = await _productAbcDescriptionService.GetProductAbcDescriptionByProductIdAsync(eventMessage.Entity.ProductId);
            if (pad == null) { return; }

            // Is there a picture in product_images?
            var abcProductImagePath = _nopFileProvider.GetFiles("wwwroot/product_images", $"{pad.AbcItemNumber}_large.*").FirstOrDefault();
            if (abcProductImagePath == null) { return; }

            // Are they the same picture? If so delete.
            var nopPictureBinary = await _pictureService.GetPictureBinaryByPictureIdAsync(eventMessage.Entity.PictureId);
            var fileSystemBinary = await _nopFileProvider.ReadAllBytesAsync(abcProductImagePath);
            if (nopPictureBinary.BinaryData.SequenceEqual(fileSystemBinary))
            {
                _nopFileProvider.DeleteFile(abcProductImagePath);
                await _logger.InformationAsync($"Deleted image `{abcProductImagePath}` (image deleted in NOP)");
            }
        }

        // Cleans additional cache for HTML Widgets after changing content to reflect changes immediately
        public async System.Threading.Tasks.Task HandleEventAsync(EntityUpdatedEvent<HtmlWidget> eventMessage)
        {
            await _cacheManager.RemoveByPrefixAsync("Nop.conditionstatement.all.");
        }

        public System.Threading.Tasks.Task<IList<string>> GetWidgetZonesAsync()
        {
            return System.Threading.Tasks.Task.FromResult<IList<string>>(new List<string> { AdminWidgetZones.ProductDetailsBlock, AdminWidgetZones.HeaderBefore });
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "AbcCore";
        }

        public bool HideInWidgetList => false;

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AbcCore/Configure";
        }

        public override async System.Threading.Tasks.Task InstallAsync()
        {
            await InstallStoredProcs();
            await UpdateLocales();
            await AddTasksAsync();

            await base.InstallAsync();
        }

        public override async System.Threading.Tasks.Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourcesAsync(CoreLocales.Base);
            await DeleteStoredProcs();
            await RemoveTasksAsync();

            await base.UninstallAsync();
        }

        public override async System.Threading.Tasks.Task UpdateAsync(string oldVersion, string currentVersion)
        {
            await InstallStoredProcs();
            await UpdateLocales();
            await AddTasksAsync();
        }

        private async System.Threading.Tasks.Task AddTasksAsync()
        {
            // Clear all previous instances first
            await RemoveTasksAsync();
            
            ScheduleTask task = new ScheduleTask();
            task.Name = $"Update Product Tags with ABC Item Number";
            task.Seconds = 14400;
            task.Type = TaskType;
            task.Enabled = false;
            task.StopOnError = false;

            await _scheduleTaskService.InsertTaskAsync(task);
        }

        private async System.Threading.Tasks.Task RemoveTasksAsync()
        {
            var task = await _scheduleTaskService.GetTaskByTypeAsync(TaskType);
            if (task != null)
            {
                await _scheduleTaskService.DeleteTaskAsync(task);
            }
        }

        private async System.Threading.Tasks.Task DeleteStoredProcs()
        {
            await _nopDataProvider.ExecuteNonQueryAsync("DROP PROCEDURE IF EXISTS dbo.UpdateAbcPromos");
        }

        private async System.Threading.Tasks.Task InstallStoredProcs()
        {
            await DeleteStoredProcs();
            string updateAbcPromosStoredProcScript = File.ReadAllText(
                            $"{_webHostEnvironment.ContentRootPath}/Plugins/Misc.AbcCore/UpdateAbcPromos.StoredProc.sql"
                        );
            await _nopDataProvider.ExecuteNonQueryAsync(updateAbcPromosStoredProcScript);
        }

        private async System.Threading.Tasks.Task UpdateLocales()
        {
            await _localizationService.AddLocaleResourceAsync(
                new Dictionary<string, string>
                {
                    ["Admin.Catalog.Products.PLPDescription"] = "PLP description",
                    ["Admin.Catalog.Products.PLPDescriptionHint"] = "Product listing page description.",
                    [CoreLocales.IsDebugMode] = "Debug Mode",
                    [CoreLocales.IsDebugModeHint] = "Logs detailed information, useful for debugging issues.",
                    [CoreLocales.AreExternalCallsSkipped] = "Skip External Calls",
                    [CoreLocales.AreExternalCallsSkippedHint] = "Skips calls to ISAM API, useful for local development.",
                    [CoreLocales.PLPDescription] = "PLP Description",
                    [CoreLocales.PLPDescriptionHint] = "Description displayed for PLP (Product Box).",
                    [CoreLocales.MobilePhoneNumber] = "Mobile Phone Header",
                    [CoreLocales.MobilePhoneNumberHint] = "The phone number used on the mobile header.",
                    [CoreLocales.GoogleMapsGeocodingAPIKey] = "Google Maps Geocoding API Key",
                    [CoreLocales.GoogleMapsGeocodingAPIKeyHint] = "API key for handling geocoding services, including delivery options/pickup in store.",
                    [CoreLocales.IsFedExMode] = "FedEx Mode",
                    [CoreLocales.IsFedExModeHint] = "Turns on FedEx mode (products require 'FedEx' attribute for delivery, intended for Mickey)."
                }
            );
        }

        public System.Threading.Tasks.Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            return System.Threading.Tasks.Task.Run(() => 
            {
                var rootMenuItem = new SiteMapNode()
                {
                    SystemName = "ABCWarehouse",
                    Title = "ABC Warehouse",
                    Visible = true,
                    RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
                    ChildNodes = new List<SiteMapNode>()
                    {
                        new SiteMapNode()
                        {
                            SystemName = "ABCWarehouse.Promos",
                            Title = "Promos",
                            Visible = true,
                            ControllerName = "AbcPromo",
                            ActionName = "List"
                        },
                        new SiteMapNode()
                        {
                            SystemName = "ABCWarehouse.MissingImageProducts",
                            Title = "Missing Image Products",
                            Visible = true,
                            ControllerName = "MissingImageProduct",
                            ActionName = "List"
                        },
                        new SiteMapNode()
                        {
                            SystemName = "ABCWarehouse.NewProducts",
                            Title = "New Products",
                            Visible = true,
                            ControllerName = "NewProduct",
                            ActionName = "List"
                        }
                    }
                };

                rootNode.ChildNodes.Add(rootMenuItem);
            });
        }
    }
}
