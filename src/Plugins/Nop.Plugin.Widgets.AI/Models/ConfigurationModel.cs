using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.AI.Models
{
    public class ConfigurationModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AI.OverrideInstrumentationKey")]
        public string OverrideInstrumentationKey { get; set; }
        public bool OverrideInstrumentationKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AI.Enabled")]
        public bool Enabled { get; set; }
        public bool Enabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AI.EnableDebug")]
        public bool EnableDebug { get; set; }
        public bool EnableDebug_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AI.DisableExceptionTracking")]
        public bool DisableExceptionTracking { get; set; }
        public bool DisableExceptionTracking_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AI.DisableFetchTracking")]
        public bool DisableFetchTracking { get; set; }
        public bool DisableFetchTracking_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AI.DisableAjaxTracking")]
        public bool DisableAjaxTracking { get; set; }
        public bool DisableAjaxTracking_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AI.MaxAjaxCallsPerView")]
        public int MaxAjaxCallsPerView { get; set; }
        public bool MaxAjaxCallsPerView_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.AI.OverridePageViewDuration")]
        public bool OverridePageViewDuration { get; set; }
        public bool OverridePageViewDuration_OverrideForStore { get; set; }
    }
}
