using Nop.Core.Domain.Shipping;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Tests.Nop.Web.Tests.Public.Controllers.Checkout;

/// <summary>
/// Test-only shipping rate computation method that offers exactly one shipping option.
/// Checkout tests use it to reach the "BypassShippingMethodSelectionIfOnlyOne" branch of CheckoutController.ShippingMethod
/// (FixedRateTestShippingRateComputationMethod always offers two options).
/// </summary>
public class SingleOptionTestShippingRateComputationMethod : BasePlugin, IShippingRateComputationMethod
{
    public const string SystemName = "Shipping.CheckoutSingleOptionTest";

    public const string OptionName = "Single test option";

    public Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
    {
        ArgumentNullException.ThrowIfNull(getShippingOptionRequest);

        var response = new GetShippingOptionResponse();
        response.ShippingOptions.Add(new ShippingOption
        {
            Name = OptionName,
            Description = string.Empty,
            Rate = 5M
        });

        return Task.FromResult(response);
    }

    public Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
    {
        ArgumentNullException.ThrowIfNull(getShippingOptionRequest);

        return Task.FromResult<decimal?>(5M);
    }

    public Task<IShipmentTracker> GetShipmentTrackerAsync()
    {
        return Task.FromResult<IShipmentTracker>(null);
    }
}
