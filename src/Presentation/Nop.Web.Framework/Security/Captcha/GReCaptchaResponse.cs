using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Web.Framework.Security.Captcha
{
    public class GReCaptchaResponse
    {
        [JsonProperty(propertyName: "success", Required = Required.Always)]
        public bool IsValid { get; set; }

        [JsonProperty(propertyName: "error-codes", Required = Required.Default)]
        public IList<string> ErrorCodes { get; set; }

        public GReCaptchaResponse()
        {
            ErrorCodes = new List<string>();
        }
    }
}