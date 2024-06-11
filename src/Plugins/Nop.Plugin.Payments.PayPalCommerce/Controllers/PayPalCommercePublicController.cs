using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Plugin.Payments.PayPalCommerce.Factories;
using Nop.Plugin.Payments.PayPalCommerce.Models.Public;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.PayPalCommerce.Controllers;

[AutoValidateAntiforgeryToken]
public class PayPalCommercePublicController : BasePublicController
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IWebHelper _webHelper;
    private readonly PayPalCommerceModelFactory _modelFactory;

    #endregion

    #region Ctor

    public PayPalCommercePublicController(ILocalizationService localizationService,
        INotificationService notificationService,
        IWebHelper webHelper,
        PayPalCommerceModelFactory modelFactory)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _webHelper = webHelper;
        _modelFactory = modelFactory;
    }

    #endregion

    #region Methods

    #region Checkout

    public async Task<IActionResult> PluginPaymentInfo()
    {
        var model = await _modelFactory.PrepareCheckoutPaymentInfoModelAsync();

        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/PluginPaymentInfo.cshtml", model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public async Task<IActionResult> ValidateShoppingCart()
    {
        var warnings = await _modelFactory.GetShoppingCartWarningsAsync();
        if (warnings?.Any() ?? false)
            return ErrorJson(warnings.ToArray());

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(int placement, string paymentSource, int cardId, bool saveCard)
    {
        var model = await _modelFactory.PrepareOrderModelAsync((ButtonPlacement)placement, null, paymentSource, cardId, saveCard);
        if (model.LoginIsRequired)
            return Json(new { redirect = Url.RouteUrl("Login", new { returnUrl = Url.RouteUrl(PayPalCommerceDefaults.Route.ShoppingCart) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.ShoppingCart) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        //customer can complete 3D Secure if prompted
        if (cardId > 0 && !string.IsNullOrEmpty(model.PayerActionUrl))
            return Json(new { redirect = model.PayerActionUrl });

        return Json(new { orderId = model.OrderId, status = model.Status });
    }

    [HttpPost]
    public async Task<IActionResult> GetOrderStatus(int placement, string orderId)
    {
        var model = await _modelFactory.PrepareOrderModelAsync((ButtonPlacement)placement, orderId, null, null, false);
        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        return Json(new { status = model.Status, payerAction = model.PayerActionUrl });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderShipping(OrderShippingModel model)
    {
        model = await _modelFactory.PrepareOrderShippingModelAsync(model);
        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> ApproveOrder(string orderId, string liabilityShift)
    {
        var model = await _modelFactory.PrepareOrderApprovedModelAsync(orderId, liabilityShift);
        if (model.LoginIsRequired)
            return Json(new { redirect = Url.RouteUrl("Login", new { returnUrl = Url.RouteUrl(PayPalCommerceDefaults.Route.ShoppingCart) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.ShoppingCart) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        //order is approved but the customer must confirm it before
        if (!model.PayNow)
        {
            return Json(new
            {
                redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.ConfirmOrder, new { orderId = model.OrderId, liabilityShift = liabilityShift })
            });
        }

        //or pay it right now
        var completedModel = await _modelFactory.PrepareOrderCompletedModelAsync(orderId, liabilityShift);
        if (!string.IsNullOrEmpty(completedModel.Error))
            return ErrorJson(completedModel.Error);

        return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.CheckoutCompleted, new { orderId = completedModel.OrderId }) });
    }

    public async Task<IActionResult> ConfirmOrder(string orderId, string token, string liabilityShift, bool approve)
    {
        if (string.IsNullOrEmpty(liabilityShift))
            liabilityShift = _webHelper.QueryString<string>("liability_shift");

        var model = await _modelFactory.PrepareOrderConfirmModelAsync(orderId, token, liabilityShift, approve);
        if (model.LoginIsRequired)
            return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl(PayPalCommerceDefaults.Route.ShoppingCart) });

        if (!model.CheckoutIsEnabled)
            return RedirectToRoute(PayPalCommerceDefaults.Route.ShoppingCart);

        if (!string.IsNullOrEmpty(model.Error))
            _notificationService.ErrorNotification(model.Error);

        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/ConfirmOrder.cshtml", model);
    }

    [ValidateCaptcha]
    [HttpPost]
    public async Task<IActionResult> ConfirmOrderPost(string orderId, string orderGuid, string liabilityShift, bool captchaValid)
    {
        var model = await _modelFactory.PrepareOrderConfirmModelAsync(orderId, orderGuid, null, false);
        if (model.LoginIsRequired)
            return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl(PayPalCommerceDefaults.Route.ShoppingCart) });

        if (!model.CheckoutIsEnabled)
            return RedirectToRoute(PayPalCommerceDefaults.Route.ShoppingCart);

        if (!string.IsNullOrEmpty(model.Error))
            _notificationService.ErrorNotification(model.Error);

        if (model.DisplayCaptcha && !captchaValid)
        {
            model.Warnings.Add(await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            return View("~/Plugins/Payments.PayPalCommerce/Views/Public/ConfirmOrder.cshtml", model);
        }

        var completedModel = await _modelFactory.PrepareOrderCompletedModelAsync(orderId, liabilityShift);

        if (!string.IsNullOrEmpty(completedModel.Error))
        {
            _notificationService.ErrorNotification(completedModel.Error);
            return View("~/Plugins/Payments.PayPalCommerce/Views/Public/ConfirmOrder.cshtml", model);
        }

        if (!string.IsNullOrEmpty(completedModel.Warning))
            _notificationService.ErrorNotification(completedModel.Warning);

        return RedirectToRoute(PayPalCommerceDefaults.Route.CheckoutCompleted, new { orderId = completedModel.OrderId });
    }

    [HttpPost]
    public async Task<IActionResult> AppleTransactionInfo(int placement)
    {
        var model = await _modelFactory.PrepareApplePayModelAsync((ButtonPlacement)placement);
        if (model.LoginIsRequired)
            return Json(new { redirect = Url.RouteUrl("Login", new { returnUrl = Url.RouteUrl(PayPalCommerceDefaults.Route.ShoppingCart) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.ShoppingCart) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        var totalItem = model.Items.FirstOrDefault(item => item.Type == "TOTAL");
        var items = model.Items.Where(item => item.Type != "TOTAL");

        var transactionInfo = new
        {
            currencyCode = model.CurrencyCode,
            billingContact = model.BillingAddress is null ? null : new
            {
                emailAddress = model.BillingAddress.Email,
                givenName = model.BillingAddress.FirstName,
                familyName = model.BillingAddress.LastName,
                addressLines = model.BillingAddress.AddressLines?.Where(line => !string.IsNullOrEmpty(line)).ToArray(),
                locality = model.BillingAddress.City,
                subAdministrativeArea = model.BillingAddress.County,
                administrativeArea = model.BillingAddress.State,
                countryCode = model.BillingAddress.Country,
                postalCode = model.BillingAddress.PostalCode
            },
            shippingContact = model.ShippingAddress is null ? null : new
            {
                emailAddress = model.ShippingAddress.Email,
                givenName = model.ShippingAddress.FirstName,
                familyName = model.ShippingAddress.LastName,
                addressLines = model.ShippingAddress.AddressLines?.Where(line => !string.IsNullOrEmpty(line)).ToArray(),
                locality = model.ShippingAddress.City,
                administrativeArea = model.ShippingAddress.State,
                countryCode = model.ShippingAddress.Country,
                postalCode = model.ShippingAddress.PostalCode
            },
            //shippingType = model.ShippingAddress is null ? null : (model.ShippingAddress.PickupInStore ? "storePickup" : "shipping"),
            //shippingContactEditingMode = model.ShippingAddress is null ? null : "storePickup",
            shippingMethods = model.ShippingOptions
                ?.Select(option => new { identifier = option.Id, amount = option.Price, label = option.Label, detail = option.Description })
                .ToArray(),
            total = new { label = totalItem.Label, type = totalItem.Status, amount = totalItem.Price },
            lineItems = items
                .Select(item => new { label = item.Label, type = item.Status, amount = item.Price })
                .ToArray()
        };

        return Json(new { transactionInfo = transactionInfo });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAppleShipping(ApplePayShippingModel model)
    {
        model = await _modelFactory.PrepareApplePayShippingModelAsync(model);
        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        var totalItem = model.Items.FirstOrDefault(item => item.Type == "TOTAL");
        var items = model.Items.Where(item => item.Type != "TOTAL");

        //update total, items and shipping methods
        var shippingMethods = model.ShippingOptions
            ?.Select(option => new { identifier = option.Id, amount = option.Price, label = option.Label, detail = option.Description })
            .ToArray();
        var total = new { label = totalItem.Label, type = totalItem.Status, amount = totalItem.Price };
        var lineItems = items
            .Select(item => new { label = item.Label, type = item.Status, amount = item.Price })
            .ToArray();

        return Json(new { shippingMethods = shippingMethods, total = total, lineItems = lineItems });
    }

    [HttpPost]
    public async Task<IActionResult> GoogleTransactionInfo(int placement)
    {
        var model = await _modelFactory.PrepareGooglePayModelAsync((ButtonPlacement)placement);
        if (model.LoginIsRequired)
            return Json(new { redirect = Url.RouteUrl("Login", new { returnUrl = Url.RouteUrl(PayPalCommerceDefaults.Route.ShoppingCart) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.ShoppingCart) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        var totalItem = model.Items.FirstOrDefault(item => item.Type == "TOTAL");
        var items = model.Items.Where(item => item.Type != "TOTAL");

        var transactionInfo = new
        {
            currencyCode = model.CurrencyCode,
            countryCode = model.Country,
            checkoutOption = "DEFAULT",
            totalPriceStatus = totalItem.Status,
            totalPrice = totalItem.Price,
            totalPriceLabel = totalItem.Label,
            displayItems = items.Select(item => new
            {
                type = item.Type,
                label = item.Label,
                price = item.Price,
                status = item.Status
            }).ToArray()
        };

        var callbacks = new List<string> { "PAYMENT_AUTHORIZATION" };
        if (model.ShippingIsRequired)
            callbacks.AddRange(new List<string> { "SHIPPING_ADDRESS", "SHIPPING_OPTION" });

        return Json(new { transactionInfo = transactionInfo, shipping = model.ShippingIsRequired, callbacks = callbacks.ToArray() });
    }

    [HttpPost]
    public async Task<IActionResult> CheckGoogleShipping(int placement, int? productId)
    {
        var (shippingIsRequired, error) = await _modelFactory.CheckShippingIsRequiredAsync(productId);
        if (!string.IsNullOrEmpty(error))
            return ErrorJson(error);

        return Json(new { shippingIsRequired = shippingIsRequired && placement != (int)ButtonPlacement.PaymentMethod });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateGoogleShipping(GooglePayShippingModel model)
    {
        model = await _modelFactory.PrepareGooglePayShippingModelAsync(model);
        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        var totalItem = model.Items.FirstOrDefault(item => item.Type == "TOTAL");
        var items = model.Items.Where(item => item.Type != "TOTAL");

        var transactionInfo = new
        {
            currencyCode = model.CurrencyCode,
            countryCode = model.Country,
            checkoutOption = "DEFAULT",
            totalPriceStatus = totalItem.Status,
            totalPrice = totalItem.Price,
            totalPriceLabel = totalItem.Label,
            displayItems = items.Select(item => new
            {
                type = item.Type,
                label = item.Label,
                price = item.Price,
                status = item.Status
            }).ToArray()
        };

        var options = model.Options?.Select(option => new
        {
            id = option.Id,
            label = option.Label,
            description = option.Description
        }).ToArray();

        return Json(new { transactionInfo = transactionInfo, options = new { shippingOptions = options, defaultSelectedOptionId = model.OptionId } });
    }

    #endregion

    #region Payment tokens

    public async Task<IActionResult> PaymentTokens()
    {
        var model = await _modelFactory.PreparePaymentTokenListModelAsync();
        if (!model.VaultIsEnabled)
            return RedirectToRoute(PayPalCommerceDefaults.Route.CustomerInfo);

        if (!string.IsNullOrEmpty(model.Error))
            _notificationService.ErrorNotification(model.Error);

        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/PaymentTokens.cshtml", model);

    }

    [HttpPost]
    public async Task<IActionResult> PaymentTokensDelete(int tokenId)
    {
        var model = await _modelFactory.PreparePaymentTokenListModelAsync(deleteTokenId: tokenId);
        if (!model.VaultIsEnabled)
            return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.CustomerInfo) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.PaymentTokens) });
    }

    [HttpPost]
    public async Task<IActionResult> PaymentTokensMarkDefault(int tokenId)
    {
        var model = await _modelFactory.PreparePaymentTokenListModelAsync(defaultTokenId: tokenId);
        if (!model.VaultIsEnabled)
            return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.CustomerInfo) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.PaymentTokens) });
    }

    [HttpPost]
    public async Task<IActionResult> GetSavedCards(int placement)
    {
        var model = await _modelFactory.PrepareSavedCardListModelAsync((ButtonPlacement)placement);
        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        if (model.PaymentTokens?.Any() != true)
            return Json(new { });

        var cards = model.PaymentTokens.Select(token => new { id = token.Id, label = token.Title }).ToArray();

        return Json(new { cards = cards, defaultId = model.PaymentTokens.FirstOrDefault().Id });
    }

    #endregion

    #endregion
}