using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Test.ProductProvider.Api.Models
{
    public class Response
    {
        /// <summary>
        /// Gets or sets the inventory balance before update
        /// </summary>
        [JsonProperty(PropertyName = "validationMessages")]
        public List<ValidationMessage> ValidationMessages { get; set; }
    }
}
