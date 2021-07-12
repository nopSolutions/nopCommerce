using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Common.PushApiTask
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PushApiTaskReceiptRequest
    {
        
        [JsonProperty(PropertyName ="ids")]
        public List<string> PushTicketIds { get; set; }
    }
}
