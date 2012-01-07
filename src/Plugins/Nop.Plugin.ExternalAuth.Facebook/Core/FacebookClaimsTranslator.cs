//Contributor:  Nicholas Mayne

using System.Collections.Generic;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Facebook.Core
{
    public class FacebookClaimsTranslator : IClaimsTranslator<IDictionary<string, object>>
    {
        public UserClaims Translate(IDictionary<string, object> response)
        {
            var claims = new UserClaims();

            claims.Contact = new ContactClaims();
            if (response.ContainsKey("email"))
                claims.Contact.Email = response["email"].ToString();

            claims.Name = new NameClaims();
            if (response.ContainsKey("first_name"))
                claims.Name.First = response["first_name"].ToString();
            if (response.ContainsKey("last_name"))
                claims.Name.Last = response["last_name"].ToString();

            return claims;
        }
    }
}