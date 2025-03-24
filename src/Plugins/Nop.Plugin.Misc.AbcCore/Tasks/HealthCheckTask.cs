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
    class HealthCheckTask : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly INopDataProvider _nopDataProvider;
        private readonly IStaticCacheManager _staticCacheManager;

        public HealthCheckTask(ILogger logger,
            INopDataProvider nopDataProvider,
            IStaticCacheManager staticCacheManager)
        {
            _logger = logger;
            _nopDataProvider = nopDataProvider;
            _staticCacheManager = staticCacheManager;
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            // delete ABC products with illegal branding
            var illegalAbcProductCount = await _nopDataProvider.ExecuteNonQueryAsync(@"
                DELETE FROM StoreMapping
                WHERE Id IN (
                    SELECT sm.Id FROM Product_Manufacturer_Mapping pmm
                    JOIN Product p ON p.Id = pmm.ProductId
                    JOIN StoreMapping sm ON sm.EntityId = p.Id AND sm.EntityName = 'Product'
                    WHERE ManufacturerId IN (SELECT Id FROM Manufacturer WHERE Name IN ('WOLF', 'SUBZERO', 'COVE'))
                    AND p.Published = 1
                    AND sm.StoreId IN (SELECT Id FROM Store WHERE Name LIKE '%ABC%')
                )
            ");

            if (illegalAbcProductCount > 0)
            {
                await _logger.WarningAsync($"Unmapped {illegalAbcProductCount} illegal products from ABC Warehouse store, clearing cache.");
                await _staticCacheManager.ClearAsync();
            }
        }
    }
}
