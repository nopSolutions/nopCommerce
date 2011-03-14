using System;

using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
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
                var emailAccount = EmailAccountService.DefaultEmailAccount;

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

                QueuedEmailService.InsertQueuedEmail(queuedEmail);

                return true;
            }
            catch (Exception ex)
            {
                Logger.InsertLog(LogLevel.Error, ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the setting service
        /// </summary>
        public ISettingService SettingService { get; set; }

        /// <summary>
        /// Gets or sets the queued email service
        /// </summary>
        public IQueuedEmailService QueuedEmailService { get; set; }

        /// <summary>
        /// Gets or sets the email account service
        /// </summary>
        public IEmailAccountService EmailAccountService { get; set; }

        /// <summary>
        /// Gets or sets the setting service
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the Verizon email
        /// </summary>
        public string VerizonEmail
        {
            get
            {
                return SettingService.GetSettingByKey<string>("Mobile.SMS.Verizon.Email");
            }
            set
            {
                SettingService.SetSetting<string>("Mobile.SMS.Verizon.Email", value);
            }
        }

    }
}
