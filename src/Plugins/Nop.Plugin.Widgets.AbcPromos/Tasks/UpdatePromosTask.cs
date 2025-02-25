using System.Collections.Generic;
using System.Linq;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Plugin.Widgets.AbcPromos.Tasks.LegacyTasks;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Services.Tasks;
using Task = System.Threading.Tasks.Task;
using Nop.Plugin.Misc.AbcCore.Data;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.AbcPromos.Tasks
{
    public class UpdatePromosTask : IScheduleTask
    {
        private readonly CoreSettings _coreSettings;

        private readonly ILogger _logger;

        private readonly ICustomNopDataProvider _nopDataProvider;

        private readonly IAbcPromoService _abcPromoService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;

        private readonly GenerateRebatePromoPageTask _generateRebatePromoPageTask;

        public UpdatePromosTask(
            CoreSettings coreSettings,
            AbcPromosSettings settings,
            ILogger logger,
            ICustomNopDataProvider nopDataProvider,
            IAbcPromoService abcPromoService,
            IManufacturerService manufacturerService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            GenerateRebatePromoPageTask generateRebatePromoPageTask
        )
        {
            _coreSettings = coreSettings;
            _logger = logger;
            _nopDataProvider = nopDataProvider;
            _abcPromoService = abcPromoService;
            _manufacturerService = manufacturerService;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _generateRebatePromoPageTask = generateRebatePromoPageTask;
        }

        public async Task ExecuteAsync()
        {
            if (_coreSettings.AreExternalCallsSkipped)
            {
                await _logger.WarningAsync("Widgets.AbcPromos: External calls skipped, will not update from backend.");
            }
            else
            {
                await _nopDataProvider.ExecuteNonQueryAsync("EXEC UpdateAbcPromos", 1200);
            }

            var promos = await _abcPromoService.GetAllPromosAsync();
            // consider combining these steps so we loop through promos only once
            await SetPromoManufacturersAsync(promos);
            await SetPromoSlugsAsync(promos);
            await UpdateStoreMappings(promos);

            await _generateRebatePromoPageTask.ExecuteAsync();
        }

        private async Task UpdateStoreMappings(IList<AbcPromo> promos)
        {
            foreach (var promo in promos)
            {
                var newStoreMappingIds = new HashSet<int>();
                var products = await _abcPromoService.GetPublishedProductsByPromoIdAsync(promo.Id);
                foreach (var product in products)
                {
                    var storeMappings =
                        await _storeMappingService.GetStoreMappingsAsync(product);

                    foreach (var storeMapping in storeMappings)
                    {
                        newStoreMappingIds.Add(storeMapping.StoreId);
                    }
                }

                var existingPromoStoreMappings = await _storeMappingService.GetStoreMappingsAsync(promo);

                var toBeDeleted = existingPromoStoreMappings
                    .Where(e => !newStoreMappingIds.Any(n => n == e.StoreId));
                var toBeInserted = newStoreMappingIds
                    .Where(n => !existingPromoStoreMappings.Any(e => n == e.StoreId));

                toBeInserted.ToList().ForEach(async n => {
                    await _storeMappingService.InsertStoreMappingAsync(promo, n);
                });
                toBeDeleted.ToList().ForEach(async e => {
                    await _storeMappingService.DeleteStoreMappingAsync(e);
                });
            }
        }

        private async Task SetPromoManufacturersAsync(IList<AbcPromo> promos)
        {
            foreach (var promo in promos)
            {
                var manufacturerIds = new HashSet<int>();
                var products = await _abcPromoService.GetProductsByPromoIdAsync(promo.Id);
                foreach (var product in products)
                {
                    var productManufacturers =
                        await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id);

                    foreach (var productManufacturer in productManufacturers)
                    {
                        manufacturerIds.Add(productManufacturer.ManufacturerId);
                    }
                }

                promo.ManufacturerId = manufacturerIds.Count == 1 ? manufacturerIds.FirstOrDefault() : null;
                _abcPromoService.UpdatePromo(promo);
            }
        }

        private async Task SetPromoSlugsAsync(IList<AbcPromo> promos)
        {
            foreach (var promo in promos)
            {
                var name = promo.Name.Replace("_", "-") + "-" + promo.Description;

                if (promo.ManufacturerId != null)
                {
                    var manufacturer =
                        await _manufacturerService.GetManufacturerByIdAsync(promo.ManufacturerId.Value);
                    var manufacturerName = manufacturer.Name;

                    name = name.Insert(0, $"{manufacturerName} - ");
                }

                var seName = await _urlRecordService.ValidateSeNameAsync(
                    promo,
                    string.Empty,
                    name,
                    true
                );
                await _urlRecordService.SaveSlugAsync(promo, seName, 0);
            }
        }
    }
}
