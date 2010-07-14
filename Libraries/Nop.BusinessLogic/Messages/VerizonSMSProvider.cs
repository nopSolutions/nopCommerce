using System;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Clickatell;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common;
using System.Net.Mail;

namespace NopSolutions.NopCommerce.BusinessLogic.Messages
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
        /// <param name="text">Text</param>
        public bool SendSMS(string text)
        {
            try
            {
                var from = new MailAddress(MessageManager.AdminEmailAddress, MessageManager.AdminEmailDisplayName);
                var to = new MailAddress(VerizonEmail);

                MessageManager.SendEmail(String.Empty, text, from, to);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.InsertLog(LogTypeEnum.Unknown, ex.Message, ex);
            }
            return false;
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
