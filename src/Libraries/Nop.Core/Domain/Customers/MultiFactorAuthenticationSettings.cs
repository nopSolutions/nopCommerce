using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Multi-factor authentication settings
    /// </summary>
    public partial class MultiFactorAuthenticationSettings : ISettings
    {
        #region Ctor

        public MultiFactorAuthenticationSettings()
        {
            ActiveAuthenticationMethodSystemNames = new List<string>();
        }

        #endregion

        /// <summary>
        /// Gets or sets system names of active multi-factor authentication methods
        /// </summary>
        public List<string> ActiveAuthenticationMethodSystemNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to force multi-factor authentication
        /// </summary>
        public bool ForceMultifactorAuthentication { get; set; }
    }
}
