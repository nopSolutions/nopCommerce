using System;
using System.Net.Mail;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Clickatell;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Messages.SMS
{
    /// <summary>
    /// Represents the Verizon SMS provider
    /// </summary>
    public class VerizonSMSProvider : ISMSProvider
    {
        #region Methods
        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">SMS text</param>
        /// <returns>Result</returns>
        public bool SendSMS(string text)
        {
            try
            {
                var emailAccount = MessageManager.DefaultEmailAccount;

                var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
                var to = new MailAddress(VerizonEmail);

                MessageManager.InsertQueuedEmail(5, from, to,
                    string.Empty, string.Empty, SettingManager.StoreName, text, 
                    DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogTypeEnum.Unknown, ex.Message, ex);
                return false;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Verizon email
        /// </summary>
        public static string VerizonEmail
        {
            get
            {
                return SettingManager.GetSettingValue("Mobile.SMS.Verizon.Email");
            }
            set
            {
                SettingManager.SetParam("Mobile.SMS.Verizon.Email", value);
            }
        }
        #endregion

    }
}
