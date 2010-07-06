using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;
using System;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Clickatell;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.BusinessLogic.Messages
{
    /// <summary>
    /// Represents the SMS manager
    /// </summary>
    public class SMSManager
    {
        #region Methods
        /// <summary>
        /// Sends SMS message
        /// </summary>
        /// <param name="text">The text of message</param>
        /// <returns>true if message was sent successfully; otherwise false.</returns>
        public static bool Send(string text)
        {
            return Send(text, PhoneNumber);
        }
        
        /// <summary>
        /// Sends SMS message
        /// </summary>
        /// <param name="text">The text of message</param>
        /// <param name="phone">The phone number</param>
        /// <returns>true if message was sent successfully; otherwise false.</returns>
        public static bool Send(string text, string phone)
        {
            try
            {
                using(var svc = new PushServerWS())
                {
                    string authRsp = svc.auth(Int32.Parse(ClickatellAPIId), 
                        ClickatellUsername, ClickatellPassword);
                    if(!authRsp.ToUpperInvariant().StartsWith("OK"))
                    {
                        throw new NopException(authRsp);
                    }
                    string ssid = authRsp.Substring(4);
                    string[] sndRsp = svc.sendmsg(ssid, 
                        Int32.Parse(ClickatellAPIId), ClickatellUsername, 
                        ClickatellPassword, new string[1] { phone }, 
                        String.Empty, text, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                        String.Empty, 0, String.Empty, String.Empty, String.Empty, 0);

                    if (!sndRsp[0].ToUpperInvariant().StartsWith("ID"))
                    {
                        throw new NopException(sndRsp[0]);
                    }
                    return true;
                }
            }
            catch(Exception ex)
            {
                LogManager.InsertLog(LogTypeEnum.Unknown, ex.Message, ex);
            }
            return false;
        }

        /// <summary>
        /// Sends SMS notification about placed order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>true if message was sent successfully; otherwise false.</returns>
        public static bool SendOrderPlacedNotification(Order order)
        {
            if(order == null)
                return false;

            return Send(String.Format("New order(#{0}) has been placed.", order.OrderId));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the SMS notifications is enabled
        /// </summary>
        public static bool IsSMSAlertsEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Mobile.SMS.IsEnabled", false);
            }
            set
            {
                SettingManager.SetParam("Mobile.SMS.IsEnabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the admin phone number
        /// </summary>
        public static string PhoneNumber
        {
            get
            {
                return SettingManager.GetSettingValue("Mobile.SMS.AdminPhoneNumber");
            }
            set
            {
                SettingManager.SetParam("Mobile.SMS.AdminPhoneNumber", value);
            }
        }

        /// <summary>
        /// Gets or sets the Clickatell API ID
        /// </summary>
        public static string ClickatellAPIId
        {
            get
            {
                return SettingManager.GetSettingValue("Mobile.SMS.Clickatell.APIID");
            }
            set
            {
                SettingManager.SetParam("Mobile.SMS.Clickatell.APIID", value);
            }
        }

        /// <summary>
        /// Gets or sets the Clickatell username
        /// </summary>
        public static string ClickatellUsername
        {
            get
            {
                return SettingManager.GetSettingValue("Mobile.SMS.Clickatell.Username");
            }
            set
            {
                SettingManager.SetParam("Mobile.SMS.Clickatell.Username", value);
            }
        }

        /// <summary>
        /// Gets or sets the Clickatell password
        /// </summary>
        public static string ClickatellPassword
        {
            get
            {
                return SettingManager.GetSettingValue("Mobile.SMS.Clickatell.Password");
            }
            set
            {
                SettingManager.SetParam("Mobile.SMS.Clickatell.Password", value);
            }
        }
        #endregion

    }
}
