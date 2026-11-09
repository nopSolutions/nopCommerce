using System.Reflection;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core.Http;
using Nop.Services.Localization;
using Nop.Web.Controllers;
using Nop.Web.Models.Checkout;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Controllers.Checkout;

/// <summary>
/// Characterization tests of the guard block of the 20 guarded checkout actions
/// (checkout disabled → empty cart → wrong flow → guest with anonymous checkout disallowed)
/// </summary>
/// <remarks>
/// Every action is called for each single guard condition and for the precedence chain
/// (all four conditions, the last three, the last two), so the exit of the first-checked condition is pinned.
/// Test cases come from <see cref="GuardCases"/> (a TestCaseSource over action × condition) rather than
/// one [TestCase] attribute per pair; <see cref="EveryGuardedActionHasGuardTestCases"/> checks the coverage.
/// The guest condition puts the test customer (the admin) into the "Guests" role for the duration of the test.
/// Multi-step POST actions are called directly with a form that contains "nextstep",
/// as [FormValueRequired("nextstep")] requires over HTTP.
/// </remarks>
[TestFixture]
public class CheckoutGuardTests : CheckoutControllerTestBase
{
    #region Guard conditions and flows

    [Flags]
    public enum GuardCondition
    {
        CheckoutDisabled = 1,
        EmptyCart = 2,
        WrongFlow = 4,
        GuestNotAllowed = 8,

        /// <summary>
        /// Index only: a guest with anonymous checkout allowed, but a downloadable product in the cart
        /// and RequireRegistrationForDownloadableProducts on
        /// </summary>
        GuestWithDownloadableProducts = 16
    }

    public enum GuardFlow
    {
        /// <summary>
        /// Index: no flow-mode check; guest check includes downloadable products
        /// </summary>
        Index,

        /// <summary>
        /// Multi-step actions: wrong flow = one page checkout enabled
        /// </summary>
        MultiStep,

        /// <summary>
        /// OnePageCheckout GET: wrong flow = one page checkout disabled
        /// </summary>
        OnePageGet,

        /// <summary>
        /// OPC POST actions: exits are { error = 1, message } JSON plus a warning log record
        /// </summary>
        OnePagePost
    }

    private sealed record GuardedAction(GuardFlow Flow, Func<CheckoutController, Task<IActionResult>> Invoke);

    private static FormCollection NextStepForm() =>
        new(new Dictionary<string, StringValues> { ["nextstep"] = "nextstep" });

    /// <summary>
    /// The 20 guarded actions (SelectBillingAddress and SelectShippingAddress have a partial guard and are out of scope)
    /// </summary>
    private static readonly string[] _expectedGuardedActions =
    [
        "Index",
        "BillingAddress", "NewBillingAddress", "ShippingAddress", "NewShippingAddress",
        "ShippingMethod", "SelectShippingMethod", "PaymentMethod", "SelectPaymentMethod",
        "PaymentInfo", "EnterPaymentInfo", "Confirm", "ConfirmOrder",
        "OnePageCheckout", "OpcSaveBilling", "OpcSaveShipping", "OpcSaveShippingMethod",
        "OpcSavePaymentMethod", "OpcSavePaymentInfo", "OpcConfirmOrder"
    ];

    private static readonly Dictionary<string, GuardedAction> _actions = new()
    {
        [nameof(CheckoutController.Index)] = new(GuardFlow.Index, c => c.Index()),

        [nameof(CheckoutController.BillingAddress)] = new(GuardFlow.MultiStep, c => c.BillingAddress(new FormCollection(null))),
        [nameof(CheckoutController.NewBillingAddress)] = new(GuardFlow.MultiStep, c => c.NewBillingAddress(new CheckoutBillingAddressModel(), NextStepForm())),
        [nameof(CheckoutController.ShippingAddress)] = new(GuardFlow.MultiStep, c => c.ShippingAddress()),
        [nameof(CheckoutController.NewShippingAddress)] = new(GuardFlow.MultiStep, c => c.NewShippingAddress(new CheckoutShippingAddressModel(), NextStepForm())),
        [nameof(CheckoutController.ShippingMethod)] = new(GuardFlow.MultiStep, c => c.ShippingMethod()),
        [nameof(CheckoutController.SelectShippingMethod)] = new(GuardFlow.MultiStep, c => c.SelectShippingMethod(null, NextStepForm())),
        [nameof(CheckoutController.PaymentMethod)] = new(GuardFlow.MultiStep, c => c.PaymentMethod()),
        [nameof(CheckoutController.SelectPaymentMethod)] = new(GuardFlow.MultiStep, c => c.SelectPaymentMethod(null, new CheckoutPaymentMethodModel())),
        [nameof(CheckoutController.PaymentInfo)] = new(GuardFlow.MultiStep, c => c.PaymentInfo()),
        [nameof(CheckoutController.EnterPaymentInfo)] = new(GuardFlow.MultiStep, c => c.EnterPaymentInfo(NextStepForm())),
        [nameof(CheckoutController.Confirm)] = new(GuardFlow.MultiStep, c => c.Confirm()),
        [nameof(CheckoutController.ConfirmOrder)] = new(GuardFlow.MultiStep, c => c.ConfirmOrder(false)),

        [nameof(CheckoutController.OnePageCheckout)] = new(GuardFlow.OnePageGet, c => c.OnePageCheckout()),
        [nameof(CheckoutController.OpcSaveBilling)] = new(GuardFlow.OnePagePost, c => c.OpcSaveBilling(new CheckoutBillingAddressModel(), new FormCollection(null))),
        [nameof(CheckoutController.OpcSaveShipping)] = new(GuardFlow.OnePagePost, c => c.OpcSaveShipping(new CheckoutShippingAddressModel(), new FormCollection(null))),
        [nameof(CheckoutController.OpcSaveShippingMethod)] = new(GuardFlow.OnePagePost, c => c.OpcSaveShippingMethod(null, new FormCollection(null))),
        [nameof(CheckoutController.OpcSavePaymentMethod)] = new(GuardFlow.OnePagePost, c => c.OpcSavePaymentMethod(null, new CheckoutPaymentMethodModel())),
        [nameof(CheckoutController.OpcSavePaymentInfo)] = new(GuardFlow.OnePagePost, c => c.OpcSavePaymentInfo(new FormCollection(null))),
        //the captcha is disabled in every case, so the guard nested in the captcha branch runs
        [nameof(CheckoutController.OpcConfirmOrder)] = new(GuardFlow.OnePagePost, c => c.OpcConfirmOrder(false))
    };

    private static IEnumerable<GuardCondition> ConditionsFor(GuardFlow flow)
    {
        const GuardCondition all = GuardCondition.CheckoutDisabled | GuardCondition.EmptyCart | GuardCondition.WrongFlow | GuardCondition.GuestNotAllowed;

        if (flow == GuardFlow.Index)
        {
            //Index has no flow-mode check
            return
            [
                GuardCondition.CheckoutDisabled,
                GuardCondition.EmptyCart,
                GuardCondition.GuestNotAllowed,
                GuardCondition.GuestWithDownloadableProducts,
                //precedence
                GuardCondition.CheckoutDisabled | GuardCondition.EmptyCart | GuardCondition.GuestNotAllowed,
                GuardCondition.EmptyCart | GuardCondition.GuestNotAllowed
            ];
        }

        return
        [
            GuardCondition.CheckoutDisabled,
            GuardCondition.EmptyCart,
            GuardCondition.WrongFlow,
            GuardCondition.GuestNotAllowed,
            //precedence
            all,
            all & ~GuardCondition.CheckoutDisabled,
            GuardCondition.WrongFlow | GuardCondition.GuestNotAllowed
        ];
    }

    public static IEnumerable<TestCaseData> GuardCases()
    {
        foreach (var (actionName, action) in _actions)
            foreach (var conditions in ConditionsFor(action.Flow))
                yield return new TestCaseData(actionName, conditions)
                    .SetName($"Guard_{actionName}_{conditions.ToString().Replace(", ", "_And_")}");
    }

    #endregion

    #region Utilities

    private async Task ArrangeAsync(GuardFlow flow, GuardCondition conditions)
    {
        var opcAction = flow is GuardFlow.OnePageGet or GuardFlow.OnePagePost;
        var wrongFlow = conditions.HasFlag(GuardCondition.WrongFlow);

        await Checkout.SetOrderSettingsAsync(settings =>
        {
            settings.CheckoutDisabled = conditions.HasFlag(GuardCondition.CheckoutDisabled);
            //Index has no flow-mode check, so it runs with the default (one page checkout enabled)
            settings.OnePageCheckoutEnabled = flow == GuardFlow.Index || opcAction != wrongFlow;
            settings.AnonymousCheckoutAllowed = !conditions.HasFlag(GuardCondition.GuestNotAllowed);
        });
        await Checkout.SetCustomerSettingsAsync(settings =>
            settings.RequireRegistrationForDownloadableProducts = conditions.HasFlag(GuardCondition.GuestWithDownloadableProducts));
        await Checkout.SetCaptchaSettingsAsync(settings => settings.Enabled = false);

        await Checkout.ClearCartAsync();
        if (!conditions.HasFlag(GuardCondition.EmptyCart))
        {
            var product = await Checkout.AddSampleProductToCartAsync();

            if (conditions.HasFlag(GuardCondition.GuestWithDownloadableProducts))
                await Checkout.UpdateProductAsync(product, p => p.IsDownload = true);
        }

        if (conditions.HasFlag(GuardCondition.GuestNotAllowed) || conditions.HasFlag(GuardCondition.GuestWithDownloadableProducts))
            await Checkout.MakeCurrentCustomerGuestAsync();
    }

    /// <summary>
    /// The guard condition that is checked first among the ones that are true
    /// </summary>
    private static GuardCondition FirstChecked(GuardCondition conditions)
    {
        return new[]
        {
            GuardCondition.CheckoutDisabled,
            GuardCondition.EmptyCart,
            GuardCondition.WrongFlow,
            GuardCondition.GuestNotAllowed,
            GuardCondition.GuestWithDownloadableProducts
        }.First(condition => conditions.HasFlag(condition));
    }

    private async Task<string> ExpectedOpcMessageAsync(GuardCondition condition)
    {
        return condition switch
        {
            GuardCondition.CheckoutDisabled => await GetService<ILocalizationService>().GetResourceAsync("Checkout.Disabled"),
            GuardCondition.EmptyCart => "Your cart is empty",
            GuardCondition.WrongFlow => "One page checkout is disabled",
            GuardCondition.GuestNotAllowed => "Anonymous checkout is not allowed",
            _ => throw new ArgumentOutOfRangeException(nameof(condition))
        };
    }

    #endregion

    #region Tests

    [TestCaseSource(nameof(GuardCases))]
    public async Task GuardExit(string actionName, GuardCondition conditions)
    {
        var action = _actions[actionName];
        await ArrangeAsync(action.Flow, conditions);
        var customer = await Checkout.GetCurrentCustomerAsync();

        var result = await action.Invoke(CreateController());

        var firstChecked = FirstChecked(conditions);
        switch (action.Flow)
        {
            case GuardFlow.Index:
            case GuardFlow.MultiStep:
            case GuardFlow.OnePageGet:
                if (firstChecked is GuardCondition.GuestNotAllowed or GuardCondition.GuestWithDownloadableProducts)
                {
                    CheckoutResults.Challenge(result);
                    break;
                }

                var expectedRoute = firstChecked switch
                {
                    GuardCondition.CheckoutDisabled or GuardCondition.EmptyCart => NopRouteNames.General.CART,
                    GuardCondition.WrongFlow when action.Flow == GuardFlow.MultiStep => NopRouteNames.Standard.CHECKOUT_ONE_PAGE,
                    GuardCondition.WrongFlow => NopRouteNames.Standard.CHECKOUT,
                    _ => throw new ArgumentOutOfRangeException(nameof(conditions))
                };
                CheckoutResults.RouteName(result).Should().Be(expectedRoute);
                break;

            case GuardFlow.OnePagePost:
                var expectedMessage = await ExpectedOpcMessageAsync(firstChecked);
                CheckoutResults.JsonError(result).Should().Be(expectedMessage);

                var logs = await Checkout.GetWarningLogsSinceCaptureAsync(customer.Id);
                logs.Should().ContainSingle(log => log.ShortMessage == expectedMessage,
                    "the OPC catch block logs the guard exception as a warning for the current customer");
                break;
        }
    }

    [Test]
    public void EveryGuardedActionHasGuardTestCases()
    {
        _expectedGuardedActions.Should().HaveCount(20).And.OnlyHaveUniqueItems();

        foreach (var actionName in _expectedGuardedActions)
        {
            var methods = typeof(CheckoutController)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.Name == actionName && method.DeclaringType == typeof(CheckoutController))
                .ToList();

            methods.Should().ContainSingle($"{actionName} is a public action of CheckoutController");
            var method = methods.Single();
            method.IsVirtual.Should().BeTrue($"{actionName} is public virtual");
            method.ReturnType.Should().Be(typeof(Task<IActionResult>));
        }

        var coveredActions = GuardCases().Select(testCase => (string)testCase.Arguments[0]).Distinct().ToList();
        coveredActions.Should().BeEquivalentTo(_expectedGuardedActions);

        foreach (var actionName in _expectedGuardedActions)
        {
            var conditions = GuardCases()
                .Where(testCase => (string)testCase.Arguments[0] == actionName)
                .Select(testCase => (GuardCondition)testCase.Arguments[1])
                .ToList();

            conditions.Should().Contain([GuardCondition.CheckoutDisabled, GuardCondition.EmptyCart, GuardCondition.GuestNotAllowed]);

            if (actionName != nameof(CheckoutController.Index))
                conditions.Should().Contain(GuardCondition.WrongFlow);
            else
                conditions.Should().Contain(GuardCondition.GuestWithDownloadableProducts);
        }
    }

    #endregion
}
