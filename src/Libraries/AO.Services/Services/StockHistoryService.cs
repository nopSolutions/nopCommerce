using AO.Services.Domain;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public class StockHistoryService : IStockHistoryService
    {
        #region Private variables        
        private readonly IRepository<AOStockHistory> _stockHistoryRepository;
        #endregion

        public StockHistoryService(IRepository<AOStockHistory> stockHistoryRepository)
        {
            _stockHistoryRepository = stockHistoryRepository;
        }

        public async Task<AOStockHistory> CreateStockHistoryAsync(decimal costPrice)
        {
            var stockHistory = new AOStockHistory()
            {
                CostPrice = costPrice,
                CreatedDate = DateTime.UtcNow
            };

            await _stockHistoryRepository.InsertAsync(stockHistory);

            return stockHistory;
        }

        public async Task<List<AOStockHistory>> GetStockHistoriesAsync(int daysBack)
        {
            var stockHistories = await _stockHistoryRepository.GetAllAsync(query =>
            {
                return from stockHistory in query
                       where stockHistory.CreatedDate > DateTime.UtcNow.AddDays(-daysBack)
                       orderby stockHistory.CreatedDate descending
                       select stockHistory;
            });

            return stockHistories.ToList();
        }

        public async Task<bool> StockHistoryExistForTodayAsync()
        {
            var stockHistory = await _stockHistoryRepository.GetAllAsync(query =>
            {
                return from stockHistory in query
                       where stockHistory.CreatedDate.Date ==  DateTime.UtcNow.Date                       
                       select stockHistory;
            });

            return stockHistory.Any();
        }
    }
}
