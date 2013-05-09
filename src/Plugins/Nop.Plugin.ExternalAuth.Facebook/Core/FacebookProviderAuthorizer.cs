using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Facebook.Core
{
    public class FacebookProviderAuthorizer : IOAuthProviderFacebookAuthorizer
    {
        #region Fields

        private readonly IExternalAuthorizer _authorizer;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly FacebookExternalAuthSettings _facebookExternalAuthSettings;
        private readonly HttpContextBase _httpContext;
        private readonly IWebHelper _webHelper;
        private FacebookClient _facebookApplication;

        #endregion

        #region Ctor

        public FacebookProviderAuthorizer(IExternalAuthorizer authorizer,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            FacebookExternalAuthSettings facebookExternalAuthSettings,
            HttpContextBase httpContext,
            IWebHelper webHelper)
        {
            this._authorizer = authorizer;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._facebookExternalAuthSettings = facebookExternalAuthSettings;
            this._httpContext = httpContext;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities

        private FacebookClient FacebookApplication
        {
            get { return _facebookApplication ?? (_facebookApplication = new FacebookClient(_facebookExternalAuthSettings.ClientKeyIdentifier, _facebookExternalAuthSettings.ClientSecret)); }
        }

        private AuthorizeState VerifyAuthentication(string returnUrl)
        {
            var authResult = this.FacebookApplication.VerifyAuthentication(_httpContext, GenerateLocalCallbackUri());

            if (authResult.IsSuccessful)
            {
                if (!authResult.ExtraData.ContainsKey("id"))
                    throw new Exception("Authentication result does not contain id data");

                if (!authResult.ExtraData.ContainsKey("accesstoken"))
                    throw new Exception("Authentication result does not contain accesstoken data");

                var parameters = new OAuthAuthenticationParameters(Provider.SystemName)
                {
                    ExternalIdentifier = authResult.ProviderUserId,
                    OAuthToken = authResult.ExtraData["accesstoken"],
                    OAuthAccessToken = authResult.ProviderUserId,
                };

                if (_externalAuthenticationSettings.AutoRegisterEnabled)
                    ParseClaims(authResult, parameters);

                var result = _authorizer.Authorize(parameters);

                return new AuthorizeState(returnUrl, result);
            }

            var state = new AuthorizeState(returnUrl, OpenAuthenticationStatus.Error);
            var error = authResult.Error != null ? authResult.Error.Message : "Unknown error";
            state.AddError(error);
            return state;
        }

        private void ParseClaims(AuthenticationResult authenticationResult, OAuthAuthenticationParameters parameters)
        {
            var claims = new UserClaims();
            claims.Contact = new ContactClaims();
            if (authenticationResult.ExtraData.ContainsKey("username"))
                claims.Contact.Email = authenticationResult.ExtraData["username"];
            claims.Name = new NameClaims();
            if (authenticationResult.ExtraData.ContainsKey("name"))
            {
                var name = authenticationResult.ExtraData["name"];
                if (!String.IsNullOrEmpty(name))
                {
                    var nameSplit = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (nameSplit.Length >= 2)
                    {
                        claims.Name.First = nameSplit[0];
                        claims.Name.Last = nameSplit[1];
                    }
                    else
                    {
                        claims.Name.Last = nameSplit[0];
                    }
                }
            }

            parameters.AddClaim(claims);
        }

        private AuthorizeState RequestAuthentication(string returnUrl)
        {
            var authUrl = GenerateServiceLoginUrl().AbsoluteUri;
            return new AuthorizeState("", OpenAuthenticationStatus.RequiresRedirect) { Result = new RedirectResult(authUrl) };
        }

        private Uri GenerateLocalCallbackUri()
        {
            string url = string.Format("{0}plugins/externalauthFacebook/logincallback/", _webHelper.GetStoreLocation());
            return new Uri(url);
            //var builder = new UriBuilder(_httpContext.Request.Url.GetLeftPart(UriPartial.Authority));
            //var path = _httpContext.Request.ApplicationPath + "/Plugins/ExternalAuthFacebook/LoginCallback/";
            //builder.Path = path.Replace(@"//", @"/");
            //return builder.Uri;
        }

        private Uri GenerateServiceLoginUrl()
        {
            //code copied from DotNetOpenAuth.AspNet.Clients.FacebookClient file
            var builder = new UriBuilder("https://www.facebook.com/dialog/oauth");
            var args = new Dictionary<string, string>();
            args.Add("client_id", _facebookExternalAuthSettings.ClientKeyIdentifier);
            args.Add("redirect_uri", GenerateLocalCallbackUri().AbsoluteUri);
            args.Add("scope", "email");
            AppendQueryArgs(builder, args);
            return builder.Uri;

            //var callBackUrl = GenerateLocalCallbackUri().AbsoluteUri;
            //var authUrl = _webHelper.ModifyQueryString("https://www.facebook.com/dialog/oauth", "client_id=" + this._facebookExternalAuthSettings.ClientKeyIdentifier, null);
            //authUrl = _webHelper.ModifyQueryString(authUrl, "redirect_uri=" + HttpUtility.UrlEncode(callBackUrl), null);
            //return new Uri(authUrl);
        }

        private void AppendQueryArgs(UriBuilder builder, IEnumerable<KeyValuePair<string, string>> args)
        {
            if ((args != null) && (args.Count<KeyValuePair<string, string>>() > 0))
            {
                StringBuilder builder2 = new StringBuilder(50 + (args.Count<KeyValuePair<string, string>>() * 10));
                if (!string.IsNullOrEmpty(builder.Query))
                {
                    builder2.Append(builder.Query.Substring(1));
                    builder2.Append('&');
                }
                builder2.Append(CreateQueryString(args));
                builder.Query = builder2.ToString();
            }
        }
        private string CreateQueryString(IEnumerable<KeyValuePair<string, string>> args)
        {
            if (!args.Any<KeyValuePair<string, string>>())
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder(args.Count<KeyValuePair<string, string>>() * 10);
            foreach (KeyValuePair<string, string> pair in args)
            {
                builder.Append(EscapeUriDataStringRfc3986(pair.Key));
                builder.Append('=');
                builder.Append(EscapeUriDataStringRfc3986(pair.Value));
                builder.Append('&');
            }
            builder.Length--;
            return builder.ToString();
        }
        private readonly string[] UriRfc3986CharsToEscape = new string[] { "!", "*", "'", "(", ")" };
        private string EscapeUriDataStringRfc3986(string value)
        {
            StringBuilder builder = new StringBuilder(Uri.EscapeDataString(value));
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                builder.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }
            return builder.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Authorize response
        /// </summary>
        /// <param name="returnUrl">Return URL</param>
        /// <param name="verifyResponse">true - Verify response;false - request authentication;null - determine automatically</param>
        /// <returns>Authorize state</returns>
        public AuthorizeState Authorize(string returnUrl, bool? verifyResponse = null)
        {
            if (!verifyResponse.HasValue)
                throw new ArgumentException("Facebook plugin cannot automatically determine verifyResponse property");

            if (verifyResponse.Value)
            {
                return VerifyAuthentication(returnUrl);
            }
            else
            {
                return RequestAuthentication(returnUrl);
            }
        }

        #endregion
    }
}