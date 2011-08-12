//Contributor:  Nicholas Mayne


using System.Collections.Generic;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.OpenId.Core
{
    public sealed class OpenIdAuthenticationParameters : OpenAuthenticationParameters
    {
        private readonly IAuthenticationResponse _authenticationResponse;
        private readonly IList<UserClaims> _claims;

        public OpenIdAuthenticationParameters() { }

        public OpenIdAuthenticationParameters(IAuthenticationResponse authenticationResponse)
        {
            _authenticationResponse = authenticationResponse;

            ExternalIdentifier = _authenticationResponse.ClaimedIdentifier;
            ExternalDisplayIdentifier = _authenticationResponse.FriendlyIdentifierForDisplay;

            _claims = new List<UserClaims>();
            var claimsResponseTranslator = new OpenIdClaimsResponseClaimsTranslator();
            var claims1 = claimsResponseTranslator.Translate(_authenticationResponse.GetExtension<ClaimsResponse>());
            if (claims1 != null)
                UserClaims.Add(claims1);

            var fetchResponseTranslator = new OpenIdFetchResponseClaimsTranslator();
            var claims2 = fetchResponseTranslator.Translate(_authenticationResponse.GetExtension<FetchResponse>());
            if (claims2 != null)
                UserClaims.Add(claims2);
        }

        public override IList<UserClaims> UserClaims
        {
            get
            {
                return _claims;
            }
        }

        public override string ProviderSystemName
        {
            get { return Provider.SystemName; }
        }
    }
}