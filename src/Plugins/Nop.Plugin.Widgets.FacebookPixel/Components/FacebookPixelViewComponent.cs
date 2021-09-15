using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Plugin.Widgets.FacebookPixel.Services;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.FacebookPixel.Components
{
    /// <summary>
    /// Represents Facebook Pixel view component
    /// </summary>
    [ViewComponent(Name = FacebookPixelDefaults.VIEW_COMPONENT)]
    public class FacebookPixelViewComponent : NopViewComponent
    {
        #region Fields

        private readonly FacebookPixelService _facebookPixelService;

        #endregion

        #region Ctor

        public FacebookPixelViewComponent(FacebookPixelService facebookPixelService)
        {
            _facebookPixelService = facebookPixelService;
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
            var script = widgetZone != PublicWidgetZones.HeadHtmlTag
                ? await _facebookPixelService.PrepareCustomEventsScriptAsync(widgetZone)
                : await _facebookPixelService.PrepareScriptAsync();

            return new HtmlContentViewComponentResult(new HtmlString(script ?? string.Empty));
        }

        #endregion
    }
}