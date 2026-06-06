using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Jotform.Components;

/// <summary>
/// Represents the view component to display Jotform AI agent chatbot button
/// </summary>
public class WidgetJotformViewComponent : NopViewComponent
{
    #region Fields

    private readonly JotformSettings _jotformSettings;

    #endregion

    #region Ctor

    public WidgetJotformViewComponent(JotformSettings jotformSettings)
    {
        _jotformSettings = jotformSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>The view component result</returns>
    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
        if (!_jotformSettings.Enabled || string.IsNullOrEmpty(_jotformSettings.EmbedCode))
            return Content(string.Empty);

        return new HtmlContentViewComponentResult(new HtmlString(_jotformSettings.EmbedCode));
    }

    #endregion
}