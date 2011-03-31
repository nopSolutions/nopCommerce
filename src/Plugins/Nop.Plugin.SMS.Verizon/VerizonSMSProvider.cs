using System;
using System.Security.Principal;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Core.Domain;

namespace Nop.Plugin.SMS.Verizon
{
    /// <summary>
    /// Represents the Verizon SMS provider
    /// </summary>
    public class VerizonSMSProvider : BasePlugin, ISMSProvider
    {
        private readonly ISettingService _settingService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILogger _logger;
        private readonly StoreInformationSettings _storeSettings;

        public VerizonSMSProvider(ISettingService settingService,
            IQueuedEmailService queuedEmailService, IEmailAccountService emailAccountService,
            ILogger logger, StoreInformationSettings storeSettigs)
        {
            this._settingService = settingService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._logger = logger;

            this._storeSettings = storeSettigs;
        }

        /// <summary>
        /// Gets the friendly name
        /// </summary>
        public override string FriendlyName
        {
            get { return "Verizon SMS Provider"; }
        }

        /// <summary>
        /// Gets the system name
        /// </summary>
        public override string SystemName
        {
            get { return "Mobile.SMS.Verizon"; }
        }

        /// <summary>
        /// Gets the author
        /// </summary>
        public override string Author
        {
            get { return "nopCommerce team"; }
        }

        /// <summary>
        /// Gets the version
        /// </summary>
        public override string Version
        {
            get { return "1.00"; }
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">SMS text</param>
        /// <returns>Result</returns>
        public bool SendSMS(string text)
        {
            try
            {
                var emailAccount = _emailAccountService.DefaultEmailAccount;

                var queuedEmail = new QueuedEmail()
                {
                    Priority = 5,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = VerizonEmail,
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
        /// Gets or sets the Verizon email
        /// </summary>
        public string VerizonEmail
        {
            get
            {
                return _settingService.GetSettingByKey<string>("Mobile.SMS.Verizon.Email");
            }
            set
            {
                _settingService.SetSetting<string>("Mobile.SMS.Verizon.Email", value);
            }
        }
    }
}
