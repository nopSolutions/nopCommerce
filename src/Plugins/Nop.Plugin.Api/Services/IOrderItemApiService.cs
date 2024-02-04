using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Api.Services
{
    public interface IOrderItemApiService
    {
        Task<IList<OrderItem>> GetOrderItemsForOrderAsync(Order order, int limit, int page, int sinceId);
        Task<int> GetOrderItemsCountAsync(Order order);
    }
}
