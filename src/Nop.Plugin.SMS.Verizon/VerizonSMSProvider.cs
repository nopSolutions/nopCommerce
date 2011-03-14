using System;
using System.Security.Principal;
using Nop.Core.Domain.Logging;
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
    public class VerizonSMSProvider : ISMSProvider
    {
        private readonly ISettingService _settingService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILogger _logger;

        public VerizonSMSProvider(ISettingService settingService,
            IQueuedEmailService queuedEmailService,
            IEmailAccountService emailAccountService,
            ILogger logger)
        {
            this._settingService = settingService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._logger = logger;
        }

        /// <summary>
        /// Gets the friendly name
        /// </summary>
        public string FriendlyName
        {
            get { return "Verizon SMS Provider"; }
        }

        /// <summary>
        /// Gets the system name
        /// </summary>
        public string SystemName
        {
            get { return "Mobile.SMS.Verizon"; }
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
                    //TODO: implement getting store name
                    //Subject = SettingService.GetSettingByKey<string>("Common.StoreNamw"),
                    Body = text,
                    CreatedOn = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id
                };

                _queuedEmailService.InsertQueuedEmail(queuedEmail);

                return true;
            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, ex.Message, ex);
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

        #region IPlugin Members

        public string Name
        {
            get { return FriendlyName; }
        }

        public int SortOrder
        {
            get { return 1; }
        }

        public bool IsAuthorized(IPrincipal user)
        {
            return true;
        }

        public int CompareTo(IPlugin other)
        {
            return SortOrder - other.SortOrder;
        }
        #endregion
    }
}
