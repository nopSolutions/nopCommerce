using System;
using System.Collections.Generic;
using System.Linq;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Messages.SMS
{
    /// <summary>
    /// Represents the SMS manager
    /// </summary>
    public partial interface ISMSManager
    {
        /// <summary>
        /// Deletes a SMS provider
        /// </summary>
        /// <param name="smsProviderId">SMS provider identifier</param>
        void DeleteSMSProvider(int smsProviderId);

        /// <summary>
        /// Gets a SMS provider
        /// </summary>
        /// <param name="smsProviderId">SMS provider identifier</param>
        /// <returns>SMS provider</returns>
        SMSProvider GetSMSProviderById(int smsProviderId);

        /// <summary>
        /// Gets a SMS provider
        /// </summary>
        /// <param name="systemKeyword">SMS provider system keyword</param>
        /// <returns>SMS provider</returns>
        SMSProvider GetSMSProviderBySystemKeyword(string systemKeyword);

        /// <summary>
        /// Gets all SMS providers
        /// </summary>
        /// <returns>SMS provider collection</returns>
        List<SMSProvider> GetAllSMSProviders();

        /// <summary>
        /// Gets all SMS providers
        /// </summary>
        /// <param name="showHidden">A value indicating whether the not active SMS providers should be load</param>
        /// <returns>SMS provider collection</returns>
        List<SMSProvider> GetAllSMSProviders(bool showHidden);

        /// <summary>
        /// Inserts a SMS provider
        /// </summary>
        /// <param name="smsProvider">SMS provider</param>
        void InsertSMSProvider(SMSProvider smsProvider);

        /// <summary>
        /// Updates the SMS provider
        /// </summary>
        /// <param name="smsProvider">SMS provider</param>
        void UpdateSMSProvider(SMSProvider smsProvider);

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Number of sent messages</returns>
        int SendSMS(string text);

        /// <summary>
        /// Sends SMS notification about placed order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>true if message was sent successfully; otherwise false.</returns>
        void SendOrderPlacedNotification(Order order);
    }
}
