using System;

namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// Represents an email account
    /// </summary>
    public partial class EmailAccount : BaseEntity
    {
        /// <summary>
        /// Gets or sets an email address
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets an email display name
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets an email host
        /// </summary>
        public virtual string Host { get; set; }

        /// <summary>
        /// Gets or sets an email port
        /// </summary>
        public virtual int Port { get; set; }

        /// <summary>
        /// Gets or sets an email user name
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// Gets or sets an email password
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Gets or sets a value that controls whether the SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection
        /// </summary>
        public virtual bool EnableSsl { get; set; }

        /// <summary>
        /// Gets or sets a value that controls whether the default system credentials of the application are sent with requests.
        /// </summary>
        public virtual bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// Gets a friendly email account name
        /// </summary>
        public virtual string FriendlyName
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(this.DisplayName))
                    return this.Email + " (" + this.DisplayName + ")";
                return this.Email;
            }
        }
    }
}
