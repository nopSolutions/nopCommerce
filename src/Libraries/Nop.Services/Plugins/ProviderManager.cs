using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;

namespace Nop.Services.Plugins
{
    public partial class ProviderManager<TPlugin> : IProviderManager<TPlugin> where TPlugin : class, IProvider
    {
        #region Fields

        private readonly List<PluginDescriptor> _descriptors;
        private readonly Dictionary<string, TPlugin> _providersBySysName;
        private readonly Dictionary<Guid, List<TPlugin>> _allProviders;

        #endregion

        #region ctor

        public ProviderManager(IPluginService pluginService)
        {
            _providersBySysName = new Dictionary<string, TPlugin>();
            _allProviders = new Dictionary<Guid, List<TPlugin>>();
            _descriptors = pluginService.GetPluginDescriptors<TPlugin>().ToList();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether to load the plugin based on the customer passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="customer">Customer</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByCustomer(PluginDescriptor pluginDescriptor, Customer customer)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (customer == null || !pluginDescriptor.LimitedToCustomerRoles.Any())
                return true;

            var customerRoleIds = customer.CustomerRoles.Where(role => role.Active).Select(role => role.Id);

            return pluginDescriptor.LimitedToCustomerRoles.Intersect(customerRoleIds).Any();
        }

        /// <summary>
        /// Check whether to load the plugin based on the store identifier passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByStore(PluginDescriptor pluginDescriptor, int storeId)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            //no validation required
            if (storeId == 0)
                return true;

            if (!pluginDescriptor.LimitedToStores.Any())
                return true;

            return pluginDescriptor.LimitedToStores.Contains(storeId);
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Returns all providers
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>List of providers</returns>
        public virtual IList<TPlugin> LoadAllProviders(Customer customer = null, int storeId = 0)
        {
            //get customer GUID. If customer is null, then use default (all zeros) GUID value
            var customerGuid = customer?.CustomerGuid ?? default(Guid);

            if (_allProviders.ContainsKey(customerGuid))
                return _allProviders[customerGuid];

            var pluginDescriptors = _descriptors.Where(descriptor =>
                FilterByCustomer(descriptor, customer) &&
                FilterByStore(descriptor, storeId));
            _allProviders.Add(customerGuid, pluginDescriptors.Select(descriptor => descriptor.Instance<TPlugin>()).ToList());

            return _allProviders[customerGuid];
        }

        /// <summary>
        /// Returns provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <returns>Provider</returns>
        public virtual TPlugin LoadProviderBySystemName(string systemName, Customer customer = null)
        {
            if (systemName == null)
                return null;

            if (_providersBySysName.ContainsKey(systemName)) 
                return _providersBySysName[systemName];

            var pluginDescriptors = _descriptors.Where(descriptor =>
                FilterByCustomer(descriptor, customer));
            var pluginDescriptor = pluginDescriptors.FirstOrDefault(descriptor => descriptor.SystemName.Equals(
                systemName,
                StringComparison.InvariantCultureIgnoreCase));

            _providersBySysName.Add(systemName, pluginDescriptor?.Instance<TPlugin>());

            return pluginDescriptor?.Instance<TPlugin>();
        }

        /// <summary>
        /// Returns active provider
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Active provider</returns>
        public virtual TPlugin LoadActiveProvider(string systemName, Customer customer = null, int storeId = 0)
        {
            var plugin = LoadProviderBySystemName(systemName, customer) ??
                         LoadAllProviders(customer, storeId).FirstOrDefault();

            return plugin;
        }

        /// <summary>
        /// Returns active providers
        /// </summary>
        /// <param name="systemNames">System names</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>List of active providers</returns>
        public virtual IList<TPlugin> LoadActiveProviders(List<string> systemNames, Customer customer = null, int storeId = 0)
        {
            return LoadAllProviders(customer, storeId)
                .Where(provider => systemNames.Contains(provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        #endregion
    }
}
