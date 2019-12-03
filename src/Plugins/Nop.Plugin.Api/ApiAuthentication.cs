namespace Nop.Plugin.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using IdentityModel;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Nop.Plugin.Api.Helpers;
    using Nop.Services.Authentication.External;
    using Org.BouncyCastle.Asn1.X509.Qualified;

    public class ApiAuthentication : IExternalAuthenticationRegistrar
    {
        public void Configure(AuthenticationBuilder builder)
        {
            RsaSecurityKey signingKey = CryptoHelper.CreateRsaSecurityKey();

            builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwt =>
                {
                    jwt.Audience = "nop_api";
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateActor = false,
                        ValidateIssuer = false,
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role,
                        // Uncomment this if you are using an certificate to sign your tokens.
                        // IssuerSigningKey = new X509SecurityKey(cert),
                        IssuerSigningKeyResolver = (string token, SecurityToken securityToken, string kid,
                                TokenValidationParameters validationParameters) =>
                               new List<RsaSecurityKey> { signingKey }
                    };
                });
        }
    }
}