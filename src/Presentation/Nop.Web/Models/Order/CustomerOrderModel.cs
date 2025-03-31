using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Order;

public partial record CustomerOrderModel : BaseNopEntityModel
{
    public string CustomOrderNumber { get; set; }
    public string OrderTotal { get; set; }
    public bool IsReturnRequestAllowed { get; set; }
    public OrderStatus OrderStatusEnum { get; set; }
    public string OrderStatus { get; set; }
    public string PaymentStatus { get; set; }
    public string ShippingStatus { get; set; }
    public DateTime CreatedOn { get; set; }
}
