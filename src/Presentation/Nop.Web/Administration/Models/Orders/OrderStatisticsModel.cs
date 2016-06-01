using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class OrderStatisticsModel : BaseNopModel
    {
        public OrderStatisticsModel()
        {
            Month = new List<OrderStatisticsItemModel>();
            Year = new List<OrderStatisticsItemModel>();
        }

        public List<OrderStatisticsItemModel> Month { get; set; }

        public List<OrderStatisticsItemModel> Year { get; set; }
    }

    public partial class OrderStatisticsItemModel : BaseNopModel
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}