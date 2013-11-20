using System;
using System.Linq;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;

namespace Nop.Plugin.SMS.Verizon
{
    /// <summary>
    /// Represents the Verizon SMS provider
    /// </summary>
    public class VerizonSmsProvider : BasePlugin, IMiscPlugin
    {
        private readonly VerizonSettings _verizonSettings;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly EmailAccountSettings _emailAccountSettings;

        public VerizonSmsProvider(VerizonSettings verizonSettings,
            IQueuedEmailService queuedEmailService, 
            IEmailAccountService emailAccountService,
            ILogger logger,
            ISettingService settingService,
            IStoreContext storeContext,
            EmailAccountSettings emailAccountSettings)
        {
            this._verizonSettings = verizonSettings;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._logger = logger;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._emailAccountSettings = emailAccountSettings;
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">SMS text</param>
        /// <returns>Result</returns>
        public bool SendSms(string text)
        {
            try
            {
                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                if (emailAccount == null)
                    throw new Exception("No email account could be loaded");

                var queuedEmail = new QueuedEmail()
                {
                    Priority = 5,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = _verizonSettings.Email,
                    ToName = string.Empty,
                    Subject = _storeContext.CurrentStore.Name,
                    Body = text,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id
                };

                _queuedEmailService.InsertQueuedEmail(queuedEmail);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return false;
            }
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
            controllerName = "SmsVerizon";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.SMS.Verizon.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new VerizonSettings()
            {
                Email = "yournumber@vtext.com",
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Verizon.TestFailed", "Test message sending failed");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Verizon.TestSuccess", "Test message was sent (queued)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Verizon.Fields.Enabled", "Enabled");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Verizon.Fields.Enabled.Hint", "Check to enable SMS provider");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Verizon.Fields.Email", "Email");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Verizon.Fields.Email.Hint", "Verizon email address(e.g. your_phone_number@vtext.com)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Verizon.Fields.TestMessage", "Message text");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Verizon.Fields.TestMessage.Hint", "Text of the test message");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Verizon.SendTest", "Send");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Verizon.SendTest.Hint", "Send test message");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<VerizonSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Sms.Verizon.TestFailed");
            this.DeletePluginLocaleResource("Plugins.Sms.Verizon.TestSuccess");
            this.DeletePluginLocaleResource("Plugins.Sms.Verizon.Fields.Enabled");
            this.DeletePluginLocaleResource("Plugins.Sms.Verizon.Fields.Enabled.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Verizon.Fields.Email");
            this.DeletePluginLocaleResource("Plugins.Sms.Verizon.Fields.Email.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Verizon.Fields.TestMessage");
            this.DeletePluginLocaleResource("Plugins.Sms.Verizon.Fields.TestMessage.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Verizon.SendTest");
            this.DeletePluginLocaleResource("Plugins.Sms.Verizon.SendTest.Hint");

            base.Uninstall();
        }
    }
}
