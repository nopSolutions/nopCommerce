using System;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// Extensions of external authentication method 
    /// </summary>
    public static class ExternalAuthenticationMethodExtensions
    {
        /// <summary>
        /// Check whether external authentication method is active
        /// </summary>
        /// <param name="method">External authentication method</param>
        /// <param name="settings">External authentication settings</param>
        /// <returns>True if method is active; otherwise false</returns>
        public static bool IsMethodActive(this IExternalAuthenticationMethod method, ExternalAuthenticationSettings settings)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.ActiveAuthenticationMethodSystemNames == null)
                return false;

            foreach (var activeMethodSystemName in settings.ActiveAuthenticationMethodSystemNames)
                if (method.PluginDescriptor.SystemName.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }
    }
}
