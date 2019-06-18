using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Models
{
    public class PaymentMethods
    {
        [JsonProperty(PropertyName = "excluded_payment_methods")]
        public PaymentType[] ExcludedPaymentMethods { get; set; }

        [JsonProperty(PropertyName = "excluded_payment_types")]
        public PaymentType[] ExcludedPaymentTypes { get; set; }

        public int? Installments { get; set; }

        [JsonProperty(PropertyName = "default_payment_method_id")]
        public string DefaultPaymentMethodId { get; set; }

        [JsonProperty(PropertyName = "default_installments")]
        public int? DefaultInstallments { get; set; }
    }
}
