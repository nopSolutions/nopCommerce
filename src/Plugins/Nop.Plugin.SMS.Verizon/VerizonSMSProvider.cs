using System;
using System.Linq;
using System.Web.Routing;
using Nop.Core.Domain;
using Nop.Core.Domain.Messages;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Messages;

namespace Nop.Plugin.SMS.Verizon
{
    /// <summary>
    /// Represents the Verizon SMS provider
    /// </summary>
    public class VerizonSmsProvider : BasePlugin, ISmsProvider
    {
        private readonly VerizonSettings _verizonSettings;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly StoreInformationSettings _storeSettings;
        private readonly EmailAccountSettings _emailAccountSettings;

        public VerizonSmsProvider(VerizonSettings verizonSettings,
            IQueuedEmailService queuedEmailService, IEmailAccountService emailAccountService,
            ILogger logger, ISettingService settingService, StoreInformationSettings storeSettigs,
            EmailAccountSettings emailAccountSettings)
        {
            this._verizonSettings = verizonSettings;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._logger = logger;
            this._settingService = settingService;

            this._storeSettings = storeSettigs;
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

                var queuedEmail = new QueuedEmail()
                {
                    Priority = 5,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = _verizonSettings.Email,
                    ToName = string.Empty,
                    Subject = _storeSettings.StoreName,
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
            var settings = new VerizonSettings()
            {
                Email = "yournumber@vtext.com",
            };
            _settingService.SaveSetting(settings);

            base.Install();
        }

    }
}
