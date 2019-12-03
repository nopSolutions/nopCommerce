//using Nop.Plugin.Api.WebHooks;

//namespace Nop.Plugin.Api.Services
//{
//    using Microsoft.AspNet.WebHooks;
//    using Microsoft.AspNet.WebHooks.Diagnostics;
//    using Microsoft.AspNet.WebHooks.Services;
//    using Nop.Plugin.Api.Domain;
//    using Nop.Plugin.Api.Helpers;
//    using System;
//    using System.Collections.Generic;
//    using System.Web.Http.Tracing;

//    public class WebHookService : IWebHookService
//    {
//        private IWebHookManager _webHookManager;
//        private IWebHookSender _webHookSender;
//        private IWebHookStore _webHookStore;
//        private IWebHookFilterManager _webHookFilterManager;
//        private ILogger _logger;
                
//        private readonly IConfigManagerHelper _configManagerHelper;

//        public WebHookService(IConfigManagerHelper configManagerHelper,ILogger logger)
//        {
//            _configManagerHelper = configManagerHelper;
//            _logger = logger;
//        }

//        public IWebHookFilterManager GetWebHookFilterManager()
//        {
//            if (_webHookFilterManager == null)
//            {
//                var filterProviders = new List<IWebHookFilterProvider>();
//                filterProviders.Add(new FilterProvider());
//                _webHookFilterManager = new WebHookFilterManager(filterProviders);
//            }

//            return _webHookFilterManager;
//        }

//        public IWebHookManager GetWebHookManager()
//        {
//            if (_webHookManager == null)
//            {
//                _webHookManager = new WebHookManager(GetWebHookStore(), GetWebHookSender(), _logger);
//            }

//            return _webHookManager;
//        }

//        public IWebHookSender GetWebHookSender()
//        {
//            if (_webHookSender == null)
//            {
//                _webHookSender = new ApiWebHookSender(_logger);
//            }

//            return _webHookSender;
//        }

//        public IWebHookStore GetWebHookStore()
//        {
//            if (_webHookStore == null)
//            {
//                var dataSettings = _configManagerHelper.DataSettings;
//                Microsoft.AspNet.WebHooks.Config.SettingsDictionary settings = new Microsoft.AspNet.WebHooks.Config.SettingsDictionary();
//                settings.Add("MS_SqlStoreConnectionString", dataSettings.DataConnectionString);
//                settings.Connections.Add("MS_SqlStoreConnectionString", new Microsoft.AspNet.WebHooks.Config.ConnectionSettings("MS_SqlStoreConnectionString", dataSettings.DataConnectionString));

//                Microsoft.AspNet.WebHooks.IWebHookStore store = new Microsoft.AspNet.WebHooks.SqlWebHookStore(settings, _logger);

//                Microsoft.AspNet.WebHooks.Services.CustomServices.SetStore(store);

//                _webHookStore = CustomServices.GetStore();
//            }

//            return _webHookStore;
//        }        
//    }
//}
