
using Nop.Core.Plugins;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Represents a discount requirement rule
    /// </summary>
    public partial interface IDiscountRequirementRule : IPlugin
    {
        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        string SystemName { get; }

        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>true - requirement is met; otherwise, false</returns>
        bool CheckRequirement(CheckDiscountRequirementRequest request);
    }
}
