using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Stereo.Components;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Stereo
{
    public class StereoPlugin : BasePlugin , IPlugin , IWidgetPlugin
    {
        public override async Task InstallAsync()
        {
            base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            base.UninstallAsync();
        }

        public bool HideInWidgetList => false;


        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { 
                PublicWidgetZones.HeaderAfter
            });
        }
        Type IWidgetPlugin.GetWidgetViewComponent(string widgetZone)
        {
            return typeof(StereoViewComponent);
        }
    }
}
