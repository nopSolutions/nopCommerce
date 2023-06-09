using Nop.Services.Plugins;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Represents a discount requirement rule
    /// </summary>
    public partial interface IDiscountRequirementRule : IPlugin
    {
        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<DiscountRequirementValidationResult> CheckRequirementAsync(DiscountRequirementValidationRequest request);

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>
        string GetConfigurationUrl(int discountId, int? discountRequirementId);
    }
}
