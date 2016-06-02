using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Customers
{
    public partial class CustomerStatisticsModel : BaseNopModel
    {
        public CustomerStatisticsModel()
        {
            ByMonthItems = new List<CustomerStatisticsItemModel>();
            ByYearItems = new List<CustomerStatisticsItemModel>();
        }

        public List<CustomerStatisticsItemModel> ByMonthItems { get; set; }

        public List<CustomerStatisticsItemModel> ByYearItems { get; set; }
    }

    public partial class CustomerStatisticsItemModel : BaseNopModel
    {
        public DateTime Date { get; set; }

        public string Value { get; set; }
    }
}