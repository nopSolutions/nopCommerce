//Contributor:  Nicholas Mayne


using System;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.OpenId.Core
{
    public class OpenIdFetchResponseClaimsTranslator : IClaimsTranslator<FetchResponse>
    {
        public UserClaims Translate(FetchResponse response)
        {
            if (response == null)
                return null;

            var claims = new UserClaims();
            claims.BirthDate = new BirthDateClaims();
            int dayOfMonth;
            if (int.TryParse(response.GetAttributeValue(WellKnownAttributes.BirthDate.DayOfMonth), out dayOfMonth))
                claims.BirthDate.DayOfMonth = dayOfMonth;
            int month;
            if (int.TryParse(response.GetAttributeValue(WellKnownAttributes.BirthDate.Month), out month))
                claims.BirthDate.Month = month;
            DateTime wholeBirthDate;
            if (DateTime.TryParse(response.GetAttributeValue(WellKnownAttributes.BirthDate.WholeBirthDate), out wholeBirthDate))
                claims.BirthDate.WholeBirthDate = wholeBirthDate;
            int year;
            if (int.TryParse(response.GetAttributeValue(WellKnownAttributes.BirthDate.Year), out year))
                claims.BirthDate.Year = year;

            claims.Company = new CompanyClaims();
            claims.Company.CompanyName = response.GetAttributeValue(WellKnownAttributes.Company.CompanyName);
            claims.Company.JobTitle = response.GetAttributeValue(WellKnownAttributes.Company.JobTitle);

            claims.Contact = new ContactClaims();
            claims.Contact.Email = response.GetAttributeValue(WellKnownAttributes.Contact.Email);

            claims.Contact.Address = new AddressClaims();
            claims.Contact.Address.City = response.GetAttributeValue(WellKnownAttributes.Contact.HomeAddress.City);
            claims.Contact.Address.Country = response.GetAttributeValue(WellKnownAttributes.Contact.HomeAddress.Country);
            claims.Contact.Address.PostalCode = response.GetAttributeValue(WellKnownAttributes.Contact.HomeAddress.PostalCode);
            claims.Contact.Address.State = response.GetAttributeValue(WellKnownAttributes.Contact.HomeAddress.State);
            claims.Contact.Address.StreetAddressLine1 = response.GetAttributeValue(WellKnownAttributes.Contact.HomeAddress.StreetAddressLine1);
            claims.Contact.Address.StreetAddressLine2 = response.GetAttributeValue(WellKnownAttributes.Contact.HomeAddress.StreetAddressLine2);

            claims.Contact.IM = new InstantMessagingClaims();
            claims.Contact.IM.AOL = response.GetAttributeValue(WellKnownAttributes.Contact.IM.AOL);
            claims.Contact.IM.ICQ = response.GetAttributeValue(WellKnownAttributes.Contact.IM.ICQ);
            claims.Contact.IM.Jabber = response.GetAttributeValue(WellKnownAttributes.Contact.IM.Jabber);
            claims.Contact.IM.MSN = response.GetAttributeValue(WellKnownAttributes.Contact.IM.MSN);
            claims.Contact.IM.Skype = response.GetAttributeValue(WellKnownAttributes.Contact.IM.Skype);
            claims.Contact.IM.Yahoo = response.GetAttributeValue(WellKnownAttributes.Contact.IM.Yahoo);

            claims.Contact.Phone = new TelephoneClaims();
            claims.Contact.Phone.Fax = response.GetAttributeValue(WellKnownAttributes.Contact.Phone.Fax);
            claims.Contact.Phone.Home = response.GetAttributeValue(WellKnownAttributes.Contact.Phone.Home);
            claims.Contact.Phone.Mobile = response.GetAttributeValue(WellKnownAttributes.Contact.Phone.Mobile);
            claims.Contact.Phone.Preferred = response.GetAttributeValue(WellKnownAttributes.Contact.Phone.Preferred);
            claims.Contact.Phone.Work = response.GetAttributeValue(WellKnownAttributes.Contact.Phone.Work);

            claims.Contact.Web = new WebClaims();
            claims.Contact.Web.Amazon = response.GetAttributeValue(WellKnownAttributes.Contact.Web.Amazon);
            claims.Contact.Web.Blog = response.GetAttributeValue(WellKnownAttributes.Contact.Web.Blog);
            claims.Contact.Web.Delicious = response.GetAttributeValue(WellKnownAttributes.Contact.Web.Delicious);
            claims.Contact.Web.Flickr = response.GetAttributeValue(WellKnownAttributes.Contact.Web.Flickr);
            claims.Contact.Web.Homepage = response.GetAttributeValue(WellKnownAttributes.Contact.Web.Homepage);
            claims.Contact.Web.LinkedIn = response.GetAttributeValue(WellKnownAttributes.Contact.Web.LinkedIn);

            claims.Contact.WorkAddress = new AddressClaims();
            claims.Contact.WorkAddress.City = response.GetAttributeValue(WellKnownAttributes.Contact.WorkAddress.City);
            claims.Contact.WorkAddress.Country = response.GetAttributeValue(WellKnownAttributes.Contact.WorkAddress.Country);
            claims.Contact.WorkAddress.PostalCode = response.GetAttributeValue(WellKnownAttributes.Contact.WorkAddress.PostalCode);
            claims.Contact.WorkAddress.State = response.GetAttributeValue(WellKnownAttributes.Contact.WorkAddress.State);
            claims.Contact.WorkAddress.StreetAddressLine1 = response.GetAttributeValue(WellKnownAttributes.Contact.WorkAddress.StreetAddressLine1);
            claims.Contact.WorkAddress.StreetAddressLine2 = response.GetAttributeValue(WellKnownAttributes.Contact.WorkAddress.StreetAddressLine2);

            claims.Media = new MediaClaims();
            claims.Media.AudioGreeting = response.GetAttributeValue(WellKnownAttributes.Media.AudioGreeting);
            claims.Media.SpokenName = response.GetAttributeValue(WellKnownAttributes.Media.SpokenName);
            claims.Media.VideoGreeting = response.GetAttributeValue(WellKnownAttributes.Media.VideoGreeting);

            claims.Media.Images = new ImageClaims();
            claims.Media.Images.Aspect11 = response.GetAttributeValue(WellKnownAttributes.Media.Images.Aspect11);
            claims.Media.Images.Aspect34 = response.GetAttributeValue(WellKnownAttributes.Media.Images.Aspect34);
            claims.Media.Images.Aspect43 = response.GetAttributeValue(WellKnownAttributes.Media.Images.Aspect43);
            claims.Media.Images.Default = response.GetAttributeValue(WellKnownAttributes.Media.Images.Default);
            claims.Media.Images.FavIcon = response.GetAttributeValue(WellKnownAttributes.Media.Images.FavIcon);

            claims.Name = new NameClaims();
            claims.Name.Alias = response.GetAttributeValue(WellKnownAttributes.Name.Alias);
            claims.Name.First = response.GetAttributeValue(WellKnownAttributes.Name.First);
            claims.Name.FullName = response.GetAttributeValue(WellKnownAttributes.Name.FullName);
            claims.Name.Last = response.GetAttributeValue(WellKnownAttributes.Name.Last);
            claims.Name.Middle = response.GetAttributeValue(WellKnownAttributes.Name.Middle);
            claims.Name.Prefix = response.GetAttributeValue(WellKnownAttributes.Name.Prefix);
            claims.Name.Suffix = response.GetAttributeValue(WellKnownAttributes.Name.Suffix);

            claims.Person = new PersonClaims();
            claims.Person.Biography = response.GetAttributeValue(WellKnownAttributes.Person.Biography);
            claims.Person.Gender = response.GetAttributeValue(WellKnownAttributes.Person.Gender);

            claims.Preferences = new PreferenceClaims();
            claims.Preferences.Language = response.GetAttributeValue(WellKnownAttributes.Preferences.Language);
            claims.Preferences.TimeZone = response.GetAttributeValue(WellKnownAttributes.Preferences.TimeZone);

            return claims;
        }
    }
}