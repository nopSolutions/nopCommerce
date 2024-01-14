using AO.Services.Products.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AO.Services.Products
{
    [Obsolete("This method makes use of web api, use AO.Services.Services.SyncjobServices.IAONopProductService.UpdateDatabaseAsync instead")]
    public interface IAOProductService
    {
        [Obsolete("This method makes use of web api, use AO.Services.Services.SyncjobServices.IAONopProductService.UpdateDatabaseAsync instead")]
        Task<string> SaveVariantDataAsync(List<VariantData> variantDataList, string updaterName, int minimumStockCount);
    }
}
