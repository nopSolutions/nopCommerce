using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.MercadoPago.FuraFila.Models;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Preferences
{
    public class PreferenceRequest
    {
        public List<Item> Items { get; set; }

        public Payer Payer { get; set; }

        [JsonProperty(PropertyName = "payment_methods")]
        public PaymentMethods PaymentMethods { get; set; }

        [JsonProperty(PropertyName = "back_urls")]
        public BackUrls BackUrls { get; set; }

        [JsonProperty(PropertyName = "notification_url")]
        public string NotificationUrl { get; set; }

        [JsonProperty(PropertyName = "auto_return")]
        public string AutoReturn { get; set; }

        public Shipment Shipments { get; set; }

        /// <summary>
        /// Referencia que pode sincronizar com seu sistema de pagamentos
        /// </summary>
        [JsonProperty(PropertyName = "external_reference")]
        public string ExternalReference { get; set; }

        /// <summary>
        /// Informacoes adicionais
        /// </summary>
        [JsonProperty(PropertyName ="additional_info")]
        public string AdditionalInfo { get; set; }

        public override string ToString()
        {
            var builder = PooledStringBuilder.Create();
            builder.Append(GetType().Name).Append("(");

            builder.Append("Reference=").Append(ExternalReference).Append(", ");

            string email = Payer?.Email;
            builder.Append("Payer.Email=").Append(email).Append(")");

            return builder.ToStringAndReturn();
        }
    }
}
