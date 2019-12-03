using System.Collections.Generic;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Api.Services
{
    public interface IOrderItemApiService
    {
        IList<OrderItem> GetOrderItemsForOrder(Order order, int limit, int page, int sinceId);
        int GetOrderItemsCount(Order order);
    }
}