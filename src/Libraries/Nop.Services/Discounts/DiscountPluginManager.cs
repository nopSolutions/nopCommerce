using Nop.Services.Caching;
using Nop.Services.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Represents a discount requirement plugin manager implementation
    /// </summary>
    public partial class DiscountPluginManager : PluginManager<IDiscountRequirementRule>, IDiscountPluginManager
    {
        #region Ctor

        public DiscountPluginManager(ICacheKeyService cacheKeyService,
            ICustomerService customerService,
            IPluginService pluginService) : base(cacheKeyService, customerService, pluginService)
        {
        }

        #endregion
    }
}