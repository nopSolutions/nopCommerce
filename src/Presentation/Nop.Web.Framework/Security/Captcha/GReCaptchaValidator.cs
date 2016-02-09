using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Nop.Web.Framework.Security.Captcha
{
    public class GReCaptchaValidator
    {
        public string SecretKey { get; set; }
        public string RemoteIp { get; set; }
        public string Response { get; set; }

        public GReCaptchaResponse Validate()
        {
            var uriBuilder = new UriBuilder("https://www.google.com/recaptcha/api/siteverify")
            {
                Query = string.Format("secret={0}&response={1}&remoteip={2}", SecretKey, Response, RemoteIp)
            };
            var result = new GReCaptchaResponse();
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var taskResult = httpClient.GetAsync(uriBuilder.Uri);
                    taskResult.Wait();
                    var response = taskResult.Result;
                    response.EnsureSuccessStatusCode();
                    var taskString = response.Content.ReadAsStringAsync();
                    taskString.Wait();
                    var resultObject = JObject.Parse(taskString.Result);
                    result.IsValid = resultObject.Value<bool>("success");
                    if (resultObject.Value<JToken>("error-codes") != null &&
                        resultObject.Value<JToken>("error-codes").Values<string>().Any())
                        result.ErrorCodes = resultObject.Value<JToken>("error-codes").Values<string>().ToList();
                }
                catch (Exception ex)
                {
                    result.IsValid = false;
                    result.ErrorCodes.Add("Unknown error");
                }
            }

            return result;
        }
    }
}