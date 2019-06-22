using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.AI
{
    public class AIPlugin : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsAI";
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.HeadHtmlTag };
        }

        public override string GetConfigurationPageUrl()
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            return webHelper.GetStoreLocation() + "Admin/WidgetsAI/Configure";
        }

        public override void Install()
        {
            var settings = new AISettings
            {
                Enabled = false
            };

            var settingSvc = EngineContext.Current.Resolve<ISettingService>();
            settingSvc.SaveSetting(settings);

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AI.OverrideInstrumentationKey", "Instrumentation Key");
            localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AI.Enabled", "Enabled");
            localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AI.EnableDebug", "Debug enabled");
            localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AI.DisableExceptionTracking", "Disable exception tracking");
            localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AI.DisableFetchTracking", "Disable fetch tracking");
            localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AI.DisableAjaxTracking", "Disable ajax tracking");
            localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AI.MaxAjaxCallsPerView", "Max ajax call per view");
            localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AI.OverridePageViewDuration", "Override page view duration");
            localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.AI.Instructions", "<p>Application Insigths is the instrumentation used by the sys admin. Do not disable this feature unless you're the sys admin.</p>");

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            var settingSvc = EngineContext.Current.Resolve<ISettingService>();
            settingSvc.DeleteSetting<AISettings>();

            //locales
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            localizationService.DeletePluginLocaleResource("Plugins.Widgets.AI.OverrideInstrumentationKey");
            localizationService.DeletePluginLocaleResource("Plugins.Widgets.AI.Enabled");
            localizationService.DeletePluginLocaleResource("Plugins.Widgets.AI.EnableDebug");
            localizationService.DeletePluginLocaleResource("Plugins.Widgets.AI.DisableExceptionTracking");
            localizationService.DeletePluginLocaleResource("Plugins.Widgets.AI.DisableFetchTracking");
            localizationService.DeletePluginLocaleResource("Plugins.Widgets.AI.DisableAjaxTracking");
            localizationService.DeletePluginLocaleResource("Plugins.Widgets.AI.MaxAjaxCallsPerView");
            localizationService.DeletePluginLocaleResource("Plugins.Widgets.AI.OverridePageViewDuration");
            localizationService.DeletePluginLocaleResource("Plugins.Widgets.AI.Instructions");

            base.Uninstall();
        }
    }
}
