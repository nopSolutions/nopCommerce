//Contributor:  Nicholas Mayne


using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId.RelyingParty;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.OpenId.Core
{
    public class OpenIdProviderAuthorizer : IOpenIdProviderAuthorizer
    {
        private readonly IOpenIdRelyingPartyService _openIdRelyingPartyService;
        private readonly IExternalAuthorizer _authorizer;

        public OpenIdProviderAuthorizer(IOpenIdRelyingPartyService openIdRelyingPartyService,
            IExternalAuthorizer authorizer)
        {
            _openIdRelyingPartyService = openIdRelyingPartyService;
            _authorizer = authorizer;
        }

        /// <summary>
        /// Authorize response
        /// </summary>
        /// <param name="returnUrl">Return URL</param>
        /// <param name="verifyResponse">true - Verify response;false - request authentication;null - determine automatically</param>
        /// <returns>Authorize state</returns>
        public AuthorizeState Authorize(string returnUrl, bool? verifyResponse = null)
        {
            if (verifyResponse.HasValue ? verifyResponse.Value : IsOpenIdCallback)
                return VerifyAuthentication(returnUrl);

            return RequestAuthentication(returnUrl);
        }

        private AuthorizeState VerifyAuthentication(string returnUrl)
        {
            switch (_openIdRelyingPartyService.Response.Status)
            {
                case AuthenticationStatus.Authenticated:
                    var parameters = new OpenIdAuthenticationParameters(_openIdRelyingPartyService.Response);
                    return new AuthorizeState(returnUrl, _authorizer.Authorize(parameters));
                case AuthenticationStatus.Canceled:
                    {
                        var result = new AuthorizeState(returnUrl, OpenAuthenticationStatus.AssociateOnLogon);
                        return result;
                    }
                case AuthenticationStatus.Failed:
                    {
                        var result = new AuthorizeState(returnUrl, OpenAuthenticationStatus.Error);
                        result.AddError(_openIdRelyingPartyService.Response.Exception.Message);
                        return result;
                    }
            }
            return new AuthorizeState(returnUrl, OpenAuthenticationStatus.Unknown);
        }

        private AuthorizeState RequestAuthentication(string returnUrl)
        {
            var identifier = new OpenIdIdentifier(EnternalIdentifier);
            if (!identifier.IsValid)
            {
                var result = new AuthorizeState(returnUrl, OpenAuthenticationStatus.Error);
                result.AddError("Invalid Open ID identifier");
                return result;
            }

            try
            {
                var request = _openIdRelyingPartyService.CreateRequest(identifier);

                request.AddExtension(Claims.CreateClaimsRequest());
                request.AddExtension(Claims.CreateFetchRequest());

                return new AuthorizeState(returnUrl, OpenAuthenticationStatus.RequiresRedirect)
                {
                    Result = request.RedirectingResponse.AsActionResultMvc5()
                };
            }
            catch (ProtocolException ex)
            {
                var result = new AuthorizeState(returnUrl, OpenAuthenticationStatus.Error);
                result.AddError("Unable to authenticate: " + ex.Message);
                return result;
            }
        }

        public string EnternalIdentifier { get; set; }

        public bool IsOpenIdCallback
        {
            get { return _openIdRelyingPartyService.HasResponse; }
        }
    }
}