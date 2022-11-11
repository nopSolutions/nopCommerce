using System.Collections.Generic;

namespace Nop.Plugin.Widgets.FacebookPixel.Domain
{
    /// <summary>
    /// Represents custom event configuration
    /// </summary>
    public class CustomEvent
    {
        #region Ctor

        public CustomEvent()
        {
            WidgetZones = new List<string>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the custom event name
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the list of widget zones in which this event is tracked
        /// </summary>
        public IList<string> WidgetZones { get; set; }

        #endregion
    }
}