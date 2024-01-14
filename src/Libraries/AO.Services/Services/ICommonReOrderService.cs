using AO.Services.Orders.Models;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public interface ICommonReOrderService
    {             
        Task RemoveFromReOrderListAsync(int orquantityToOrderderId, int orderItemId);
                
        Task ReAddToReOrderListAsync(int orderItemId);

        Task<int> ChangeQuantityAsync(int reOrderItemId, int quantity);                

        Task DeleteReOrderItemAsync(int reOrderItemId);

        Task<AOReOrderItem> GetReOrderItemAsync(int reOrderItemId);

        Task<AOReOrderItem> GetReOrderItemByOrderItemIdAsync(int orderItemId);
    }
}