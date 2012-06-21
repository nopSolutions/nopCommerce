using System;
using System.ServiceModel;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.SMS.Clickatell.Clickatell;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Plugin.SMS.Clickatell
{
    /// <summary>
    /// Represents the Clickatell SMS provider
    /// </summary>
    public class ClickatellSmsProvider : BasePlugin, IMiscPlugin
    {
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly ClickatellSettings _clickatellSettings;

        public ClickatellSmsProvider(ClickatellSettings clickatellSettings,
            ILogger logger, ISettingService settingService)
        {
            this._clickatellSettings = clickatellSettings;
            this._logger = logger;
            this._settingService = settingService;
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

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new ClickatellSettings()
            {
                Enabled = false,
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.TestFailed", "Test message sending failed");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.TestSuccess", "Test message was sent");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.Fields.Enabled", "Enabled");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.Fields.Enabled.Hint", "Check to enable SMS provider");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.Fields.ApiId", "API ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.Fields.ApiId.Hint", "Clickatell API ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.Fields.Password", "Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.Fields.Password.Hint", "Clickatell password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.Fields.PhoneNumber", "Phone number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.Fields.PhoneNumber.Hint", "Your phone number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.Fields.Username", "Username");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.Fields.Username.Hint", "Clickatell username");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.Fields.TestMessage", "Message text");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.Fields.TestMessage.Hint", "Text of the test message");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.SendTest", "Send");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Clickatell.SendTest.Hint", "Send test message");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ClickatellSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.TestFailed");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.TestSuccess");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.Fields.Enabled");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.Fields.Enabled.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.Fields.ApiId");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.Fields.ApiId.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.Fields.Password");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.Fields.Password.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.Fields.PhoneNumber");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.Fields.PhoneNumber.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.Fields.Username");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.Fields.Username.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.Fields.TestMessage");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.Fields.TestMessage.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.SendTest");
            this.DeletePluginLocaleResource("Plugins.Sms.Clickatell.SendTest.Hint");

            base.Uninstall();
        }
    }
}
