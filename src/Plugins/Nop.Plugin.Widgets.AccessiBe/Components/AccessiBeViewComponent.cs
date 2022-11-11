using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Plugin.Widgets.AccessiBe.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.AccessiBe.Components
{
    /// <summary>
    /// Represents the view component to place a widget into pages
    /// </summary>
    public class AccessiBeViewComponent : NopViewComponent
    {
        #region Fields

        private readonly AccessiBeService _accessiBeService;
        private readonly AccessiBeSettings _accessiBeSettings;

        #endregion

        #region Ctor

        public AccessiBeViewComponent(AccessiBeService accessiBeService,
            AccessiBeSettings accessiBeSettings)
        {
            _accessiBeService = accessiBeService;
            _accessiBeSettings = accessiBeSettings;
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
            var script = widgetZone != _accessiBeSettings.WidgetZone
                ? string.Empty
                : await _accessiBeService.PrepareScriptAsync();

            return new HtmlContentViewComponentResult(new HtmlString(script ?? string.Empty));
        }

        #endregion
    }
}