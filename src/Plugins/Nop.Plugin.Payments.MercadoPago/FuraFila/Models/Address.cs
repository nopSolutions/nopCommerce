using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Models
{
    public class Address
    {
        /// <summary>
        /// rua
        /// </summary>
        [JsonProperty(PropertyName = "street_name")]
        public string StreetName { get; set; }

        /// <summary>
        /// O numero
        /// </summary>
        [JsonProperty(PropertyName = "street_number")]
        public int? StreetNumber { get; set; }

        /// <summary>
        /// Codigo postal
        /// </summary>
        [JsonProperty(PropertyName = "zip_code")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Apartamento
        /// </summary>
        public string Floor { get; set; }

        /// <summary>
        /// Apartamento
        /// </summary>
        public string Apartment { get; set; }
    }
}
