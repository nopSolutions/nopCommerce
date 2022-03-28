using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Services.Shipping;

namespace Nop.Plugin.Misc.AbcCore.HomeDelivery
{
    public interface IHomeDeliveryCostService
    {
        Task<decimal> GetHomeDeliveryCostAsync(IList<OrderItem> orderItems);
        Task<decimal> GetHomeDeliveryCostAsync(IList<GetShippingOptionRequest.PackageItem> packageItems);
    }
}
