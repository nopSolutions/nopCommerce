using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Models
{
    public class Payer
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        [JsonProperty(PropertyName = "date_created")]
        public string DateCreated { get; set; }

        public Phone Phone { get; set; }

        public Identification Identification { get; set; }

        public Address Address { get; set; }
    }
}
