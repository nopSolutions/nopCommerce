namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents an external authentication record
    /// </summary>
    public partial class ExternalAuthenticationRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public virtual int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the external email
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the external identifier
        /// </summary>
        public virtual string ExternalIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the external display identifier
        /// </summary>
        public virtual string ExternalDisplayIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the OAuthToken
        /// </summary>
        public virtual string OAuthToken { get; set; }

        /// <summary>
        /// Gets or sets the OAuthAccessToken
        /// </summary>
        public virtual string OAuthAccessToken { get; set; }

        /// <summary>
        /// Gets or sets the provider
        /// </summary>
        public virtual string ProviderSystemName { get; set; }
        
        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }
    }

}
