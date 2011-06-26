using System.Collections.Generic;

namespace Nop.Services.Common
{
    /// <summary>
    /// Live chat service interface
    /// </summary>
    public partial interface ILiveChatService
    {
        /// <summary>
        /// Load active live chat providers
        /// </summary>
        /// <returns>Active live chat providers</returns>
        IList<ILiveChatProvider> LoadActiveLiveChatProviders();

        /// <summary>
        /// Load live chat provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found live chat provider</returns>
        ILiveChatProvider LoadLiveChatProviderBySystemName(string systemName);

        /// <summary>
        /// Load all live chat providers
        /// </summary>
        /// <returns>Live chat providers</returns>
        IList<ILiveChatProvider> LoadAllLiveChatProviders();
    }
}
