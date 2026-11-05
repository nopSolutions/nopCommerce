using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core.Http;
using Nop.Services.Orders;
using Nop.Tests.Nop.Services.Tests.Payments;
using Nop.Web.Models.Checkout;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Controllers.Checkout;

/// <summary>
/// Characterization tests of the payment-info pair: the multi-step EnterPaymentInfo and the one page OpcSavePaymentInfo
/// </summary>
/// <remarks>
/// The payment method is the existing test plugin "Payments.TestMethod" (TestPaymentMethod). To reach the
/// "invalid form" branch, TestPaymentMethod.ValidatePaymentFormAsync was extended (test code only) to return the value
/// of the <see cref="TestPaymentMethod.ValidationWarningFormKey"/> form field as a warning; without that field it
/// returns no warnings, as before.
/// Whether the process payment request was stored is read through IOrderProcessingService.GetProcessPaymentRequestAsync.
/// Rendered branches are pinned through the harness marker strings (partials) or by view name and model type (views);
/// every branch of the pair can be pinned this way.
/// </remarks>
[TestFixture]
public class PaymentInfoStepTests : CheckoutControllerTestBase
{
    #region Utilities

    private const string ValidationWarning = "Test payment form warning";

    private static FormCollection Form(params (string Key, string Value)[] fields)
    {
        var values = new Dictionary<string, StringValues> { ["nextstep"] = "nextstep" };
        foreach (var (key, value) in fields)
            values[key] = value;

        return new FormCollection(values);
    }

    private static FormCollection InvalidForm() => Form((TestPaymentMethod.ValidationWarningFormKey, ValidationWarning));

    private async Task ArrangeAsync(bool onePage, string selectedPaymentMethod = TestPaymentMethodSystemName, bool freeOrder = false)
    {
        await Checkout.SetOrderSettingsAsync(settings =>
        {
            settings.CheckoutDisabled = false;
            settings.OnePageCheckoutEnabled = onePage;
            settings.AnonymousCheckoutAllowed = true;
        });
        await Checkout.SetPaymentSettingsAsync(settings =>
            settings.ActivePaymentMethodSystemNames = [TestPaymentMethodSystemName]);

        await Checkout.ClearCartAsync();
        //a free order without shipping has a zero total, so the payment workflow is not required
        var product = await Checkout.AddSampleProductToCartAsync(requiresShipping: !freeOrder);
        if (freeOrder)
            await Checkout.UpdateProductAsync(product, p => p.Price = decimal.Zero);

        await Checkout.ClearCheckoutAttributesAsync();
        if (!string.IsNullOrEmpty(selectedPaymentMethod))
            await Checkout.SetSelectedPaymentMethodAsync(selectedPaymentMethod);
    }

    private async Task ShouldHaveStoredProcessPaymentRequestAsync(bool stored)
    {
        var processPaymentRequest = await ResolveFresh<IOrderProcessingService>().GetProcessPaymentRequestAsync();

        if (stored)
            processPaymentRequest.Should().NotBeNull();
        else
            processPaymentRequest.Should().BeNull();
    }

    #endregion

    #region Payment workflow not required

    [Test]
    public async Task MultiStepPaymentWorkflowNotRequiredRedirectsToConfirm()
    {
        await ArrangeAsync(onePage: false, freeOrder: true);

        var result = await CreateController().EnterPaymentInfo(InvalidForm());

        CheckoutResults.RouteName(result).Should().Be(NopRouteNames.Standard.CHECKOUT_CONFIRM);
        await ShouldHaveStoredProcessPaymentRequestAsync(false);
    }

    #endregion

    #region No selected payment method

    [Test]
    public async Task MultiStepNoSelectedMethodRedirectsToPaymentMethod()
    {
        await ArrangeAsync(onePage: false, selectedPaymentMethod: null);

        var result = await CreateController().EnterPaymentInfo(Form());

        CheckoutResults.RouteName(result).Should().Be(NopRouteNames.Standard.CHECKOUT_PAYMENT_METHOD);
        await ShouldHaveStoredProcessPaymentRequestAsync(false);
    }

    [Test]
    public async Task OpcNoSelectedMethodReturnsError()
    {
        await ArrangeAsync(onePage: true, selectedPaymentMethod: null);

        var result = await CreateController().OpcSavePaymentInfo(Form());

        CheckoutResults.JsonError(result).Should().Be("Payment method is not selected");
        await ShouldHaveStoredProcessPaymentRequestAsync(false);
    }

    #endregion

    #region Valid form

    [Test]
    public async Task MultiStepValidFormStoresRequestAndRedirectsToConfirm()
    {
        await ArrangeAsync(onePage: false);

        var result = await CreateController().EnterPaymentInfo(Form());

        CheckoutResults.RouteName(result).Should().Be(NopRouteNames.Standard.CHECKOUT_CONFIRM);
        await ShouldHaveStoredProcessPaymentRequestAsync(true);
    }

    [Test]
    public async Task OpcValidFormStoresRequestAndRendersConfirmOrder()
    {
        await ArrangeAsync(onePage: true);

        var result = await CreateController().OpcSavePaymentInfo(Form());

        var json = CheckoutResults.Json(result);
        json.Keys.Should().BeEquivalentTo("update_section", "goto_section");
        var (name, html) = CheckoutResults.UpdateSection(json);
        name.Should().Be("confirm-order");
        html.Should().Be("<partial:OpcConfirmOrder>");
        json["goto_section"].Should().Be("confirm_order");
        await ShouldHaveStoredProcessPaymentRequestAsync(true);
    }

    #endregion

    #region Invalid form

    [Test]
    public async Task MultiStepInvalidFormRendersPaymentInfoView()
    {
        await ArrangeAsync(onePage: false);
        var controller = CreateController();

        var result = await controller.EnterPaymentInfo(InvalidForm());

        var (viewName, modelType) = CheckoutResults.View(result);
        viewName.Should().BeNull();
        modelType.Should().Be(typeof(CheckoutPaymentInfoModel));
        controller.ModelState.IsValid.Should().BeFalse();
        controller.ModelState[string.Empty]!.Errors.Select(error => error.ErrorMessage).Should().Equal(ValidationWarning);
        await ShouldHaveStoredProcessPaymentRequestAsync(false);
    }

    [Test]
    public async Task OpcInvalidFormRendersPaymentInfoSection()
    {
        await ArrangeAsync(onePage: true);
        var controller = CreateController();

        var result = await controller.OpcSavePaymentInfo(InvalidForm());

        var json = CheckoutResults.Json(result);
        json.Keys.Should().BeEquivalentTo("update_section");
        var (name, html) = CheckoutResults.UpdateSection(json);
        name.Should().Be("payment-info");
        html.Should().Be("<partial:OpcPaymentInfo>");
        controller.ModelState.IsValid.Should().BeFalse();
        controller.ModelState[string.Empty]!.Errors.Select(error => error.ErrorMessage).Should().Equal(ValidationWarning);
        await ShouldHaveStoredProcessPaymentRequestAsync(false);
    }

    #endregion
}
