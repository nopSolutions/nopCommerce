using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Models
{
    public class Customer
    {
        public string Id { get; set; }

        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        public Phone Phone { get; set; }

        public string Description { get; set; }

        public bool LiveMode { get; set; }
    }
}
