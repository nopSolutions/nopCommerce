//Contributor:  Nicholas Mayne


using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.OpenId.Core
{
    public class OpenIdClaimsResponseClaimsTranslator : IClaimsTranslator<ClaimsResponse>
    {
        public UserClaims Translate(ClaimsResponse response)
        {
            if (response == null)
                return null;

            var claims = new UserClaims();

            claims.BirthDate = new BirthDateClaims();
            if (response.BirthDate.HasValue)
            {
                claims.BirthDate.DayOfMonth = response.BirthDate.Value.Day;
                claims.BirthDate.Month = response.BirthDate.Value.Month;
                claims.BirthDate.WholeBirthDate = response.BirthDate;
                claims.BirthDate.Year = response.BirthDate.Value.Year;
            }
            claims.BirthDate.Raw = response.BirthDateRaw;

            claims.Contact = new ContactClaims();
            claims.Contact.Email = response.Email;

            claims.Contact.Address = new AddressClaims();
            claims.Contact.Address.Country = response.Country;
            claims.Contact.Address.PostalCode = response.PostalCode;

            claims.Preferences = new PreferenceClaims();
            if (response.Culture != null)
                claims.Preferences.Language = response.Culture.IetfLanguageTag;

            claims.Preferences.PrimaryLanguage = response.Language;
            claims.Preferences.TimeZone = response.TimeZone;

            claims.Name = new NameClaims();
            claims.Name.FullName = response.FullName;
            claims.Name.Nickname = response.Nickname;

            claims.Person = new PersonClaims();
            if (response.Gender.HasValue)
                claims.Person.Gender = response.Gender.Value.ToString();

            claims.IsSignedByProvider = response.IsSignedByProvider;

            claims.Contact.MailAddress = new AddressClaims();
            if (response.MailAddress != null)
            {
                claims.Contact.MailAddress.SingleLineAddress = response.MailAddress.Address;
                claims.Contact.MailAddress.DisplayName = response.MailAddress.DisplayName;
                claims.Contact.MailAddress.Host = response.MailAddress.Host;
                claims.Contact.MailAddress.User = response.MailAddress.User;
            }

            claims.Version = response.Version;
            return claims;
        }
    }
}