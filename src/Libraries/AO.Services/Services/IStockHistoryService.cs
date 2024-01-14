using AO.Services.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public interface IStockHistoryService
    {
        Task<AOStockHistory> CreateStockHistoryAsync(decimal costPrice);
        Task<List<AOStockHistory>> GetStockHistoriesAsync(int daysBack);
        Task<bool> StockHistoryExistForTodayAsync();
    }
}