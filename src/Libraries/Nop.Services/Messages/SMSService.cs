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
    public partial class SmsService:ISmsService
    {
        private readonly IPluginFinder _pluginFinder;
        private readonly SmsSettings _smsSettings;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="smsSettings">SmsSettings instance</param>
        public SmsService(IPluginFinder pluginFinder, SmsSettings smsSettings)
        {
            this._pluginFinder = pluginFinder;
            this._smsSettings = smsSettings;
        }

        /// <summary>
        /// Load all SMS providers
        /// </summary>
        /// <returns>SMS provider list</returns>
        public virtual IList<ISmsProvider> LoadAllSmsProviders()
        {
            //TODO Do not return plugin instances, return a list of PluginDescriptor
            var smsProviders = _pluginFinder.GetPlugins<ISmsProvider>();
            return smsProviders.ToList();
        }

        /// <summary>
        /// Load SMS provider by system name
        /// </summary>
        /// <param name="systemName">SMS provider system name</param>
        /// <returns>SMS provider</returns>
        public virtual ISmsProvider LoadSmsProviderBySystemName(string systemName)
        {
            var providers = LoadAllSmsProviders();
            var provider = providers.SingleOrDefault(p => p.PluginDescriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
            return provider;
        }

        /// <summary>
        /// Load active SMS providers
        /// </summary>
        /// <returns>Active SMS provider list</returns>
        public virtual IList<ISmsProvider> LoadActiveSmsProviders()
        {
            return LoadAllSmsProviders()
                .Where(provider => _smsSettings.ActiveSmsProviderSystemNames.Contains(provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Number of sent messages</returns>
        public virtual int SendSms(string text)
        {
            int i = 0;

            foreach (var smsProvider in LoadActiveSmsProviders())
            {
                if (smsProvider.SendSms(text))
                {
                    i++;
                }
            }
            return i;
        } 
    }
}
