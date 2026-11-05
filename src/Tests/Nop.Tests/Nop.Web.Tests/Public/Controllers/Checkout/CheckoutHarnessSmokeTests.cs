using AwesomeAssertions;
using Nop.Core.Http;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Controllers.Checkout;

/// <summary>
/// Proves the DI controller harness works before characterization tests depend on it
/// </summary>
[TestFixture]
public class CheckoutHarnessSmokeTests : CheckoutControllerTestBase
{
    [Test]
    public async Task IndexWithEmptyCartRedirectsToCart()
    {
        await Checkout.SetOrderSettingsAsync(settings => settings.CheckoutDisabled = false);
        await Checkout.ClearCartAsync();

        var result = await CreateController().Index();

        CheckoutResults.RouteName(result).Should().Be(NopRouteNames.General.CART);
    }

    [Test]
    public async Task OpcSaveShippingMethodWhenShippingIsNotRequiredReturnsJsonError()
    {
        await Checkout.SetOrderSettingsAsync(settings =>
        {
            settings.CheckoutDisabled = false;
            settings.OnePageCheckoutEnabled = true;
        });
        await Checkout.ClearCartAsync();
        await Checkout.AddSampleProductToCartAsync(requiresShipping: false);

        var result = await CreateController().OpcSaveShippingMethod(null, null);

        var json = CheckoutResults.Json(result);
        json.Should().HaveCount(2);
        json["error"].Should().Be(1);
        json["message"].Should().Be("Shipping is not required");
    }
}
