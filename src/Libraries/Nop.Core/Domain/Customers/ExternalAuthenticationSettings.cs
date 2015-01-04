using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// External authentication settings
    /// </summary>
    public class ExternalAuthenticationSettings : ISettings
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalAuthenticationSettings()
        {
            ActiveAuthenticationMethodSystemNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto registration is enabled
        /// </summary>
        public bool AutoRegisterEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether email validation is required with "AutoRegisterEnabled".
        /// In most cases we can skip email validation for Facebook or any other third-party external authentication plugins. I guess we can trust  Facebook for the validation.
        /// </summary>
        public bool RequireEmailValidation { get; set; }

        /// <summary>
        /// Gets or sets system names of active payment methods
        /// </summary>
        public List<string> ActiveAuthenticationMethodSystemNames { get; set; }
    }
}