using System.Collections.Generic;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Represents the SMS service
    /// </summary>
    public partial interface ISMSService
    {
        /// <summary>
        /// Load all SMS providers
        /// </summary>
        /// <returns>SMS provider list</returns>
        IList<ISMSProvider> LoadAllSMSProviders();

         /// <summary>
        /// Load SMS provider by system name
        /// </summary>
        /// <param name="systemName">SMS provider system name</param>
        /// <returns>SMS provider</returns>
        ISMSProvider LoadSMSProviderBySystemName(string systemName);

        /// <summary>
        /// Load active SMS providers
        /// </summary>
        /// <returns>Active SMS provider list</returns>
        IList<ISMSProvider> LoadActiveSMSProviders();

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Number of sent messages</returns>
        int SendSMS(string text); 
    }
}
