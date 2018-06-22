using System;
using System.Collections.Generic;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// External authentication parameters
    /// </summary>
    [Serializable]
    public partial class ExternalAuthenticationParameters
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ExternalAuthenticationParameters()
        {
            Claims = new List<ExternalAuthenticationClaim>();
        }

        /// <summary>
        /// Gets or sets the system name of external authentication method
        /// </summary>
        public string ProviderSystemName { get; set; }

        /// <summary>
        /// Gets or sets user external identifier 
        /// </summary>
        public string ExternalIdentifier { get; set; }

        /// <summary>
        /// Gets or sets user external display identifier 
        /// </summary>
        public string ExternalDisplayIdentifier { get; set; }

        /// <summary>
        /// Gets or sets access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets user email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the additional user info as a list of a custom claims
        /// </summary>
        public IList<ExternalAuthenticationClaim> Claims { get; set; }
    }

    /// <summary>
    /// External authentication claim
    /// </summary>
    [Serializable]
    public class ExternalAuthenticationClaim
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="value">Value</param>
        public ExternalAuthenticationClaim(string type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the claim type of the claim
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the claim
        /// </summary>
        public string Value { get; set; }
    }
}