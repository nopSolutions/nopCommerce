using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Models
{
    public class TransactionDetails
    {
        [JsonProperty(PropertyName = "total_paid_amount")]
        public decimal? TotalPaidAmount { get; set; }

        [JsonProperty(PropertyName = "installment_amount")]
        public int? InstallmentAmount { get; set; }

        [JsonProperty(PropertyName = "net_received_amount")]
        public decimal? NetReceivedAmount { get; set; }

        [JsonProperty(PropertyName = "overpaid_amount")]
        public decimal? OverpaidAmount { get; set; }
    }
}
