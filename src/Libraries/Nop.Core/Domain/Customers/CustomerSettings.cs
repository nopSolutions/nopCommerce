
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
        /// Gets or sets a value indicating whether users are allowed to change their usernames
        /// </summary>
        public bool AllowUsersToChangeUsernames { get; set; }

        /// <summary>
        /// Gets or sets a customer password format (SHA1, MD5) when passwords are hashed
        /// </summary>
        public string HashedPasswordFormat { get; set; }

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
        /// Gets or sets a value indicating whether to validate user when downloading products
        /// </summary>
        public bool DownloadableProductsValidateUser { get; set; }

        /// <summary>
        /// Customer name formatting
        /// </summary>
        public CustomerNameFormat CustomerNameFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Gender' is enabled
        /// </summary>
        public bool GenderEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Date of Birth' is enabled
        /// </summary>
        public bool DateOfBirthEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Company' is enabled
        /// </summary>
        public bool CompanyEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Newsletter' form field is enabled
        /// </summary>
        public bool NewsletterEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide newsletter box
        /// </summary>
        public bool HideNewsletterBlock { get; set; }
    }
}