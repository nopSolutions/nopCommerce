using Nop.Plugin.UserDeatils.Components;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.UserDeatils
{
        public class UserDeatilsPlugin : BasePlugin, IPlugin, IWidgetPlugin
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

            public Type GetWidgetViewComponent(string widgetZone)
            {
                    return typeof(UserdetailsViewComponent);

            }

            public Task<IList<string>> GetWidgetZonesAsync()
            {
                return Task.FromResult<IList<string>>(new List<string> {

                PublicWidgetZones.HeaderAfter,
              
            });
            }
        }
    }