using System;
using System.Collections.Generic;
using System.Linq;

using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Represents the SMS service
    /// </summary>
    public partial class SMSService:ISMSService
    {
        private readonly IPluginFinder _pluginFinder;
        private readonly SMSSettings _smsSettings;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="smsSettings">SMSSettings instance</param>
        public SMSService(IPluginFinder pluginFinder, SMSSettings smsSettings)
        {
            this._pluginFinder = pluginFinder;
            this._smsSettings = smsSettings;
        }

        /// <summary>
        /// Load all SMS providers
        /// </summary>
        /// <returns>SMS provider list</returns>
        public IList<ISMSProvider> LoadAllSMSProviders()
        {
            var smsProviders = _pluginFinder.GetPlugins<ISMSProvider>();
            return smsProviders.OrderBy(tp => tp.FriendlyName).ToList();
        }

        /// <summary>
        /// Load SMS provider by system name
        /// </summary>
        /// <param name="systemName">SMS provider system name</param>
        /// <returns>SMS provider</returns>
        public ISMSProvider LoadSMSProviderBySystemName(string systemName)
        {
            var providers = LoadAllSMSProviders();
            var provider = providers.SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
            return provider;
        }

        /// <summary>
        /// Load active SMS providers
        /// </summary>
        /// <returns>Active SMS provider list</returns>
        public IList<ISMSProvider> LoadActiveSMSProviders()
        {
            return LoadAllSMSProviders()
                .Where(smsProvider => _smsSettings.ActiveSMSProviderSystemNames.Contains(smsProvider.SystemName, StringComparer.InvariantCultureIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Number of sent messages</returns>
        public int SendSMS(string text)
        {
            int i = 0;

            foreach (var smsProvider in LoadActiveSMSProviders())
            {
                if (smsProvider.SendSMS(text))
                {
                    i++;
                }
            }
            return i;
        } 
    }
}
