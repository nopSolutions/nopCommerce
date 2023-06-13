using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.GoogleAuth.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Logging;

namespace Nop.Plugin.GoogleAuth.Controllers
{
    public class GoogleDataDeletionController : Controller
    {
        #region Fields

        private readonly GoogleExternalAuthSettings _googleExternalAuthSettings;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public GoogleDataDeletionController(GoogleExternalAuthSettings googleExternalAuthSettings,
            IExternalAuthenticationService externalAuthenticationService,
            ILogger logger,
            IWebHelper webHelper)
        {
            _googleExternalAuthSettings = googleExternalAuthSettings;
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
                var appSecretBytes = Encoding.UTF8.GetBytes(_googleExternalAuthSettings.ClientSecret);
                HMAC hmac = new HMACSHA256(appSecretBytes);
                var expectedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(split[1]));
                if (!expectedHash.SequenceEqual(signature))
                    throw new NopException("Hash validation failed");

                var fbUser = JsonConvert.DeserializeObject<GoogleUserDTO>(json);
                var authenticationParameters = new ExternalAuthenticationParameters
                {
                    ProviderSystemName = GoogleAuthenticationDefaults.SystemName,
                    AccessToken = await HttpContext.GetTokenAsync(GoogleDefaults.AuthenticationScheme, "access_token"),
                    ExternalIdentifier = fbUser.UserId
                };
                var externalAuthenticationRecord = await _externalAuthenticationService.GetExternalAuthenticationRecordByExternalAuthenticationParametersAsync(authenticationParameters);
                if (externalAuthenticationRecord is not null)
                {
                    await _logger.InformationAsync($"{GoogleAuthenticationDefaults.SystemName} data deletion completed. " +
                        $"CustomerId: {externalAuthenticationRecord.CustomerId}, " +
                        $"CustomerEmail: {externalAuthenticationRecord.Email}, " +
                        $"ExternalAuthenticationRecordId: {externalAuthenticationRecord.Id}");

                    await _externalAuthenticationService.DeleteExternalAuthenticationRecordAsync(externalAuthenticationRecord);
                }

                var notificationUrl = Url.RouteUrl(GoogleAuthenticationDefaults.DataDeletionStatusCheckRoute,
                    new { earId = externalAuthenticationRecord?.Id ?? 0 },
                    _webHelper.GetCurrentRequestProtocol());

                return Ok(new { url = notificationUrl, confirmation_code = $"{externalAuthenticationRecord?.Id ?? 0}" });
            }
            catch (Exception exception)
            {
                await _logger.ErrorAsync($"{GoogleAuthenticationDefaults.SystemName} data deletion error: {exception.Message}.", exception);
                return BadRequest();
            }
        }

        #endregion

    }
}