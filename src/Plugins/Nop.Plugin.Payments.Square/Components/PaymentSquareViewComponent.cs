using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.Square.Models;
using Nop.Plugin.Payments.Square.Services;
using Nop.Services.Common;
using Nop.Services.Localization;

namespace Nop.Plugin.Payments.Square.Components
{
    /// <summary>
    /// Represents payment info view component
    /// </summary>
    [ViewComponent(Name = SquarePaymentDefaults.VIEW_COMPONENT_NAME)]
    public class PaymentSquareViewComponent : ViewComponent
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly SquarePaymentManager _squarePaymentManager;

        #endregion

        #region Ctor

        public PaymentSquareViewComponent(IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            SquarePaymentManager squarePaymentManager)
        {
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _workContext = workContext;
            _squarePaymentManager = squarePaymentManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var model = new PaymentInfoModel
            {
                //whether current customer is guest
                IsGuest = _workContext.CurrentCustomer.IsGuest(),

                //get postal code from the billing address or from the shipping one
                PostalCode = _workContext.CurrentCustomer.BillingAddress?.ZipPostalCode
                    ?? _workContext.CurrentCustomer.ShippingAddress?.ZipPostalCode
            };

            //whether customer already has stored cards
            var customerId = _genericAttributeService
                .GetAttribute<string>(_workContext.CurrentCustomer, SquarePaymentDefaults.CustomerIdAttribute) ?? string.Empty;
            var customer = _squarePaymentManager.GetCustomer(customerId);
            if (customer?.Cards != null)
            {
                var cardNumberMask = _localizationService.GetResource("Plugins.Payments.Square.Fields.StoredCard.Mask");
                model.StoredCards = customer.Cards
                    .Select(card => new SelectListItem { Text = string.Format(cardNumberMask, card.Last4), Value = card.Id })
                    .ToList();
            }

            //add the special item for 'select card' with empty GUID value
            if (model.StoredCards.Any())
            {
                var selectCardText = _localizationService.GetResource("Plugins.Payments.Square.Fields.StoredCard.SelectCard");
                model.StoredCards.Insert(0, new SelectListItem { Text = selectCardText, Value = Guid.Empty.ToString() });
            }

            return View("~/Plugins/Payments.Square/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}