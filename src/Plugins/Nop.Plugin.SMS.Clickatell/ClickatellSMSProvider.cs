using System;
using System.Security.Principal;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Core.Plugins;
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
    public class ClickatellSMSProvider : BasePlugin, ISMSProvider
    {
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;

        public ClickatellSMSProvider(ISettingService settingService,
            ILogger logger)
        {
            this._settingService = settingService;
            this._logger = logger;
        }

        /// <summary>
        /// Gets the friendly name
        /// </summary>
        public override string FriendlyName
        {
            get { return "Clickatell SMS Provider"; }
        }

        /// <summary>
        /// Gets the system name
        /// </summary>
        public override string SystemName
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
                _logger.InsertLog(LogLevel.Error, ex.Message, ex);
            }
            return false;
        }

        /// <summary>
        /// Gets or sets the Clickatell phone number
        /// </summary>
        public string ClickatellPhoneNumber
        {
            get
            {
                return _settingService.GetSettingByKey<string>("Mobile.SMS.Clickatell.PhoneNumber");
            }
            set
            {
                _settingService.SetSetting<string>("Mobile.SMS.Clickatell.PhoneNumber", value);
            }
        }

        /// <summary>
        /// Gets or sets the Clickatell API ID
        /// </summary>
        public string ClickatellAPIId
        {
            get
            {
                return _settingService.GetSettingByKey<string>("Mobile.SMS.Clickatell.APIID");
            }
            set
            {
                _settingService.SetSetting<string>("Mobile.SMS.Clickatell.APIID", value);
            }
        }

        /// <summary>
        /// Gets or sets the Clickatell username
        /// </summary>
        public string ClickatellUsername
        {
            get
            {
                return _settingService.GetSettingByKey<string>("Mobile.SMS.Clickatell.Username");
            }
            set
            {
                _settingService.SetSetting<string>("Mobile.SMS.Clickatell.Username", value);
            }
        }

        /// <summary>
        /// Gets or sets the Clickatell password
        /// </summary>
        public string ClickatellPassword
        {
            get
            {
                return _settingService.GetSettingByKey<string>("Mobile.SMS.Clickatell.Password");
            }
            set
            {
                _settingService.SetSetting<string>("Mobile.SMS.Clickatell.Password", value);
            }
        }
    }
}
