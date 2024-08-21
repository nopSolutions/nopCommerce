﻿using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Plugins;

 namespace Nop.Services.Catalog; 

 /// <summary>
 /// Represents a search plugin manager implementation
 /// </summary>
 public partial class SearchPluginManager : PluginManager<ISearchProvider>, ISearchPluginManager
 {
     #region Fields

     protected readonly CatalogSettings _catalogSettings;

     #endregion

     #region Ctor

     public SearchPluginManager(CatalogSettings catalogSettings, ICustomerService customerService, IPluginService pluginService)
         : base(customerService, pluginService)
     {
         _catalogSettings = catalogSettings;
     }

     #endregion

     #region Methods

     /// <summary>
     /// Load primary active search provider
     /// </summary>
     /// <param name="customer">Filter by customer; pass null to load all plugins</param>
     /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
     /// <returns>
     /// A task that represents the asynchronous operation
     /// The task result contains the search provider
     /// </returns>
     public virtual async Task<ISearchProvider> LoadPrimaryPluginAsync(Customer customer = null, int storeId = 0)
     {
         if (string.IsNullOrEmpty(_catalogSettings.ActiveSearchProviderSystemName))
             return null;

         return await LoadPrimaryPluginAsync(_catalogSettings.ActiveSearchProviderSystemName, customer, storeId);
     }

     /// <summary>
     /// Check whether the passed search provider is active
     /// </summary>
     /// <param name="searchProvider">Search provider to check</param>
     /// <returns>Result</returns>
     public virtual bool IsPluginActive(ISearchProvider searchProvider)
     {
         return IsPluginActive(searchProvider, [_catalogSettings.ActiveSearchProviderSystemName]);
     }

     /// <summary>
     /// Check whether the search provider with the passed system name is active
     /// </summary>
     /// <param name="systemName">System name of search provider to check</param>
     /// <param name="customer">Filter by customer; pass null to load all plugins</param>
     /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
     /// <returns>
     /// A task that represents the asynchronous operation
     /// The task result contains the result
     /// </returns>
     public virtual async Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0)
     {
         var searchProvider = await LoadPluginBySystemNameAsync(systemName, customer, storeId);
         return IsPluginActive(searchProvider);
     }

     #endregion
 }