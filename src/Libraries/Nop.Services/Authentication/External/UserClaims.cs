//Contributor:  Nicholas Mayne

using System;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// User claims
    /// </summary>
    [Serializable]
    public partial class UserClaims
    {
        public BirthDateClaims BirthDate { get; set; }
        public ContactClaims Contact { get; set; }
        public PreferenceClaims Preferences { get; set; }
        public NameClaims Name { get; set; }
        public PersonClaims Person { get; set; }
        public bool IsSignedByProvider { get; set; }
        public Version Version { get; set; }
        public CompanyClaims Company { get; set; }
        public MediaClaims Media { get; set; }
    }

    /// <summary>
    /// Image claims
    /// </summary>
    [Serializable]
    public partial class ImageClaims
    {
        public string Aspect11 { get; set; }
        public string Aspect34 { get; set; }
        public string Aspect43 { get; set; }
        public string Default { get; set; }
        public string FavIcon { get; set; }
    }

    /// <summary>
    /// Media claims
    /// </summary>
    [Serializable]
    public partial class MediaClaims
    {
        public string AudioGreeting { get; set; }
        public string SpokenName { get; set; }
        public string VideoGreeting { get; set; }

        public ImageClaims Images { get; set; }
    }

    /// <summary>
    /// Web claims
    /// </summary>
    [Serializable]
    public partial class WebClaims
    {
        public string Amazon { get; set; }
        public string Blog { get; set; }
        public string Delicious { get; set; }
        public string Flickr { get; set; }
        public string Homepage { get; set; }
        public string LinkedIn { get; set; }
    }

    /// <summary>
    /// Telephone claims
    /// </summary>
    [Serializable]
    public partial class TelephoneClaims
    {
        public string Fax { get; set; }
        public string Home { get; set; }
        public string Mobile { get; set; }
        public string Preferred { get; set; }
        public string Work { get; set; }
    }

    /// <summary>
    /// Instant messaging claims
    /// </summary>
    [Serializable]
    public partial class InstantMessagingClaims
    {
        public string AOL { get; set; }
        public string ICQ { get; set; }
        public string Jabber { get; set; }
        public string MSN { get; set; }
        public string Skype { get; set; }
        public string Yahoo { get; set; }
    }

    /// <summary>
    /// Company claims
    /// </summary>
    [Serializable]
    public partial class CompanyClaims
    {
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
    }

    /// <summary>
    /// Address claims
    /// </summary>
    [Serializable]
    public partial class AddressClaims
    {
        public string SingleLineAddress { get; set; }
        public string DisplayName { get; set; }
        public string Host { get; set; }
        public string User { get; set; }

        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string StreetAddressLine1 { get; set; }
        public string StreetAddressLine2 { get; set; }
    }

    /// <summary>
    /// Person claims
    /// </summary>
    [Serializable]
    public partial class PersonClaims
    {
        public string Gender { get; set; }
        public string Biography { get; set; }
    }

    /// <summary>
    /// Name claims
    /// </summary>
    [Serializable]
    public partial class NameClaims
    {
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string Alias { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string Middle { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
    }

    /// <summary>
    /// Preference claims
    /// </summary>
    [Serializable]
    public partial class PreferenceClaims
    {
        public string Language { get; set; }
        public string PrimaryLanguage { get; set; }
        public string TimeZone { get; set; }
    }

    /// <summary>
    /// Contact claims
    /// </summary>
    [Serializable]
    public partial class ContactClaims
    {
        public string Email { get; set; }

        public AddressClaims Address { get; set; }
        public InstantMessagingClaims IM { get; set; }
        public TelephoneClaims Phone { get; set; }
        public WebClaims Web { get; set; }
        public AddressClaims MailAddress { get; set; }
        public AddressClaims WorkAddress { get; set; }
    }

    /// <summary>
    /// Birth date claims
    /// </summary>
    [Serializable]
    public partial class BirthDateClaims
    {
        public int DayOfMonth { get; set; }
        public int Month { get; set; }
        public DateTime? WholeBirthDate { get; set; }
        public int Year { get; set; }
        public string Raw { get; set; }
        public DateTime GeneratedBirthDate { get { return new DateTime(Year, Month, DayOfMonth); } }
    }
}