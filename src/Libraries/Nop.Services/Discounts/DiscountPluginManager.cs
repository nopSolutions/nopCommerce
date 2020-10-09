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

        public DiscountPluginManager(ICustomerService customerService,
            IPluginService pluginService) : base(customerService, pluginService)
        {
        }

        #endregion
    }
}