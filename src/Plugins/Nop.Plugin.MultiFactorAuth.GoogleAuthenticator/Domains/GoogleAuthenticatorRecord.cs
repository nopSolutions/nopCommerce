using Nop.Core;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Domains
{
    /// <summary>
    /// Represents a  Google Authenticator configuration
    /// </summary>
    public class GoogleAuthenticatorRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets a customer identifier
        /// </summary>
        public string Customer { get; set; }

        /// <summary>
        /// Gets or sets a SecretKey identifier
        /// </summary>
        public string SecretKey { get; set; }
    }
}
