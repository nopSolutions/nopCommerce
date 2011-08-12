//Contributor:  Nicholas Mayne


using System.Collections.Generic;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId.RelyingParty;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.OpenId.Core
{
    public class OpenIdProviderAuthorizer : IOpenIdProviderAuthorizer
    {
        private readonly IOpenIdRelyingPartyService _openIdRelyingPartyService;
        private readonly IExternalAuthorizer _authorizer;
        private readonly IOpenAuthenticationProviderPermissionService _openAuthenticationProviderPermissionService;

        public OpenIdProviderAuthorizer(IOpenIdRelyingPartyService openIdRelyingPartyService,
            IExternalAuthorizer authorizer,
            IOpenAuthenticationProviderPermissionService openAuthenticationProviderPermissionService)
        {
            _openIdRelyingPartyService = openIdRelyingPartyService;
            _authorizer = authorizer;
            _openAuthenticationProviderPermissionService = openAuthenticationProviderPermissionService;
        }

        public AuthorizeState Authorize(string returnUrl)
        {
            if (IsOpenIdCallback)
                return TranslateResponseState(returnUrl);

            return GenerateRequestState(returnUrl);
        }

        private AuthorizeState TranslateResponseState(string returnUrl)
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

        private AuthorizeState GenerateRequestState(string returnUrl)
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

                request.AddExtension(Claims.CreateClaimsRequest(_openAuthenticationProviderPermissionService));
                request.AddExtension(Claims.CreateFetchRequest(_openAuthenticationProviderPermissionService));

                return new AuthorizeState(returnUrl, OpenAuthenticationStatus.RequresRedirect)
                {
                    Result = request.RedirectingResponse.AsActionResult()
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