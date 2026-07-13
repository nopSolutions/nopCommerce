using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Manual.Models;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.Manual.Components;

public class CreditCardInfoViewComponent : NopViewComponent
{
    #region Fields

    private readonly IEncryptionService _encryptionService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IOrderService _orderService;

    #endregion

    #region Ctor

    public CreditCardInfoViewComponent(IEncryptionService encryptionService, IGenericAttributeService genericAttributeService, IOrderService orderService)
    {
        _encryptionService = encryptionService;
        _genericAttributeService = genericAttributeService;
        _orderService = orderService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        if (!widgetZone.Equals(AdminWidgetZones.OrderDetailsInfoPaymentMethodAdditionalData) ||
            additionalData is not OrderModel orderModel || orderModel.IsLoggedInAsVendor)
            return Content(string.Empty);

        var order = await _orderService.GetOrderByIdAsync(orderModel.Id);

        if (order == null || !order.PaymentMethodSystemName.Equals(PaymentsManualDefaults.SystemName, StringComparison.InvariantCultureIgnoreCase))
            return Content(string.Empty);

        var json = await _genericAttributeService.GetAttributeAsync<string>(order, nameof(CreditCardInfo));

        if (string.IsNullOrEmpty(json))
            return Content(string.Empty);

        var creditCardInfo = JsonConvert.DeserializeObject<CreditCardInfo>(json);

        var model = new CreditCardInfoModel
        {
            OrderId = orderModel.Id,
            //card type
            CardType = _encryptionService.DecryptText(creditCardInfo.CardType),
            //cardholder name
            CardName = _encryptionService.DecryptText(creditCardInfo.CardName),
            //card number
            CardNumber = _encryptionService.DecryptText(creditCardInfo.CardNumber),
            //cvv
            CardCvv2 = _encryptionService.DecryptText(creditCardInfo.CardCvv2)
        };

        //expiry date
        var cardExpirationMonthDecrypted = _encryptionService.DecryptText(creditCardInfo.CardExpirationMonth);

        if (!string.IsNullOrEmpty(cardExpirationMonthDecrypted) && cardExpirationMonthDecrypted != "0")
            model.CardExpirationMonth = cardExpirationMonthDecrypted;

        var cardExpirationYearDecrypted = _encryptionService.DecryptText(creditCardInfo.CardExpirationYear);

        if (!string.IsNullOrEmpty(cardExpirationYearDecrypted) && cardExpirationYearDecrypted != "0")
            model.CardExpirationYear = cardExpirationYearDecrypted;

        return await ViewAsync("~/Plugins/Payments.Manual/Views/CreditCardInfo.cshtml", model);
    }

    #endregion
}