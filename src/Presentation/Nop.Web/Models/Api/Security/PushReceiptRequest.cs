using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Web.Models.Api.Security
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PushReceiptRequest
    {
        
        [JsonProperty(PropertyName ="ids")]
        public List<string> PushTicketIds { get; set; }
    }
}
