using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public class OrderStatisticsModel
    {
        [JsonProperty("metrics")]
        public List<OrderMetric> Metrics { get; set; } = new List<OrderMetric>();

        [JsonProperty("vendorsNames")]
        public List<string> VendorsNames { get; set; } = new List<string>();
    }

    public class OrderMetric
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("totoalOrders")]
        public string TotalOrders { get; set; }

        [JsonProperty("vendorsShare")]
        public Dictionary<string, double> VendorsShare { get; set; }
    }
}
