//Contributor:  Nicholas Mayne

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Facebook;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Facebook.Core
{
    public class FacebookProviderAuthorizer : IOAuthProviderFacebookAuthorizer
    {
        private readonly IExternalAuthorizer _authorizer;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly FacebookExternalAuthSettings _facebookExternalAuthSettings;
        private readonly HttpContextBase _httpContextBase;

        private FacebookApplication _facebookApplication;

        public FacebookProviderAuthorizer(IExternalAuthorizer authorizer,
            IOpenAuthenticationService openAuthenticationService,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            FacebookExternalAuthSettings facebookExternalAuthSettings,
            HttpContextBase httpContextBase)
        {
            this._authorizer = authorizer;
            this._openAuthenticationService = openAuthenticationService;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._facebookExternalAuthSettings = facebookExternalAuthSettings;
            this._httpContextBase = httpContextBase;
        }

        private FacebookApplication FacebookApplication
        {
            get { return _facebookApplication ?? (_facebookApplication = new FacebookApplication(_facebookExternalAuthSettings.ClientKeyIdentifier, _facebookExternalAuthSettings.ClientSecret)); }
        }
        
        public AuthorizeState Authorize(string returnUrl)
        {
            FacebookOAuthResult oAuthResult;
            if (FacebookOAuthResult.TryParse(HttpContext.Current.Request.Url, out oAuthResult))
            {
                return TranslateResponseState(returnUrl, oAuthResult);
            }

            return GenerateRequestState(returnUrl);
        }

        private AuthorizeState TranslateResponseState(string returnUrl, FacebookOAuthResult oAuthResult)
        {
            if (oAuthResult.IsSuccess)
            {
                var parameters = new OAuthAuthenticationParameters(Provider.SystemName)
                {
                    ExternalIdentifier = GetAccessToken(oAuthResult.Code),
                    OAuthToken = oAuthResult.Code,
                    OAuthAccessToken = GetAccessToken(oAuthResult.Code)
                };

                var result = _authorizer.Authorize(parameters);

                if (result.Status == OpenAuthenticationStatus.AssociateOnLogon)
                {
                    if (_externalAuthenticationSettings.AutoRegisterEnabled)
                        result = GetUserNameAndRetryAuthorization(parameters);
                }

                return new AuthorizeState(returnUrl, result);
            }

            var state = new AuthorizeState(returnUrl, OpenAuthenticationStatus.Error);
            state.AddError(string.Format("Reason: {0}, Description: {1}", oAuthResult.ErrorReason, oAuthResult.ErrorDescription));
            return state;
        }

        private AuthorizationResult GetUserNameAndRetryAuthorization(OAuthAuthenticationParameters parameters)
        {
            var client = new FacebookClient(parameters.OAuthAccessToken);
            var me = client.Get("/me");

            var claimsTranslator = new FacebookClaimsTranslator();
            var claims = claimsTranslator.Translate((IDictionary<string, object>)me);

            parameters.AddClaim(claims);

            return _authorizer.Authorize(parameters);
        }

        private AuthorizeState GenerateRequestState(string returnUrl)
        {
            var facebookClient = new FacebookOAuthClient(FacebookApplication);

            var extendedPermissions = new[] { "publish_stream", "read_stream", "offline_access", "email" };
            var parameters = new Dictionary<string, object> {
                {"redirect_uri", GenerateCallbackUri() }
            };

            if (extendedPermissions != null && extendedPermissions.Length > 0)
            {
                var scope = new StringBuilder();
                scope.Append(string.Join(",", extendedPermissions));
                parameters["scope"] = scope.ToString();
            }

            var result = new RedirectResult(facebookClient.GetLoginUrl(parameters).ToString());

            return new AuthorizeState(returnUrl, OpenAuthenticationStatus.RequresRedirect) { Result = result };
        }

        private Uri GenerateCallbackUri()
        {
            UriBuilder builder = new UriBuilder(_httpContextBase.Request.Url.GetLeftPart(UriPartial.Authority));
            var path = _httpContextBase.Request.ApplicationPath + "/Plugins/ExternalAuthFacebook/Login/";
            builder.Path = path.Replace(@"//", @"/");

            return builder.Uri;
        }

        public FacebookClient GetClient(Customer customer)
        {
            var parameters = new OAuthAuthenticationParameters(Provider.SystemName);
            var identifier = _openAuthenticationService
                .GetExternalIdentifiersFor(customer)
                .Where(o => o.ProviderSystemName == parameters.ProviderSystemName)
                .ToList()
                .FirstOrDefault();

            return !string.IsNullOrEmpty(identifier.OAuthAccessToken) ? new FacebookClient(identifier.OAuthAccessToken) : null;
        }

        private string GetAccessToken(string code)
        {
            FacebookOAuthClient cl = new FacebookOAuthClient(FacebookApplication);
            cl.RedirectUri = GenerateCallbackUri();
            cl.AppId = FacebookApplication.AppId;
            cl.AppSecret = FacebookApplication.AppSecret;
            JsonObject dict = (JsonObject)cl.ExchangeCodeForAccessToken(code, new Dictionary<string, object> { { "permissions", "offline_access" } });

            return dict.Values.ElementAt(0).ToString();
        }
    }
}