using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;

namespace NopSolutions.NopCommerce.Payment.Methods.PayPoint
{
    public class HostedPaymentSettings
    {
        #region Properties
        public static string GatewayUrl
        {
            get
            {
                return SettingManager.GetSettingValue("PaymentMethod.PayPoint.HostedPayment.GatewayUrl", "https://www.secpay.com/java-bin/ValCard");
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.PayPoint.HostedPayment.GatewayUrl", value);
            }
        }

        public static string MerchantId
        {
            get
            {
                return SettingManager.GetSettingValue("PaymentMethod.PayPoint.HostedPayment.MerchantID");
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.PayPoint.HostedPayment.MerchantID", value);
            }
        }

        public static string RemotePassword
        {
            get
            {
                return SettingManager.GetSettingValue("PaymentMethod.PayPoint.HostedPayment.RemotePassword");
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.PayPoint.HostedPayment.RemotePassword", value);
            }
        }

        public static string DigestKey
        {
            get
            {
                return SettingManager.GetSettingValue("PaymentMethod.PayPoint.HostedPayment.DigestKey");
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.PayPoint.HostedPayment.DigestKey", value);
            }
        }

        public static decimal AdditionalFee
        {
            get
            {
                return SettingManager.GetSettingValueDecimalNative("PaymentMethod.PayPoint.HostedPayment.AdditionalFee");
            }
            set
            {
                SettingManager.SetParamNative("PaymentMethod.PayPoint.HostedPayment.AdditionalFee", value);
            }
        }
        #endregion
    }
}
