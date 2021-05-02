using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Web.Models.Api.Security
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PushResceiptResponse
    {
        [JsonProperty(PropertyName = "data")]
        public Dictionary<string,PushTicketDeliveryStatus> PushTicketReceipts { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public List<PushReceiptErrorInformation> ErrorInformations { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class PushTicketDeliveryStatus
    {
        [JsonProperty(PropertyName = "status")]
        public string DeliveryStatus { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string DeliveryMessage { get; set; }

        [JsonProperty(PropertyName = "details")]
        public object DeliveryDetails { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class PushReceiptErrorInformation
    {
        [JsonProperty(PropertyName = "code")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string ErrorMessage { get; set; }
    }

}
