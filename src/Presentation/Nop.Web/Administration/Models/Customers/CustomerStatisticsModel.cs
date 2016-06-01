using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Customers
{
    public partial class CustomerStatisticsModel : BaseNopModel
    {
        public CustomerStatisticsModel()
        {
            Month = new List<CustomerStatisticsItemModel>();
            Year = new List<CustomerStatisticsItemModel>();
        }

        public List<CustomerStatisticsItemModel> Month { get; set; }

        public List<CustomerStatisticsItemModel> Year { get; set; }
    }

    public partial class CustomerStatisticsItemModel : BaseNopModel
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}