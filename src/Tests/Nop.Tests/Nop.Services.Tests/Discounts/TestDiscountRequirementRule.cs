using Nop.Services.Discounts;
using Nop.Services.Plugins;

namespace Nop.Tests.Nop.Services.Tests.Discounts;

public partial class TestDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
{
    private IDiscountService _discountService;

    public TestDiscountRequirementRule(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    /// <summary>
    /// Check discount requirement
    /// </summary>
    /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
    /// <returns>Result</returns>
    public async Task<DiscountRequirementValidationResult> CheckRequirementAsync(DiscountRequirementValidationRequest request)
    {
        var dr = await _discountService.GetDiscountRequirementByIdAsync(request.DiscountRequirementId);
        var valid = !dr.IsGroup;
        valid = valid && !dr.InteractionTypeId.HasValue;
            
        return new DiscountRequirementValidationResult
        {
            IsValid = valid
        };
    }

    /// <summary>
    /// Get URL for rule configuration
    /// </summary>
    /// <param name="discountId">Discount identifier</param>
    /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
    /// <returns>URL</returns>
    public string GetConfigurationUrl(int discountId, int? discountRequirementId)
    {
        throw new NotImplementedException();
    }
}