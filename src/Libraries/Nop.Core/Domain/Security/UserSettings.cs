
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Security
{
    public class UserSettings : ISettings
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
        /// Gets or sets a customer password format (SHA1, MD5)
        /// </summary>
        public string HashedPasswordFormat { get; set; }
    }
}