using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Models
{
    public class Item
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "picture_url")]
        public string PictureUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [JsonProperty(PropertyName = "category_id")]
        public string CategoryId { get; set; }

        [JsonProperty(PropertyName = "currency_id")]
        public string CurrencyId { get; set; }

        public int Quantity { get; set; }

        [JsonProperty(PropertyName = "unit_price")]
        public decimal UnitPrice { get; set; }
    }
}
