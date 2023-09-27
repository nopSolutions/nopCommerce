using Microsoft.AspNetCore.Mvc;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.What3words.Components
{
    public class What3wordsOrderAdminViewComponent : NopViewComponent
    {
        #region Fields

        protected readonly IAddressService _addressService;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IWidgetPluginManager _widgetPluginManager;
        protected readonly What3wordsSettings _what3WordsSettings;

        #endregion

        #region Ctor

        public What3wordsOrderAdminViewComponent(IAddressService addressService,
            IGenericAttributeService genericAttributeService,
            IWidgetPluginManager widgetPluginManager,
            What3wordsSettings what3WordsSettings)
        {
            _addressService = addressService;
            _genericAttributeService = genericAttributeService;
            _widgetPluginManager = widgetPluginManager;
            _what3WordsSettings = what3WordsSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the widget view component
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="additionalData">Additional parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            //ensure that what3words widget is active and enabled
            if (!await _widgetPluginManager.IsPluginActiveAsync(What3wordsDefaults.SystemName))
                return Content(string.Empty);

            if (!_what3WordsSettings.Enabled)
                return Content(string.Empty);

            if (additionalData is not OrderModel model)
                return Content(string.Empty);

            var addressId = 0;
            if (widgetZone.Equals(AdminWidgetZones.OrderBillingAddressDetailsBottom))
                addressId = model.BillingAddress.Id;
            if (widgetZone.Equals(AdminWidgetZones.OrderShippingAddressDetailsBottom))
                addressId = model.ShippingAddress.Id;
            var address = await _addressService.GetAddressByIdAsync(addressId);
            var addressValue = address is not null
                ? await _genericAttributeService.GetAttributeAsync<string>(address, What3wordsDefaults.AddressValueAttribute)
                : null;
            if (string.IsNullOrEmpty(addressValue))
                return Content(string.Empty);

            return View("~/Plugins/Widgets.What3words/Views/AdminOrderAddress.cshtml", addressValue);
        }

        #endregion
    }
}
