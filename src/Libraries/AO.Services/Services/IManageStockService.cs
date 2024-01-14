using AO.Services.Services.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public interface IManageStockService
    {
        Task<IList<Order>> IsCombinationOnOrderListAsync(ProductAttributeCombination combination);

        Task<ProductAttributeCombination> SearchByEANAsync(string ean);

        Task<ManageStockResultModel> UpdateStockCountAsync(int changeStockBy, ProductAttributeCombination combination, Product product);

        Task<string> GetStockPlacementAsync(int productId);
        Task<ManageStockResultModel> ResetProductStockAsync(Product product);
    }
}