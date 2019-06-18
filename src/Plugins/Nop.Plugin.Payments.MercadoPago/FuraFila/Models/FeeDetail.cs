using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Models
{
    public class FeeDetail
    {
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName ="fee_payer")]
        public string FeePayer { get; set; }

        public string Type { get; set; }
    }
}
