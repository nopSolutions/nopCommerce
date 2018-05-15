using System;
using Nop.Core.Domain.Cms;

namespace Nop.Services.Cms
{
    /// <summary>
    /// Widget extensions
    /// </summary>
    public static class WidgetExtensions
    {
        /// <summary>
        /// Is widget active
        /// </summary>
        /// <param name="widget">Widget</param>
        /// <param name="widgetSettings">Widget settings</param>
        /// <returns>Result</returns>
        public static bool IsWidgetActive(this IWidgetPlugin widget,
            WidgetSettings widgetSettings)
        {
            if (widget == null)
                throw new ArgumentNullException(nameof(widget));

            if (widgetSettings == null)
                throw new ArgumentNullException(nameof(widgetSettings));

            if (widgetSettings.ActiveWidgetSystemNames == null)
                return false;
            foreach (var activeMethodSystemName in widgetSettings.ActiveWidgetSystemNames)
                if (widget.PluginDescriptor.SystemName.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }
    }
}
