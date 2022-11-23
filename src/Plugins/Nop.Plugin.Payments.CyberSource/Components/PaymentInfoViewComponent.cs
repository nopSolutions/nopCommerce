using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.CyberSource.Domain;
using Nop.Plugin.Payments.CyberSource.Models;
using Nop.Plugin.Payments.CyberSource.Services;
using Nop.Services.Messages;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.CyberSource.Components
{
    /// <summary>
    /// Represents the view component to display payment info in public store
    /// </summary>
    public class PaymentInfoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly CustomerTokenService _customerTokenService;
        private readonly CyberSourceService _cyberSourceService;
        private readonly CyberSourceSettings _cyberSourceSettings;
        private readonly INotificationService _notificationService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;

        #endregion

        #region Ctor

        public PaymentInfoViewComponent(CustomerTokenService customerTokenService,
            CyberSourceService cyberSourceService,
            CyberSourceSettings cyberSourceSettings,
            INotificationService notificationService,
            IWorkContext workContext,
            OrderSettings orderSettings)
        {
            _customerTokenService = customerTokenService;
            _cyberSourceService = cyberSourceService;
            _cyberSourceSettings = cyberSourceSettings;
            _notificationService = notificationService;
            _workContext = workContext;
            _orderSettings = orderSettings;
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
            var model = new PaymentInfoModel();
            var customer = await _workContext.GetCurrentCustomerAsync();

            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            for (var i = 1; i <= 12; i++)
            {
                model.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            model.NewCard = true;
            model.IsFlexMicroFormEnabled = _cyberSourceSettings.PaymentConnectionMethod == ConnectionMethodType.FlexMicroForm;
            model.TokenizationEnabled = _cyberSourceSettings.TokenizationEnabled;

            //prepare customer tokens if exist
            if (model.TokenizationEnabled)
            {
                var existingTokens = await _customerTokenService.GetAllTokensAsync(customer.Id);
                model.ShowExistingTokenSection = existingTokens.Any();
                if (model.ShowExistingTokenSection)
                {
                    model.ExistingTokens = existingTokens.Select((token, index) => new SelectListItem
                    {
                        Text = (token.FirstSixDigitOfCard ?? "XXXXXX") + "XXXXXX" + (token.LastFourDigitOfCard ?? "XXXX"),
                        Value = token.Id.ToString(),
                        Selected = index == 0
                    }).ToList();
                    model.NewCard = false;
                }
            }

            if (model.IsFlexMicroFormEnabled)
            {
                var (generatedKey, error) = await _cyberSourceService.GenerateFlexMicroformPublicKeyAsync();
                if (!string.IsNullOrEmpty(error))
                {
                    model.Errors = error;
                    if (_orderSettings.OnePageCheckoutEnabled)
                        ModelState.AddModelError(string.Empty, error);
                    else
                        _notificationService.ErrorNotification(error);
                }

                model.CaptureContext = generatedKey?.KeyId ?? string.Empty;
            }

            return View("~/Plugins/Payments.CyberSource/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}