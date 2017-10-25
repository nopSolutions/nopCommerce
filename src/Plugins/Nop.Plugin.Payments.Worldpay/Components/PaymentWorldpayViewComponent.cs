using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Models;
using Nop.Plugin.Payments.Worldpay.Services;
using Nop.Services.Common;
using Nop.Services.Localization;

namespace Nop.Plugin.Payments.Worldpay.Components
{
    [ViewComponent(Name = "PaymentWorldpay")]
    public class PaymentWorldpayViewComponent : ViewComponent
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly WorldpayPaymentManager _worldpayPaymentManager;

        #endregion

        #region Ctor

        public PaymentWorldpayViewComponent(ILocalizationService localizationService,
            IWorkContext workContext,
            WorldpayPaymentManager worldpayPaymentManager)
        {
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._worldpayPaymentManager = worldpayPaymentManager;
        }

        #endregion

        #region Methods

        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel();

            //prepare years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //prepare months
            for (var i = 1; i <= 12; i++)
            {
                model.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            //prepare card types
            model.CardTypes = Enum.GetValues(typeof(CreditCardType)).OfType<CreditCardType>().Select(cardType => new SelectListItem
            {
                Value = JsonConvert.SerializeObject(cardType, new StringEnumConverter()),
                Text = cardType.GetLocalizedEnum(_localizationService, _workContext)
            }).ToList();

            //whether current customer is guest
            model.IsGuest = _workContext.CurrentCustomer.IsGuest();
            if (!model.IsGuest)
            {
                //whether customer already has stored cards
                var customer = _worldpayPaymentManager.GetCustomer(_workContext.CurrentCustomer.GetAttribute<string>(WorldpayPaymentDefaults.CustomerIdAttribute));
                if (customer?.PaymentMethods != null)
                {
                    model.StoredCards = customer.PaymentMethods.Where(method => method?.Card != null)
                        .Select(method => new SelectListItem { Text = method.Card.MaskedNumber, Value = method.PaymentId }).ToList();
                }

                //add the special item for 'select card' with empty GUID value 
                if (model.StoredCards.Any())
                {
                    var selectCardText = _localizationService.GetResource("Plugins.Payments.Worldpay.Fields.StoredCard.SelectCard");
                    model.StoredCards.Insert(0, new SelectListItem { Text = selectCardText, Value = Guid.Empty.ToString() });
                }
            }

            return View("~/Plugins/Payments.Worldpay/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}