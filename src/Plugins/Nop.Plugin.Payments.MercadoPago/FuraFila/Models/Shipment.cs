using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Models
{
    public class Shipment
    {
        [JsonProperty(PropertyName = "receiver_address")]
        public Address ReceiverAddress { get; set; }

        /// <summary>
        /// Modo de envio.
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Preferencia de remoção de pacotes em agencia (mode: me2 somente)
        /// </summary>
        [JsonProperty(PropertyName = "local_pickup")]
        public bool LocalPickup { get; set; }

        /// <summary>
        /// Tamanho do pacote em cm x cm x cm, gr (mode:me2 somente)
        /// </summary>
        public string Dimensions { get; set; }

        /// <summary>
        /// Escolha um metodo de envio padrao no _check_(mode:me2 somente)
        /// </summary>
        [JsonProperty(PropertyName ="default_shipping_method")]
        public int? DefaultShippingMethod { get; set; }

        /// <summary>
        /// Custo do transporte (mode:custom somente)
        /// </summary>
        public decimal? Cost { get; set; }

        /// <summary>
        /// Preferencia de frente grátis para mode:custom
        /// </summary>
        [JsonProperty(PropertyName ="free_shipping")]
        public bool FreeShipping { get; set; }


    }
}
