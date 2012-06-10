using System.Collections.Generic;

namespace Nop.Services.Cms
{
    /// <summary>
    /// Widget service interface
    /// </summary>
    public partial interface IWidgetService
    {
        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <returns>Widgets</returns>
        IList<IWidgetPlugin> LoadActiveWidgets();

        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <returns>Widgets</returns>
        IList<IWidgetPlugin> LoadActiveWidgetsByWidgetZone(string widgetZone);

        /// <summary>
        /// Load widget by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found widget</returns>
        IWidgetPlugin LoadWidgetBySystemName(string systemName);

        /// <summary>
        /// Load all widgets
        /// </summary>
        /// <returns>Widgets</returns>
        IList<IWidgetPlugin> LoadAllWidgets();
    }
}
