using AO.Services.Products.Models;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AO.Services.Services.SyncjobServices
{
    public interface IAONopProductService
    {
        Task<SingleUpdateResult> UpdateDatabaseAsync(VariantData variantData, SyncUpdaterInfo syncUpdaterInfo);

        Task CleanUpDatabaseAsync(IList<VariantData> variantsData, IList<ProductAttributeCombination> combinations, int securityPercantage, string updaterName);
    }
}