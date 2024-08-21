using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Order;

public partial record CustomerOrderListModel : BaseNopModel
{
    public CustomerOrderListModel()
    {
        Orders = new List<OrderDetailsModel>();
        RecurringOrders = new List<RecurringOrderModel>();
        RecurringPaymentErrors = new List<string>();
    }

    public IList<OrderDetailsModel> Orders { get; set; }
    public IList<RecurringOrderModel> RecurringOrders { get; set; }
    public IList<string> RecurringPaymentErrors { get; set; }

    #region Nested classes

    public partial record OrderDetailsModel : BaseNopEntityModel
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

    public partial record RecurringOrderModel : BaseNopEntityModel
    {
        public string StartDate { get; set; }
        public string CycleInfo { get; set; }
        public string NextPayment { get; set; }
        public int TotalCycles { get; set; }
        public int CyclesRemaining { get; set; }
        public int InitialOrderId { get; set; }
        public bool CanRetryLastPayment { get; set; }
        public string InitialOrderNumber { get; set; }
        public bool CanCancel { get; set; }
    }

    #endregion
}