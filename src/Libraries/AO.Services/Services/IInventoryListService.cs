using AO.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public interface IInventoryListService
    {
        Task<InventoryListModel> GetInventoryListAsync();

        IList<InventoryStockCountedItem> GetInventoryListWithStockCountedDate();
    }
}