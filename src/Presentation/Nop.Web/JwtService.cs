using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Collections.Generic;

namespace Nop.Web
{
    public class JwtService
    {
        private readonly string _issuer;
        private readonly string _secret;
        private readonly string _expDate;

        public JwtService(IConfiguration config)
        {
            _issuer = config.GetSection("Jwt").GetSection("Issuer").Value;
            _secret = config.GetSection("Jwt").GetSection("Key").Value;
            _expDate = config.GetSection("JwtConfig").GetSection("expirationInMinutes").Value;
        }

        public string GenerateSecurityToken(string email, int customerId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>();
            permClaims.Add(new Claim("Email", email));
            permClaims.Add(new Claim("UserId", customerId.ToString()));

            var token = new JwtSecurityToken(_issuer,
              _issuer,
              claims: permClaims,
              null,
              expires: DateTime.Now.AddDays(7),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
