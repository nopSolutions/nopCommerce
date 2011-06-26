using System;
using System.ServiceModel;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.SMS.Clickatell.Clickatell;
using Nop.Services.Logging;
using Nop.Services.Messages;

namespace Nop.Plugin.SMS.Clickatell
{
    /// <summary>
    /// Represents the Clickatell SMS provider
    /// </summary>
    public class ClickatellSmsProvider : BasePlugin, ISmsProvider
    {
        private readonly ILogger _logger;
        private readonly ClickatellSettings _clickatellSettings;

        public ClickatellSmsProvider(ClickatellSettings clickatellSettings,
            ILogger logger)
        {
            this._clickatellSettings = clickatellSettings;
            this._logger = logger;
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        public bool SendSms(string text)
        {
            try
            {
                using (var svc = new PushServerWSPortTypeClient(new BasicHttpBinding(), new EndpointAddress("http://api.clickatell.com/soap/webservice_vs.php")))
                {
                    string authRsp = svc.auth(Int32.Parse(_clickatellSettings.ApiId), _clickatellSettings.Username, _clickatellSettings.Password);

                    if (!authRsp.ToUpperInvariant().StartsWith("OK"))
                    {
                        throw new NopException(authRsp);
                    }

                    string ssid = authRsp.Substring(4);
                    string[] sndRsp = svc.sendmsg(ssid,
                        Int32.Parse(_clickatellSettings.ApiId), _clickatellSettings.Username,
                        _clickatellSettings.Password, new string[1] { _clickatellSettings.PhoneNumber },
                        String.Empty, text, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                        String.Empty, 0, String.Empty, String.Empty, String.Empty, 0);

                    if (!sndRsp[0].ToUpperInvariant().StartsWith("ID"))
                    {
                        throw new NopException(sndRsp[0]);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return false;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "SmsClickatell";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.SMS.Clickatell.Controllers" }, { "area", null } };
        }
    }
}
