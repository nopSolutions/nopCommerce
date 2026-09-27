using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.WorldpayHpp.Models.Response;
public class Links {
    [JsonPropertyName("self")]
    public Self Self { get; set; }
}