using Nop.Core.Domain.Orders;
using Nop.Web.Models.Order;

namespace Nop.Web.Factories
{
    public partial interface IReturnRequestModelFactory
    {
        SubmitReturnRequestModel.OrderItemModel PrepareSubmitReturnRequestOrderItemModel(OrderItem orderItem);

        SubmitReturnRequestModel PrepareSubmitReturnRequestModel(SubmitReturnRequestModel model, Order order);

        CustomerReturnRequestsModel PrepareCustomerReturnRequestsModel();
    }
}
