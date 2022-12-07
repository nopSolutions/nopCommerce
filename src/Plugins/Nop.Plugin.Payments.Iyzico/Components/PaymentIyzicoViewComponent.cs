using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Payments.Iyzico.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Iyzico.Components
{
    [ViewComponent(Name = "PaymentIyzico")]
    public class PaymentIyzicoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IyzicoPaymentSettings _settings;

        #endregion

        #region Ctor
        public PaymentIyzicoViewComponent(IyzicoPaymentSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Methods

        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel()
            {
                UseToPaymentPopup = _settings.UseToPaymentPopup
            };

            return View("~/Plugins/Payments.Iyzico/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}
