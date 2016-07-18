
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers
{
    public class CustomerSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether usernames are used instead of emails
        /// </summary>
        public bool UsernamesEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users can check the availability of usernames (when registering or changing in 'My Account')
        /// </summary>
        public bool CheckUsernameAvailabilityEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to change their usernames
        /// </summary>
        public bool AllowUsersToChangeUsernames { get; set; }

        /// <summary>
        /// Default password format for customers
        /// </summary>
        public PasswordFormat DefaultPasswordFormat { get; set; }

        /// <summary>
        /// Gets or sets a customer password format (SHA1, MD5) when passwords are hashed
        /// </summary>
        public string HashedPasswordFormat { get; set; }

        /// <summary>
        /// Gets or sets a minimum password length
        /// </summary>
        public int PasswordMinLength { get; set; }

        /// <summary>
        /// Gets or sets a number of days for password recovery link. Set to 0 if it doesn't expire.
        /// </summary>
        public int PasswordRecoveryLinkDaysValid { get; set; }

        /// <summary>
        /// User registration type
        /// </summary>
        public UserRegistrationType UserRegistrationType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to upload avatars.
        /// </summary>
        public bool AllowCustomersToUploadAvatars { get; set; }

        /// <summary>
        /// Gets or sets a maximum avatar size (in bytes)
        /// </summary>
        public int AvatarMaximumSizeBytes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display default user avatar.
        /// </summary>
        public bool DefaultAvatarEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers location is shown
        /// </summary>
        public bool ShowCustomersLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show customers join date
        /// </summary>
        public bool ShowCustomersJoinDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to view profiles of other customers
        /// </summary>
        public bool AllowViewingProfiles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'New customer' notification message should be sent to a store owner
        /// </summary>
        public bool NotifyNewCustomerRegistration { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to hide 'Downloable products' tab on 'My account' page
        /// </summary>
        public bool HideDownloadableProductsTab { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide 'Back in stock subscriptions' tab on 'My account' page
        /// </summary>
        public bool HideBackInStockSubscriptionsTab { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate user when downloading products
        /// </summary>
        public bool DownloadableProductsValidateUser { get; set; }

        /// <summary>
        /// Customer name formatting
        /// </summary>
        public CustomerNameFormat CustomerNameFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Newsletter' form field is enabled
        /// </summary>
        public bool NewsletterEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Newsletter' checkbox is ticked by default on the registration page
        /// </summary>
        public bool NewsletterTickedByDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide newsletter box
        /// </summary>
        public bool HideNewsletterBlock { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether newsletter block should allow to unsubsribe
        /// </summary>
        public bool NewsletterBlockAllowToUnsubscribe { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the number of minutes for 'online customers' module
        /// </summary>
        public int OnlineCustomerMinutes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating we should store last visited page URL for each customer
        /// </summary>
        public bool StoreLastVisitedPage { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether deleted customer records should be prefixed suffixed with "-DELETED"
        /// </summary>
        public bool SuffixDeletedCustomers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to force entering email twice
        /// </summary>
        public bool EnteringEmailTwice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether registration is required for downloadable products
        /// </summary>
        public bool RequireRegistrationForDownloadableProducts { get; set; }

        /// <summary>
        /// Gets or sets interval (in minutes) with which the Delete Guest Task runs
        /// </summary>
        public int DeleteGuestTaskOlderThanMinutes { get; set; }

        #region Form fields

        /// <summary>
        /// Gets or sets a value indicating whether 'Gender' is enabled
        /// </summary>
        public bool GenderEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Date of Birth' is enabled
        /// </summary>
        public bool DateOfBirthEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'Date of Birth' is required
        /// </summary>
        public bool DateOfBirthRequired { get; set; }
        /// <summary>
        /// Gets or sets a minimum age. Null if ignored
        /// </summary>
        public int? DateOfBirthMinimumAge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Company' is enabled
        /// </summary>
        public bool CompanyEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'Company' is required
        /// </summary>
        public bool CompanyRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Street address' is enabled
        /// </summary>
        public bool StreetAddressEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'Street address' is required
        /// </summary>
        public bool StreetAddressRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Street address 2' is enabled
        /// </summary>
        public bool StreetAddress2Enabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'Street address 2' is required
        /// </summary>
        public bool StreetAddress2Required { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Zip / postal code' is enabled
        /// </summary>
        public bool ZipPostalCodeEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'Zip / postal code' is required
        /// </summary>
        public bool ZipPostalCodeRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'City' is enabled
        /// </summary>
        public bool CityEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'City' is required
        /// </summary>
        public bool CityRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Country' is enabled
        /// </summary>
        public bool CountryEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'Country' is required
        /// </summary>
        public bool CountryRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'State / province' is enabled
        /// </summary>
        public bool StateProvinceEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'State / province' is required
        /// </summary>
        public bool StateProvinceRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Phone number' is enabled
        /// </summary>
        public bool PhoneEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'Phone number' is required
        /// </summary>
        public bool PhoneRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Fax number' is enabled
        /// </summary>
        public bool FaxEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether 'Fax number' is required
        /// </summary>
        public bool FaxRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether privacy policy should accepted during registration
        /// </summary>
        public bool AcceptPrivacyPolicyEnabled { get; set; }

        #endregion
    }
}