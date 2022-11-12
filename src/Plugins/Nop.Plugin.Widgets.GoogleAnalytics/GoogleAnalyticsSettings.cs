<<<<<<< HEAD
﻿using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.GoogleAnalytics
{
    public class GoogleAnalyticsSettings : ISettings
    {
        public string GoogleId { get; set; }
        public string TrackingScript { get; set; }
        public bool EnableEcommerce { get; set; }
        public bool UseJsToSendEcommerceInfo { get; set; }
        public bool IncludingTax { get; set; }
        public string WidgetZone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include customer identifier to script
        /// </summary>
        public bool IncludeCustomerId { get; set; }
    }
=======
﻿using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.GoogleAnalytics
{
    public class GoogleAnalyticsSettings : ISettings
    {
        public string GoogleId { get; set; }
        public string TrackingScript { get; set; }
        public bool EnableEcommerce { get; set; }
        public bool UseJsToSendEcommerceInfo { get; set; }
        public bool IncludingTax { get; set; }
        public string WidgetZone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include customer identifier to script
        /// </summary>
        public bool IncludeCustomerId { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}