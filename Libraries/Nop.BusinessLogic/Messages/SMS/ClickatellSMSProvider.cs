using System;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Clickatell;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Messages.SMS
{
    /// <summary>
    /// Represents the Clickatell SMS provider
    /// </summary>
    public class ClickatellSMSProvider : ISMSProvider
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
                using (var svc = new PushServerWS())
                {
                    string authRsp = svc.auth(Int32.Parse(ClickatellAPIId), ClickatellUsername, ClickatellPassword);

                    if (!authRsp.ToUpperInvariant().StartsWith("OK"))
                    {
                        throw new NopException(authRsp);
                    }

                    string ssid = authRsp.Substring(4);
                    string[] sndRsp = svc.sendmsg(ssid,
                        Int32.Parse(ClickatellAPIId), ClickatellUsername,
                        ClickatellPassword, new string[1] { ClickatellPhoneNumber },
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
                IoCFactory.Resolve<ILogManager>().InsertLog(LogTypeEnum.Unknown, ex.Message, ex);
            }
            return false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Clickatell phone number
        /// </summary>
        public static string ClickatellPhoneNumber
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("Mobile.SMS.Clickatell.PhoneNumber");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("Mobile.SMS.Clickatell.PhoneNumber", value);
            }
        }

        /// <summary>
        /// Gets or sets the Clickatell API ID
        /// </summary>
        public static string ClickatellAPIId
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("Mobile.SMS.Clickatell.APIID");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("Mobile.SMS.Clickatell.APIID", value);
            }
        }

        /// <summary>
        /// Gets or sets the Clickatell username
        /// </summary>
        public static string ClickatellUsername
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("Mobile.SMS.Clickatell.Username");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("Mobile.SMS.Clickatell.Username", value);
            }
        }

        /// <summary>
        /// Gets or sets the Clickatell password
        /// </summary>
        public static string ClickatellPassword
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("Mobile.SMS.Clickatell.Password");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("Mobile.SMS.Clickatell.Password", value);
            }
        } 
        #endregion

    }
}
