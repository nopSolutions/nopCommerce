using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Home
{
    public partial class DayStatisticsModel : BaseNopModel
    {
        public bool IsLoggedInAsVendor { get; set; }

        public int NumberOfOrders { get; set; }

        public int NumberOfCustomers { get; set; }

        public int NumberOfPendingReturnRequests { get; set; }

        public int NumberOfLowStockProducts { get; set; }
    }
}