using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class IncompleteOrdersChartModel : BaseNopModel
    {
        public IncompleteOrdersChartModel()
        {
            UnpaidOrders = new IncompleteOrderItemModel();
            NotYetShippedOrders = new IncompleteOrderItemModel();
            IncompleteOrders = new IncompleteOrderItemModel();
        }

        public IncompleteOrderItemModel UnpaidOrders { get; set; }

        public IncompleteOrderItemModel NotYetShippedOrders { get; set; }

        public IncompleteOrderItemModel IncompleteOrders { get; set; }
    }

    public partial class IncompleteOrderItemModel : BaseNopModel
    {
        public string ItemName { get; set; }

        public string Total { get; set; }

        public int Count { get; set; }

        public string ViewLink { get; set; }
    }
}