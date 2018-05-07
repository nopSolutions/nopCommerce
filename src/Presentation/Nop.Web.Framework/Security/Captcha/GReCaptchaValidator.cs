using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Nop.Web.Framework.Security.Captcha
{
    /// <summary>
    /// Google reCAPTCHA validator
    /// </summary>
    public class GReCaptchaValidator
    {
        private const string RECAPTCHA_VERIFY_URL = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}&remoteip={2}";

        /// <summary>
        /// reCAPTCHA secret key
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// Remove IP address
        /// </summary>
        public string RemoteIp { get; set; }
        /// <summary>
        /// Response
        /// </summary>
        public string Response { get; set; }
        /// <summary>
        /// Challenge
        /// </summary>
        public string Challenge { get; set; }

        /// <summary>
        /// Parse response
        /// </summary>
        /// <param name="responseString">Response (string)</param>
        /// <returns>Parsed response</returns>
        private GReCaptchaResponse ParseResponseResult(string responseString)
        {
            var result = new GReCaptchaResponse();

            var resultObject = JObject.Parse(responseString);
            result.IsValid = resultObject.Value<bool>("success");
            if (resultObject.Value<JToken>("error-codes") != null &&
                resultObject.Value<JToken>("error-codes").Values<string>().Any())
                result.ErrorCodes = resultObject.Value<JToken>("error-codes").Values<string>().ToList();

            return result;
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <returns></returns>
        public GReCaptchaResponse Validate()
        {
            GReCaptchaResponse result = null;
            var httpClient = new HttpClient();
            var requestUri = string.Empty;
            requestUri = string.Format(RECAPTCHA_VERIFY_URL, SecretKey, Response, RemoteIp);

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
    }
}