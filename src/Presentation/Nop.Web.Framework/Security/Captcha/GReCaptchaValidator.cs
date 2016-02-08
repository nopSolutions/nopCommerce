using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Nop.Web.Framework.Security.Captcha
{
    public class GReCaptchaValidator
    {
        public string SecretKey { get; set; }
        public string RemoteIp { get; set; }
        public string Response { get; set; }

        public GReCaptchaValidator()
        {
            
        }

        public GReCaptchaValidator(string secretKey, string remoteIp, string response)
        {
            SecretKey = secretKey;
            RemoteIp = remoteIp;
            Response = response;
        }

        public GReCaptchaResponse Validate()
        {
            var uriBuilder = new UriBuilder("https://www.google.com/recaptcha/api/siteverify")
            {
                Query = $"secret={SecretKey}&response={Response}&remoteip={RemoteIp}"
            };
            var result = new GReCaptchaResponse();
            using (var httpClient = new HttpClient())
            {
                var taskResult = httpClient.GetAsync(uriBuilder.Uri);
                taskResult.Wait();
                var response = taskResult.Result;
                response.EnsureSuccessStatusCode();
                var taskString = response.Content.ReadAsStringAsync();
                taskString.Wait();
                var resultObject = JObject.Parse(taskString.Result);
                result.IsValid = resultObject.Value<bool>("success");
                result.ErrorCodes = resultObject.Value<JToken>("error-codes")?.Values<string>()?.ToList();
            }

            return result;
        }
    }
}