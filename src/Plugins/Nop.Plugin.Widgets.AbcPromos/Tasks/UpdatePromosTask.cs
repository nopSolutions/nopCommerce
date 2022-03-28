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

namespace Nop.Plugin.Widgets.AbcPromos.Tasks
{
    public class UpdatePromosTask : IScheduleTask
    {
        private readonly CoreSettings _coreSettings;

        private readonly ILogger _logger;

        private readonly ICustomNopDataProvider _nopDataProvider;

        private readonly IAbcPromoService _abcPromoService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IUrlRecordService _urlRecordService;

        private readonly GenerateRebatePromoPageTask _generateRebatePromoPageTask;

        public UpdatePromosTask(
            CoreSettings coreSettings,
            AbcPromosSettings settings,
            ILogger logger,
            ICustomNopDataProvider nopDataProvider,
            IAbcPromoService abcPromoService,
            IManufacturerService manufacturerService,
            IUrlRecordService urlRecordService,
            GenerateRebatePromoPageTask generateRebatePromoPageTask
        )
        {
            _coreSettings = coreSettings;
            _logger = logger;
            _nopDataProvider = nopDataProvider;
            _abcPromoService = abcPromoService;
            _manufacturerService = manufacturerService;
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
            await SetPromoManufacturersAsync(promos);
            await SetPromoSlugsAsync(promos);

            await _generateRebatePromoPageTask.ExecuteAsync();
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
