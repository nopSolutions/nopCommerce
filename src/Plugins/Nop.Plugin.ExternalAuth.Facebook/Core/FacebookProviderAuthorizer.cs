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
        private readonly HttpContextBase _httpContext;

        private FacebookApplication _facebookApplication;

        public FacebookProviderAuthorizer(IExternalAuthorizer authorizer,
            IOpenAuthenticationService openAuthenticationService,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            FacebookExternalAuthSettings facebookExternalAuthSettings,
            HttpContextBase httpContext)
        {
            this._authorizer = authorizer;
            this._openAuthenticationService = openAuthenticationService;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._facebookExternalAuthSettings = facebookExternalAuthSettings;
            this._httpContext = httpContext;
        }

        private FacebookApplication FacebookApplication
        {
            get { return _facebookApplication ?? (_facebookApplication = new FacebookApplication(_facebookExternalAuthSettings.ClientKeyIdentifier, _facebookExternalAuthSettings.ClientSecret)); }
        }
        
        public AuthorizeState Authorize(string returnUrl)
        {
            FacebookOAuthResult oAuthResult;
            if (FacebookOAuthResult.TryParse(_httpContext.Request.Url, out oAuthResult))
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

                if (_externalAuthenticationSettings.AutoRegisterEnabled)
                    GetClaims(parameters);

                var result = _authorizer.Authorize(parameters);
                
                return new AuthorizeState(returnUrl, result);
            }

            var state = new AuthorizeState(returnUrl, OpenAuthenticationStatus.Error);
            state.AddError(string.Format("Reason: {0}, Description: {1}", oAuthResult.ErrorReason, oAuthResult.ErrorDescription));
            return state;
        }

        private void GetClaims(OAuthAuthenticationParameters parameters)
        {
            var client = new FacebookClient(parameters.OAuthAccessToken);
            var me = client.Get("/me");

            var claimsTranslator = new FacebookClaimsTranslator();
            var claims = claimsTranslator.Translate((IDictionary<string, object>)me);

            parameters.AddClaim(claims);
        }

        private AuthorizeState GenerateRequestState(string returnUrl)
        {
            var facebookClient = new FacebookOAuthClient(FacebookApplication);

            var extendedPermissions = new[] { "publish_stream", "read_stream", "offline_access", "email", "user_about_me" };
            var parameters = new Dictionary<string, object> {
                {"redirect_uri", GenerateCallbackUri() }
            };

            if (extendedPermissions.Length > 0)
            {
                var scope = new StringBuilder();
                scope.Append(string.Join(",", extendedPermissions));
                parameters["scope"] = scope.ToString();
            }

            var result = new RedirectResult(facebookClient.GetLoginUrl(parameters).ToString());

            return new AuthorizeState(returnUrl, OpenAuthenticationStatus.RequiresRedirect) { Result = result };
        }

        private Uri GenerateCallbackUri()
        {
            var builder = new UriBuilder(_httpContext.Request.Url.GetLeftPart(UriPartial.Authority));
            var path = _httpContext.Request.ApplicationPath + "/Plugins/ExternalAuthFacebook/Login/";
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
            var cl = new FacebookOAuthClient(FacebookApplication);
            cl.RedirectUri = GenerateCallbackUri();
            cl.AppId = FacebookApplication.AppId;
            cl.AppSecret = FacebookApplication.AppSecret;
            var dict = (JsonObject)cl.ExchangeCodeForAccessToken(code, new Dictionary<string, object> { { "permissions", "offline_access" } });

            return dict.Values.ElementAt(0).ToString();
        }
    }
}