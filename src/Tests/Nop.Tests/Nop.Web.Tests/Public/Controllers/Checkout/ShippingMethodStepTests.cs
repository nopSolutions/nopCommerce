using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Shipping;
using Nop.Core.Http;
using Nop.Services.Localization;
using Nop.Web.Models.Checkout;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Controllers.Checkout;

/// <summary>
/// Characterization tests of the shipping-method pair: the multi-step SelectShippingMethod and the one page OpcSaveShippingMethod
/// </summary>
/// <remarks>
/// Each branch pins the exit and the stored SelectedShippingOption, SelectedPickupPoint and DesiredDeliveryDate
/// generic attributes (read back through IGenericAttributeService).
/// Pickup branches use the existing test provider "PickupPoint.TestProvider" (pickup points "1", "2", "3");
/// no new pickup point provider is added. The BypassShippingMethodSelectionIfOnlyOne branch uses
/// <see cref="SingleOptionTestShippingRateComputationMethod"/>, because the fixed rate test method offers two options.
/// After a successful OPC save, OpcLoadStepAfterShippingMethod is pinned with BypassPaymentMethodSelectionIfOnlyOne off,
/// so it renders the "payment-method" section.
/// </remarks>
[TestFixture]
public class ShippingMethodStepTests : CheckoutControllerTestBase
{
    #region Utilities

    private const string PickupPointName = "Test pint name 1";

    private static FormCollection Form(params (string Key, string Value)[] fields)
    {
        var values = new Dictionary<string, StringValues> { ["nextstep"] = "nextstep" };
        foreach (var (key, value) in fields)
            values[key] = value;

        return new FormCollection(values);
    }

    private static FormCollection PickupForm(string pickupPointId) =>
        Form(("PickupInStore", "true"), ("pickup-points-id", $"{pickupPointId}___{PickupPointProviderSystemName}"));

    private static string Option(string name, string systemName = FixedRateShippingMethodSystemName) => $"{name}___{systemName}";

    private static ShippingOption PreviouslySelectedOption() => new()
    {
        Name = "Previously selected option",
        Description = string.Empty,
        Rate = 1M,
        ShippingRateComputationMethodSystemName = FixedRateShippingMethodSystemName
    };

    private static PickupPoint PreviouslySelectedPickupPoint() => new()
    {
        Id = "3",
        Name = "Previously selected pickup point",
        ProviderSystemName = PickupPointProviderSystemName
    };

    private async Task ArrangeAsync(bool onePage, bool requiresShipping = true, bool pickupOnShippingMethodPage = false,
        bool bypassIfOnlyOne = false, string shippingMethodSystemName = FixedRateShippingMethodSystemName)
    {
        await Checkout.SetOrderSettingsAsync(settings =>
        {
            settings.CheckoutDisabled = false;
            settings.OnePageCheckoutEnabled = onePage;
            settings.AnonymousCheckoutAllowed = true;
            settings.DisplayPickupInStoreOnShippingMethodPage = pickupOnShippingMethodPage;
        });
        await Checkout.SetShippingSettingsAsync(settings =>
        {
            settings.AllowPickupInStore = pickupOnShippingMethodPage;
            settings.ActivePickupPointProviderSystemNames = [PickupPointProviderSystemName];
            settings.ActiveShippingRateComputationMethodSystemNames = [shippingMethodSystemName];
            settings.BypassShippingMethodSelectionIfOnlyOne = bypassIfOnlyOne;
            settings.AllowCustomerToChooseDeliveryDate = true;
        });
        await Checkout.SetPaymentSettingsAsync(settings =>
        {
            settings.ActivePaymentMethodSystemNames = [TestPaymentMethodSystemName];
            settings.BypassPaymentMethodSelectionIfOnlyOne = false;
        });

        await Checkout.ClearCartAsync();
        await Checkout.AddSampleProductToCartAsync(requiresShipping);
        await Checkout.ClearCheckoutAttributesAsync();
    }

    /// <summary>
    /// OpcLoadStepAfterShippingMethod with payment required and BypassPaymentMethodSelectionIfOnlyOne off
    /// </summary>
    private static void ShouldLoadPaymentMethodStep(IActionResult result)
    {
        var json = CheckoutResults.Json(result);
        json.Keys.Should().BeEquivalentTo("update_section", "goto_section");

        var (name, html) = CheckoutResults.UpdateSection(json);
        name.Should().Be("payment-method");
        html.Should().Be("<partial:OpcPaymentMethods>");
        json["goto_section"].Should().Be("payment_method");
    }

    /// <summary>
    /// The multi-step fallback "return await ShippingMethod()" re-renders the shipping method page
    /// </summary>
    private static void ShouldRenderShippingMethodView(IActionResult result)
    {
        var (viewName, modelType) = CheckoutResults.View(result);
        viewName.Should().BeNull();
        modelType.Should().Be(typeof(CheckoutShippingMethodModel));
    }

    private async Task ShouldHaveSavedPickupPointAsync()
    {
        var expectedName = string.Format(await GetService<ILocalizationService>().GetResourceAsync("Checkout.PickupPoints.Name"), PickupPointName);

        var selectedOption = await Checkout.GetSelectedShippingOptionAsync();
        selectedOption.Should().NotBeNull();
        selectedOption.IsPickupInStore.Should().BeTrue();
        selectedOption.Name.Should().Be(expectedName);
        selectedOption.ShippingRateComputationMethodSystemName.Should().Be(PickupPointProviderSystemName);

        var pickupPoint = await Checkout.GetSelectedPickupPointAsync();
        pickupPoint.Should().NotBeNull();
        pickupPoint.Id.Should().Be("1");
        pickupPoint.Name.Should().Be(PickupPointName);
        pickupPoint.ProviderSystemName.Should().Be(PickupPointProviderSystemName);

        (await Checkout.GetDesiredDeliveryDateAsync()).Should().BeNull();
    }

    private async Task ShouldHaveSavedOptionAsync(string name, string systemName, DateTime? desiredDeliveryDate = null)
    {
        var selectedOption = await Checkout.GetSelectedShippingOptionAsync();
        selectedOption.Should().NotBeNull();
        selectedOption.Name.Should().Be(name);
        selectedOption.ShippingRateComputationMethodSystemName.Should().Be(systemName);
        selectedOption.IsPickupInStore.Should().BeFalse();

        (await Checkout.GetDesiredDeliveryDateAsync()).Should().Be(desiredDeliveryDate);
    }

    private async Task ShouldHaveSavedNothingAsync()
    {
        (await Checkout.GetSelectedShippingOptionAsync()).Should().BeNull();
        (await Checkout.GetSelectedPickupPointAsync()).Should().BeNull();
        (await Checkout.GetDesiredDeliveryDateAsync()).Should().BeNull();
    }

    #endregion

    #region Shipping not required

    [Test]
    public async Task MultiStepShippingNotRequiredSavesNullOptionAndRedirectsToPaymentMethod()
    {
        await ArrangeAsync(onePage: false, requiresShipping: false);
        await Checkout.SetSelectedShippingOptionAsync(PreviouslySelectedOption());

        var result = await CreateController().SelectShippingMethod(Option("Shipping option 1"), Form());

        CheckoutResults.RouteName(result).Should().Be(NopRouteNames.Standard.CHECKOUT_PAYMENT_METHOD);
        (await Checkout.GetSelectedShippingOptionAsync()).Should().BeNull();
        (await Checkout.GetSelectedPickupPointAsync()).Should().BeNull();
        (await Checkout.GetDesiredDeliveryDateAsync()).Should().BeNull();
    }

    [Test]
    public async Task OpcShippingNotRequiredReturnsErrorAndSavesNothing()
    {
        await ArrangeAsync(onePage: true, requiresShipping: false);
        await Checkout.SetSelectedShippingOptionAsync(PreviouslySelectedOption());

        var result = await CreateController().OpcSaveShippingMethod(Option("Shipping option 1"), Form());

        CheckoutResults.JsonError(result).Should().Be("Shipping is not required");
        var selectedOption = await Checkout.GetSelectedShippingOptionAsync();
        selectedOption.Should().NotBeNull("OPC throws before saving anything");
        selectedOption.Name.Should().Be(PreviouslySelectedOption().Name);
    }

    #endregion

    #region Pickup in store

    [Test]
    public async Task MultiStepPickupChosenSavesPickupPointAndRedirectsToPaymentMethod()
    {
        await ArrangeAsync(onePage: false, pickupOnShippingMethodPage: true);

        var result = await CreateController().SelectShippingMethod(null, PickupForm("1"));

        CheckoutResults.RouteName(result).Should().Be(NopRouteNames.Standard.CHECKOUT_PAYMENT_METHOD);
        await ShouldHaveSavedPickupPointAsync();
    }

    [Test]
    public async Task OpcPickupChosenSavesPickupPointAndLoadsNextStep()
    {
        await ArrangeAsync(onePage: true, pickupOnShippingMethodPage: true);

        var result = await CreateController().OpcSaveShippingMethod(null, PickupForm("1"));

        ShouldLoadPaymentMethodStep(result);
        await ShouldHaveSavedPickupPointAsync();
    }

    [Test]
    public async Task MultiStepPickupOfferedButNotChosenClearsPickupPointAndSavesOption()
    {
        await ArrangeAsync(onePage: false, pickupOnShippingMethodPage: true);
        await Checkout.SetSelectedPickupPointAsync(PreviouslySelectedPickupPoint());

        var result = await CreateController().SelectShippingMethod(Option("Shipping option 1"), Form(("PickupInStore", "false")));

        CheckoutResults.RouteName(result).Should().Be(NopRouteNames.Standard.CHECKOUT_PAYMENT_METHOD);
        (await Checkout.GetSelectedPickupPointAsync()).Should().BeNull();
        await ShouldHaveSavedOptionAsync("Shipping option 1", FixedRateShippingMethodSystemName);
    }

    [Test]
    public async Task OpcPickupOfferedButNotChosenClearsPickupPointAndSavesOption()
    {
        await ArrangeAsync(onePage: true, pickupOnShippingMethodPage: true);
        await Checkout.SetSelectedPickupPointAsync(PreviouslySelectedPickupPoint());

        var result = await CreateController().OpcSaveShippingMethod(Option("Shipping option 1"), Form(("PickupInStore", "false")));

        ShouldLoadPaymentMethodStep(result);
        (await Checkout.GetSelectedPickupPointAsync()).Should().BeNull();
        await ShouldHaveSavedOptionAsync("Shipping option 1", FixedRateShippingMethodSystemName);
    }

    [Test]
    public async Task MultiStepPickupPointNotAllowedLetsTheExceptionEscape()
    {
        await ArrangeAsync(onePage: false, pickupOnShippingMethodPage: true);
        var controller = CreateController();

        var act = () => controller.SelectShippingMethod(null, PickupForm("99"));

        await act.Should().ThrowExactlyAsync<Exception>().WithMessage("Pickup point is not allowed");
        await ShouldHaveSavedNothingAsync();
    }

    [Test]
    public async Task OpcPickupPointNotAllowedReturnsError()
    {
        await ArrangeAsync(onePage: true, pickupOnShippingMethodPage: true);

        var result = await CreateController().OpcSaveShippingMethod(null, PickupForm("99"));

        CheckoutResults.JsonError(result).Should().Be("Pickup point is not allowed");
        await ShouldHaveSavedNothingAsync();
    }

    #endregion

    #region Unparsable or unknown option

    [TestCase(null)]
    [TestCase("")]
    [TestCase("Shipping option 1")]
    public async Task MultiStepEmptyOrMalformedOptionRendersShippingMethodView(string shippingOption)
    {
        await ArrangeAsync(onePage: false);

        var result = await CreateController().SelectShippingMethod(shippingOption, Form());

        ShouldRenderShippingMethodView(result);
        await ShouldHaveSavedNothingAsync();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("Shipping option 1")]
    public async Task OpcEmptyOrMalformedOptionReturnsError(string shippingOption)
    {
        await ArrangeAsync(onePage: true);

        var result = await CreateController().OpcSaveShippingMethod(shippingOption, Form());

        CheckoutResults.JsonError(result).Should().Be("Selected shipping method can't be parsed");
        await ShouldHaveSavedNothingAsync();
    }

    [Test]
    public async Task MultiStepOptionNotFoundRendersShippingMethodView()
    {
        await ArrangeAsync(onePage: false);

        var result = await CreateController().SelectShippingMethod(Option("Unknown option"), Form());

        ShouldRenderShippingMethodView(result);
        await ShouldHaveSavedNothingAsync();
    }

    [Test]
    public async Task OpcOptionNotFoundReturnsError()
    {
        await ArrangeAsync(onePage: true);

        var result = await CreateController().OpcSaveShippingMethod(Option("Unknown option"), Form());

        CheckoutResults.JsonError(result).Should().Be("Selected shipping method can't be loaded");
        await ShouldHaveSavedNothingAsync();
    }

    [Test]
    public async Task MultiStepInvalidOptionWithOnlyOneAvailableOptionAutoSavesItAndRedirects()
    {
        await ArrangeAsync(onePage: false, bypassIfOnlyOne: true,
            shippingMethodSystemName: SingleOptionTestShippingRateComputationMethod.SystemName);

        var result = await CreateController().SelectShippingMethod(string.Empty, Form());

        CheckoutResults.RouteName(result).Should().Be(NopRouteNames.Standard.CHECKOUT_PAYMENT_METHOD);
        await ShouldHaveSavedOptionAsync(SingleOptionTestShippingRateComputationMethod.OptionName,
            SingleOptionTestShippingRateComputationMethod.SystemName);
        (await Checkout.GetSelectedPickupPointAsync()).Should().BeNull();
    }

    #endregion

    #region Success

    private const string CachedOptionName = "Cached option";

    private static readonly DateTime _desiredDeliveryDate = new(2030, 1, 15);

    /// <summary>
    /// Cache offered options the shipping service would not return, so a successful save proves the cache was used;
    /// the option of another shipping method is filtered out, so the chosen option has index 0
    /// </summary>
    private async Task CacheOfferedOptionsAsync()
    {
        await Checkout.SetOfferedShippingOptionsAsync(
        [
            new ShippingOption { Name = "Other method option", Description = string.Empty, Rate = 3M, ShippingRateComputationMethodSystemName = "Shipping.OtherTestMethod" },
            new ShippingOption { Name = CachedOptionName, Description = string.Empty, Rate = 7M, ShippingRateComputationMethodSystemName = FixedRateShippingMethodSystemName }
        ]);
    }

    private static FormCollection DeliveryDateForm() =>
        Form(("ShippingMethods[0].DesiredDeliveryDate.SelectedDate", _desiredDeliveryDate.ToString("yyyy-MM-dd")));

    [Test]
    public async Task MultiStepSuccessWithCachedOptionsSavesOptionAndDeliveryDate()
    {
        await ArrangeAsync(onePage: false);
        await CacheOfferedOptionsAsync();

        var result = await CreateController().SelectShippingMethod(Option(CachedOptionName), DeliveryDateForm());

        CheckoutResults.RouteName(result).Should().Be(NopRouteNames.Standard.CHECKOUT_PAYMENT_METHOD);
        await ShouldHaveSavedOptionAsync(CachedOptionName, FixedRateShippingMethodSystemName, _desiredDeliveryDate);
        (await Checkout.GetSelectedPickupPointAsync()).Should().BeNull();
    }

    [Test]
    public async Task OpcSuccessWithCachedOptionsSavesOptionAndDeliveryDate()
    {
        await ArrangeAsync(onePage: true);
        await CacheOfferedOptionsAsync();

        var result = await CreateController().OpcSaveShippingMethod(Option(CachedOptionName), DeliveryDateForm());

        ShouldLoadPaymentMethodStep(result);
        await ShouldHaveSavedOptionAsync(CachedOptionName, FixedRateShippingMethodSystemName, _desiredDeliveryDate);
        (await Checkout.GetSelectedPickupPointAsync()).Should().BeNull();
    }

    [Test]
    public async Task MultiStepSuccessWithoutCachedOptionsLoadsThemFromShippingService()
    {
        await ArrangeAsync(onePage: false);

        var result = await CreateController().SelectShippingMethod(Option("Shipping option 2"), Form());

        CheckoutResults.RouteName(result).Should().Be(NopRouteNames.Standard.CHECKOUT_PAYMENT_METHOD);
        await ShouldHaveSavedOptionAsync("Shipping option 2", FixedRateShippingMethodSystemName);
        (await Checkout.GetSelectedPickupPointAsync()).Should().BeNull();
    }

    [Test]
    public async Task OpcSuccessWithoutCachedOptionsLoadsThemFromShippingService()
    {
        await ArrangeAsync(onePage: true);

        var result = await CreateController().OpcSaveShippingMethod(Option("Shipping option 2"), Form());

        ShouldLoadPaymentMethodStep(result);
        await ShouldHaveSavedOptionAsync("Shipping option 2", FixedRateShippingMethodSystemName);
        (await Checkout.GetSelectedPickupPointAsync()).Should().BeNull();
    }

    #endregion
}
