using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Nop.Web.Framework.Security.Captcha
{
    public class GReCaptchaValidator
    {
        private const string RECAPTCHA_VERIFY_URL_VERSION1 = "https://www.google.com/recaptcha/api/verify?privatekey={0}&response={1}&remoteip={2}&challenge={3}";
        private const string RECAPTCHA_VERIFY_URL_VERSION2 = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}&remoteip={2}";

        public string SecretKey { get; set; }
        public string RemoteIp { get; set; }
        public string Response { get; set; }
        public string Challenge { get; set; }

        private readonly ReCaptchaVersion _version;

        public GReCaptchaValidator(ReCaptchaVersion version = ReCaptchaVersion.Version1)
        {
            _version = version;
        }

        public GReCaptchaResponse Validate()
        {
            GReCaptchaResponse result = null;
            var httpClient = new HttpClient();
            var requestUri = string.Empty;

            switch (_version)
            {
                case ReCaptchaVersion.Version2:
                    requestUri = string.Format(RECAPTCHA_VERIFY_URL_VERSION2, SecretKey, Response, RemoteIp);
                    break;
                default:
                    requestUri = string.Format(RECAPTCHA_VERIFY_URL_VERSION1, SecretKey, Response, RemoteIp, Challenge);
                    break;
            }

            try
            {
                var taskResult = httpClient.GetAsync(requestUri);
                taskResult.Wait();
                var response = taskResult.Result;
                response.EnsureSuccessStatusCode();
                var taskString = response.Content.ReadAsStringAsync();
                taskString.Wait();
                result = ParseResponseResult(taskString.Result);
            }
            catch
            {
                result = new GReCaptchaResponse { IsValid = false };
                result.ErrorCodes.Add("Unknown error");
            }
            finally
            {
                httpClient.Dispose();
            }

            return result;
        }

        private GReCaptchaResponse ParseResponseResult(string responseString)
        {
            var result = new GReCaptchaResponse();

            if (_version == ReCaptchaVersion.Version1)
            {
                var resultObject = responseString.Split('\n');
                result.IsValid = resultObject.Contains("true");
                if(!result.IsValid)
                    result.ErrorCodes.AddRange(resultObject.Where(r => !r.Equals("false", StringComparison.InvariantCultureIgnoreCase)));
            }
            else if (_version == ReCaptchaVersion.Version2)
            {
                var resultObject = JObject.Parse(responseString);
                result.IsValid = resultObject.Value<bool>("success");
                if (resultObject.Value<JToken>("error-codes") != null &&
                        resultObject.Value<JToken>("error-codes").Values<string>().Any())
                        result.ErrorCodes = resultObject.Value<JToken>("error-codes").Values<string>().ToList();
            }

            return result;
        }
    }
}