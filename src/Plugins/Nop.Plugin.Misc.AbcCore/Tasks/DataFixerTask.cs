using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Services.Tasks;
using OfficeOpenXml;
using System.IO;
using Nop.Data;
using Nop.Core.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Nop.Services.Media;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Plugin.Misc.AbcCore.Extensions;
using System.Collections.Generic;
using Nop.Plugin.Misc.AbcCore.Services.Custom;
using System.Threading.Tasks;
using Nop.Services.Logging;
using Nop.Core.Domain.Logging;
using Nop.Core.Caching;

namespace Nop.Plugin.Misc.AbcCore.Tasks
{
    class DataFixerTask : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly INopDataProvider _nopDataProvider;
        private readonly IStaticCacheManager _staticCacheManager;

        private readonly List<string> illegalAbcBrands = new List<string>
        {
            "WOLF", "SUBZERO", "COVE", "VIKING"
        };

        public DataFixerTask(ILogger logger,
            INopDataProvider nopDataProvider,
            IStaticCacheManager staticCacheManager)
        {
            _logger = logger;
            _nopDataProvider = nopDataProvider;
            _staticCacheManager = staticCacheManager;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            var shouldClearCache = false;

            var illegalAbcProductCount = await UnmapIllegalAbcProductsAsync();
            if (illegalAbcProductCount > 0)
            {
                await _logger.WarningAsync($"DataFixer: Unmapped {illegalAbcProductCount} illegal products from ABC Warehouse store.");
                shouldClearCache = true;
            }

            var illegalAbcPromoCount = await UnmapIllegalAbcPromosAsync();
            if (illegalAbcPromoCount > 0)
            {
                await _logger.WarningAsync($"DataFixer: Unmapped {illegalAbcPromoCount} illegal promos from ABC Warehouse store.");
                shouldClearCache = true;
            }

            if (shouldClearCache)
            {
                await _logger.WarningAsync($"DataFixer: Fixes performed, clearing cache.");
                await _staticCacheManager.ClearAsync();
            }
        }

        private async Task<int> UnmapIllegalAbcProductsAsync()
        {
            return await _nopDataProvider.ExecuteNonQueryAsync($@"
                DELETE FROM StoreMapping
                WHERE Id IN (
                    SELECT sm.Id FROM Product_Manufacturer_Mapping pmm
                    JOIN Product p ON p.Id = pmm.ProductId
                    JOIN StoreMapping sm ON sm.EntityId = p.Id AND sm.EntityName = 'Product'
                    WHERE ManufacturerId IN (
                        SELECT Id FROM Manufacturer WHERE Name IN ({string.Join(",", illegalAbcBrands.Select(b => $"'{b}'"))})
                    )
                    AND p.Published = 1
                    AND sm.StoreId IN (SELECT Id FROM Store WHERE Name LIKE '%ABC%')
                )
            ");
        }

        private async Task<int> UnmapIllegalAbcPromosAsync()
        {
            return await _nopDataProvider.ExecuteNonQueryAsync($@"
                DELETE FROM StoreMapping
                WHERE EntityName = 'AbcPromo'
                AND StoreId IN (SELECT Id FROM Store WHERE Name LIKE '%ABC%')
                AND EntityId in (
                    SELECT Id from AbcPromo
                    WHERE ManufacturerId IN (
                        SELECT Id FROM Manufacturer WHERE Name IN ({string.Join(",", illegalAbcBrands.Select(b => $"'{b}'"))})
                    )
                )
            ");
        }
    }
}
