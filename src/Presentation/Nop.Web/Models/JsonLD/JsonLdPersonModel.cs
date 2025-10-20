﻿using Newtonsoft.Json;

namespace Nop.Web.Models.JsonLD;

public partial record JsonLdPersonModel : JsonLdModel
{
    #region Properties

    [JsonProperty("@type")]
    public static string Type => "Person";

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; } 

    #endregion
}