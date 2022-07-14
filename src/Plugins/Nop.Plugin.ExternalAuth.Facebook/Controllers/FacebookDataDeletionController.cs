using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.ExternalAuth.Facebook.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Logging;

namespace Nop.Plugin.ExternalAuth.Facebook.Controllers
{
    public class FacebookDataDeletionController : Controller
    {
        #region Fields

        private readonly FacebookExternalAuthSettings _facebookExternalAuthSettings;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public FacebookDataDeletionController(FacebookExternalAuthSettings facebookExternalAuthSettings,
            IExternalAuthenticationService externalAuthenticationService,
            ILogger logger,
            IWebHelper webHelper)
        {
            _facebookExternalAuthSettings = facebookExternalAuthSettings;
            _externalAuthenticationService = externalAuthenticationService;
            _logger = logger;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        // Convert string to a valid Base64 encoded string
        protected static string DecodeUrlBase64(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            str = str.Replace("-", "+").Replace("_", "/");
            var paddingToAdd = (str.Length % 4) == 3 ? 1 : (str.Length % 4);
            var charToAdd = new string('=', paddingToAdd);

            return str += charToAdd;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> DataDeletionCallback(IFormCollection form)
        {
            try
            {
                string signed_request = form["signed_request"];
                if (string.IsNullOrEmpty(signed_request))
                    throw new NopException("Request data is missing");

                var split = signed_request.Split('.');
                var signatureRaw = DecodeUrlBase64(split[0]);
                var dataRaw = DecodeUrlBase64(split[1]);
                if (string.IsNullOrEmpty(signatureRaw) || string.IsNullOrEmpty(dataRaw))
                    throw new NopException("Part of the request data is missing");

                var signature = Convert.FromBase64String(signatureRaw);
                var dataBuffer = Convert.FromBase64String(dataRaw);
                var json = Encoding.UTF8.GetString(dataBuffer);
                var appSecretBytes = Encoding.UTF8.GetBytes(_facebookExternalAuthSettings.ClientSecret);
                HMAC hmac = new HMACSHA256(appSecretBytes);
                var expectedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(split[1]));
                if (!expectedHash.SequenceEqual(signature))
                    throw new NopException("Hash validation failed");

                var fbUser = JsonConvert.DeserializeObject<FacebookUserDTO>(json);
                var authenticationParameters = new ExternalAuthenticationParameters
                {
                    ProviderSystemName = FacebookAuthenticationDefaults.SystemName,
                    AccessToken = await HttpContext.GetTokenAsync(FacebookDefaults.AuthenticationScheme, "access_token"),
                    ExternalIdentifier = fbUser.UserId
                };
                var externalAuthenticationRecord = await _externalAuthenticationService.GetExternalAuthenticationRecordByExternalAuthenticationParametersAsync(authenticationParameters);
                if (externalAuthenticationRecord is not null)
                {
                    await _logger.InformationAsync($"{FacebookAuthenticationDefaults.SystemName} data deletion completed. " +
                        $"CustomerId: {externalAuthenticationRecord.CustomerId}, " +
                        $"CustomerEmail: {externalAuthenticationRecord.Email}, " +
                        $"ExternalAuthenticationRecordId: {externalAuthenticationRecord.Id}");

                    await _externalAuthenticationService.DeleteExternalAuthenticationRecordAsync(externalAuthenticationRecord);
                }

                var notificationUrl = Url.RouteUrl(FacebookAuthenticationDefaults.DataDeletionStatusCheckRoute,
                    new { earId = externalAuthenticationRecord?.Id ?? 0 },
                    _webHelper.GetCurrentRequestProtocol());

                return Ok(new { url = notificationUrl, confirmation_code = $"{externalAuthenticationRecord?.Id ?? 0}" });
            }
            catch (Exception exception)
            {
                await _logger.ErrorAsync($"{FacebookAuthenticationDefaults.SystemName} data deletion error: {exception.Message}.", exception);
                return BadRequest();
            }
        }

        #endregion

    }
}