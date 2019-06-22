using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.AI.Configurations;
using Nop.Plugin.Widgets.AI.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.AI.Components
{
    [ViewComponent(Name = "WidgetsAI")]
    public class AIViewComponent : NopViewComponent
    {
        private readonly ILogger<AIViewComponent> _logger;
        private readonly AISettings _setting;
        private readonly AIConfiguration _aiConfiguration;
        private readonly ICacheManager _cacheManager;

        private readonly JsonSerializerSettings _jSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public AIViewComponent(ILogger<AIViewComponent> logger, AISettings setting, IOptions<AIConfiguration> aiConfiguration, ICacheManager cacheManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _setting = setting ?? throw new ArgumentNullException(nameof(setting));
            _aiConfiguration = aiConfiguration.Value ?? throw new ArgumentNullException(nameof(aiConfiguration));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var content = "{}";
            try
            {
                if (_setting.Enabled)
                {

                    var settingCached = _cacheManager.Get<string>($"AIViewComponent_{Request.Host.Host}", () =>
                    {
                        string instrumentationKey = string.IsNullOrEmpty(_setting.OverrideInstrumentationKey) ? _aiConfiguration.InstrumentationKey : _setting.OverrideInstrumentationKey;

                        var jsAiSetting = new JsonAiSettings();
                        jsAiSetting.InstrumentationKey = instrumentationKey;
                        jsAiSetting.EnableDebug = _setting.EnableDebug;
                        jsAiSetting.DisableExceptionTracking = _setting.DisableExceptionTracking;
                        jsAiSetting.DisableFetchTracking = _setting.DisableFetchTracking;
                        jsAiSetting.DisableAjaxTracking = _setting.DisableAjaxTracking;
                        jsAiSetting.OverridePageViewDuration = _setting.OverridePageViewDuration;

                        if (_setting.MaxAjaxCallsPerView >= 0)
                            jsAiSetting.MaxAjaxCallsPerView = _setting.MaxAjaxCallsPerView;

                        return JsonConvert.SerializeObject(jsAiSetting, _jSettings);
                    }, null);
                    content = settingCached;
                }
                else
                {
                    return new HtmlContentViewComponentResult(new HtmlString(""));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when executing AI Widget plugin {WidgetZone}", widgetZone);
            }
            return View("~/Plugins/Widgets.AI/Views/AIViewComponent.cshtml", content);
        }
    }
}
