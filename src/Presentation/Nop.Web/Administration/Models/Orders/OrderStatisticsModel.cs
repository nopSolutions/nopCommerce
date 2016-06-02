using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class OrderStatisticsModel : BaseNopModel
    {
        public OrderStatisticsModel()
        {
            ByMonthItems = new List<OrderStatisticsItemModel>();
            ByYearItems = new List<OrderStatisticsItemModel>();
        }

        public List<OrderStatisticsItemModel> ByMonthItems { get; set; }

        public List<OrderStatisticsItemModel> ByYearItems { get; set; }
    }

    public partial class OrderStatisticsItemModel : BaseNopModel
    {
        public DateTime Date { get; set; }

        public string Value { get; set; }
    }
}