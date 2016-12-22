using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Web.Models.Order;

namespace Nop.Web.Factories
{
    public partial interface IOrderModelFactory
    {
        CustomerOrderListModel PrepareCustomerOrderListModel();

        OrderDetailsModel PrepareOrderDetailsModel(Order order);

        ShipmentDetailsModel PrepareShipmentDetailsModel(Shipment shipment);

        CustomerRewardPointsModel PrepareCustomerRewardPoints(int? page);
    }
}
