using System;

namespace Nop.Services.Authentication.External
{
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
            Type = type;
            Value = value;
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