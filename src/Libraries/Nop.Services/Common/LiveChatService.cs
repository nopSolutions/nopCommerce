using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Common;
using Nop.Core.Plugins;

namespace Nop.Services.Common
{
    /// <summary>
    /// Live chat service
    /// </summary>
    public partial class LiveChatService : ILiveChatService
    {
        #region Fields

        private readonly LiveChatSettings _liveChatSettings;
        private readonly IPluginFinder _pluginFinder;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="liveChatSettings">Live chat settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        public LiveChatService(LiveChatSettings liveChatSettings,
            IPluginFinder pluginFinder)
        {
            this._liveChatSettings = liveChatSettings;
            this._pluginFinder = pluginFinder;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Load active live chat providers
        /// </summary>
        /// <returns>Active live chat providers</returns>
        public virtual IList<ILiveChatProvider> LoadActiveLiveChatProviders()
        {
            return LoadAllLiveChatProviders()
                   .Where(provider => _liveChatSettings.ActiveLiveChatProviderSystemName.Contains(provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase))
                   .ToList();
        }

        /// <summary>
        /// Load live chat provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found live chat provider</returns>
        public virtual ILiveChatProvider LoadLiveChatProviderBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<ILiveChatProvider>(systemName);
            if (descriptor != null)
                return descriptor.Instance<ILiveChatProvider>();

            return null;
        }

        /// <summary>
        /// Load all live chat providers
        /// </summary>
        /// <returns>Live chat providers</returns>
        public virtual IList<ILiveChatProvider> LoadAllLiveChatProviders()
        {
            return _pluginFinder.GetPlugins<ILiveChatProvider>().ToList();
        }
        
        #endregion
    }
}
