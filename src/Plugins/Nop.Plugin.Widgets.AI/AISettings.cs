using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.AI
{
    public class AISettings : ISettings
    {
        public string OverrideInstrumentationKey { get; set; }

        public bool Enabled { get; set; }

        public bool EnableDebug { get; set; }

        public bool DisableExceptionTracking { get; set; }

        public bool DisableFetchTracking { get; set; }

        public bool DisableAjaxTracking { get; set; }

        public int MaxAjaxCallsPerView { get; set; }

        public bool OverridePageViewDuration { get; set; }
    }
}
