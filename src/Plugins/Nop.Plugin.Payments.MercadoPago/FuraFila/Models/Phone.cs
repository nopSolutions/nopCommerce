using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Models
{
    public class Phone
    {
        [JsonProperty("area_code")]
        public string AreaCode { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }
    }
}
