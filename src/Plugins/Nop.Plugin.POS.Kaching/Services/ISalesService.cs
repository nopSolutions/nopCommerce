using Nop.Plugin.POS.Kaching.Models.Sales;
using Nop.Plugin.POS.Kaching.Models.SalesModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.POS.Kaching.Services
{
    public interface ISalesService
    {
        Task<int> UpdateStockAsync(Dictionary<string, Root> sales);
    }
}