using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Multi-factor authentication settings
    /// </summary>
    public class MultiFactorAuthenticationSettings : ISettings
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
    }
}
