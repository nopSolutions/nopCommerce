using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.What3words.Models;
using Nop.Services.Cms;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.What3words.Components
{
    [ViewComponent(Name = What3wordsDefaults.VIEW_COMPONENT_NAME)]
    public class What3wordsViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;
        private readonly What3wordsSettings _what3WordsSettings;

        #endregion

        #region Ctor

        public What3wordsViewComponent(ILogger logger,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext,
            What3wordsSettings what3WordsSettings)
        {
            _logger = logger;
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
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
            if (!widgetZone.Equals(PublicWidgetZones.AddressBottom))
                return Content(string.Empty);

            //ensure that what3words widget is active and enabled
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _widgetPluginManager.IsPluginActiveAsync(What3wordsDefaults.SystemName, customer))
                return Content(string.Empty);

            if (!_what3WordsSettings.Enabled)
                return Content(string.Empty);

            if (string.IsNullOrEmpty(_what3WordsSettings.ApiKey))
            {
                await _logger.ErrorAsync("what3words error: API key is not set", customer: customer);
                return Content(string.Empty);
            }

            //display this on the checkout pages only
            if (ViewData.TemplateInfo.HtmlFieldPrefix != What3wordsDefaults.BillingAddressPrefix &&
                ViewData.TemplateInfo.HtmlFieldPrefix != What3wordsDefaults.ShippingAddressPrefix)
            {
                return Content(string.Empty);
            }

            var model = new What3wordsAddressModel
            {
                ApiKey = _what3WordsSettings.ApiKey,
                Prefix = ViewData.TemplateInfo.HtmlFieldPrefix
            };

            return View("~/Plugins/Widgets.What3words/Views/PublicInfo.cshtml", model);

        }

        #endregion
    }
}