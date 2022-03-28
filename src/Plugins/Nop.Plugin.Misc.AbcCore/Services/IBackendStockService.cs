using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Misc.AbcCore.Models;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface IBackendStockService
    {
        Task<Dictionary<int, int>> GetStockAsync(int productId);
        Task<StockResponse> GetApiStockAsync(int productId);
        Task<bool> AvailableAtStore(int shopId, int productId);
    }
}