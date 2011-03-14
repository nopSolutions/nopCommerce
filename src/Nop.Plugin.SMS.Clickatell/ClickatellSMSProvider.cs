using System;

using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Plugin.SMS.Clickatell.Clickatell;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Messages;
using System.ServiceModel;

namespace Nop.Plugin.SMS.Clickatell
{
    /// <summary>
    /// Represents the Clickatell SMS provider
    /// </summary>
    public class ClickatellSMSProvider : ISMSProvider
    {
        /// <summary>
        /// Gets the friendly name
        /// </summary>
        public string FriendlyName
        {
            get { return "Clickatell SMS Provider"; }
        }

        /// <summary>
        /// Gets the system name
        /// </summary>
        public string SystemName
        {
            get { return "Mobile.SMS.Clickatell"; }
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        public bool SendSMS(string text)
        {
            try
            {
                using (var svc = new PushServerWSPortTypeClient(new BasicHttpBinding(), new EndpointAddress("http://api.clickatell.com/soap/webservice_vs.php")))
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
                Logger.InsertLog(LogLevel.Error, ex.Message, ex);
            }
            return false;
        }

        /// <summary>
        /// Gets or sets the setting service
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the setting service
        /// </summary>
        public ISettingService SettingService { get; set; }

        /// <summary>
        /// Gets or sets the Clickatell phone number
        /// </summary>
        public string ClickatellPhoneNumber
        {
            get
            {
                return SettingService.GetSettingByKey<string>("Mobile.SMS.Clickatell.PhoneNumber");
            }
            set
            {
                SettingService.SetSetting<string>("Mobile.SMS.Clickatell.PhoneNumber", value);
            }
        }

        /// <summary>
        /// Gets or sets the Clickatell API ID
        /// </summary>
        public string ClickatellAPIId
        {
            get
            {
                return SettingService.GetSettingByKey<string>("Mobile.SMS.Clickatell.APIID");
            }
            set
            {
                SettingService.SetSetting<string>("Mobile.SMS.Clickatell.APIID", value);
            }
        }

        /// <summary>
        /// Gets or sets the Clickatell username
        /// </summary>
        public string ClickatellUsername
        {
            get
            {
                return SettingService.GetSettingByKey<string>("Mobile.SMS.Clickatell.Username");
            }
            set
            {
                SettingService.SetSetting<string>("Mobile.SMS.Clickatell.Username", value);
            }
        }

        /// <summary>
        /// Gets or sets the Clickatell password
        /// </summary>
        public string ClickatellPassword
        {
            get
            {
                return SettingService.GetSettingByKey<string>("Mobile.SMS.Clickatell.Password");
            }
            set
            {
                SettingService.SetSetting<string>("Mobile.SMS.Clickatell.Password", value);
            }
        }
    }
}
