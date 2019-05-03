using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.Qualpay.Factories;
using Nop.Services.Customers;
using Nop.Services.Payments;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Payments.Qualpay.Components
{
    /// <summary>
    /// Represents Qualpay Customer Vault block view component
    /// </summary>
    [ViewComponent(Name = QualpayDefaults.CUSTOMER_VIEW_COMPONENT_NAME)]
    public class QualpayCustomerViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly QualpayCustomerModelFactory _qualpayCustomerModelFactory;
        private readonly QualpaySettings _qualpaySettings;

        #endregion

        #region Ctor

        public QualpayCustomerViewComponent(ICustomerService customerService,
            IPaymentPluginManager paymentPluginManager,
            QualpayCustomerModelFactory qualpayCustomerModelFactory,
            QualpaySettings qualpaySettings)
        {
            _customerService = customerService;
            _paymentPluginManager = paymentPluginManager;
            _qualpayCustomerModelFactory = qualpayCustomerModelFactory;
            _qualpaySettings = qualpaySettings;
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
            if (!widgetZone?.Equals(AdminWidgetZones.CustomerDetailsBlock, StringComparison.InvariantCultureIgnoreCase) ?? true)
                return Content(string.Empty);

            //check whether the payment plugin is active
            if (!_paymentPluginManager.IsPluginActive(QualpayDefaults.SystemName))
                return Content(string.Empty);

            //whether Qualpay Customer Vault feature is enabled
            if (!_qualpaySettings.UseCustomerVault)
                return Content(string.Empty);

            //get the view model
            if (!(additionalData is CustomerModel customerModel))
                return Content(string.Empty);

            //check whether a customer exists and isn't guest
            var customer = _customerService.GetCustomerById(customerModel.Id);
            if (customer?.IsGuest() ?? true)
                return Content(string.Empty);

            var model = _qualpayCustomerModelFactory.PrepareQualpayCustomerModel(customerModel, customer);

            return View("~/Plugins/Payments.Qualpay/Views/Customer/_CreateOrUpdate.Qualpay.cshtml", model);
        }

        #endregion
    }
}