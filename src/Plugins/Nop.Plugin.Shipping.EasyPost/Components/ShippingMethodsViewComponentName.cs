using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Newtonsoft.Json;
using Nop.Services.Messages;
using Nop.Services.Shipping;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Shipping.EasyPost.Components
{
    /// <summary>
    /// Represents view component to display address verification warning on the opc shipping methods page in the public store
    /// </summary>
    [ViewComponent(Name = EasyPostDefaults.SHIPPING_METHODS_VIEW_COMPONENT_NAME)]
    public class ShippingMethodsViewComponentName : NopViewComponent
    {
        #region Fields

        private readonly EasyPostSettings _easyPostSettings;
        private readonly IShippingPluginManager _shippingPluginManager;

        #endregion

        #region Ctor

        public ShippingMethodsViewComponentName(EasyPostSettings easyPostSettings,
            IShippingPluginManager shippingPluginManager)
        {
            _easyPostSettings = easyPostSettings;
            _shippingPluginManager = shippingPluginManager;
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
            if (!await _shippingPluginManager.IsPluginActiveAsync(EasyPostDefaults.SystemName))
                return Content(string.Empty);

            if (!widgetZone.Equals(PublicWidgetZones.OpCheckoutShippingMethodTop))
                return Content(string.Empty);

            if (!_easyPostSettings.AddressVerification)
                return Content(string.Empty);

            if (!TempData.ContainsKey(NopMessageDefaults.NotificationListKey))
                return Content(string.Empty);

            var warnings = JsonConvert.DeserializeObject<IList<NotifyData>>(TempData[NopMessageDefaults.NotificationListKey].ToString())
                .Where(note => note.Type == NotifyType.Warning)
                .Select(note => note.Message)
                .ToList();
            if (!warnings.Any())
                return Content(string.Empty);

            var script = @$"
                <script>
                    $(document).ready(function () {{
                        alert('{string.Join(". ", warnings).TrimEnd(' ')}');
                    }});
                </script>
            ";

            return new HtmlContentViewComponentResult(new HtmlString(script));
        }

        #endregion
    }
}