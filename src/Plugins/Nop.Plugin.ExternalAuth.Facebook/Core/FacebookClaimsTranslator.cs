//Contributor:  Nicholas Mayne

using System.Collections.Generic;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Facebook.Core
{
    public class FacebookClaimsTranslator : IClaimsTranslator<IDictionary<string, object>>
    {
        public UserClaims Translate(IDictionary<string, object> response)
        {
            UserClaims claims = new UserClaims();

            claims.Contact = new ContactClaims();
            if (response.ContainsKey("email"))
                claims.Contact.Email = response["email"].ToString();

            return claims;
        }
    }
}