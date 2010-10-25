using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Payment.Methods.PayPoint
{
    public class HostedPaymentSettings
    {
        #region Properties
        public static string GatewayUrl
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayPoint.HostedPayment.GatewayUrl", "https://www.secpay.com/java-bin/ValCard");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.PayPoint.HostedPayment.GatewayUrl", value);
            }
        }

        public static string MerchantId
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayPoint.HostedPayment.MerchantID");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.PayPoint.HostedPayment.MerchantID", value);
            }
        }

        public static string RemotePassword
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayPoint.HostedPayment.RemotePassword");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.PayPoint.HostedPayment.RemotePassword", value);
            }
        }

        public static string DigestKey
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayPoint.HostedPayment.DigestKey");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.PayPoint.HostedPayment.DigestKey", value);
            }
        }

        public static decimal AdditionalFee
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.PayPoint.HostedPayment.AdditionalFee");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParamNative("PaymentMethod.PayPoint.HostedPayment.AdditionalFee", value);
            }
        }
        #endregion
    }
}
