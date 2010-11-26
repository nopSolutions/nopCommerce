using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Payment.Methods.PayPoint
{
    public class HostedPaymentSettings
    {
        #region Properties
        public static string GatewayUrl
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayPoint.HostedPayment.GatewayUrl", "https://www.secpay.com/java-bin/ValCard");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.PayPoint.HostedPayment.GatewayUrl", value);
            }
        }

        public static string MerchantId
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayPoint.HostedPayment.MerchantID");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.PayPoint.HostedPayment.MerchantID", value);
            }
        }

        public static string RemotePassword
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayPoint.HostedPayment.RemotePassword");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.PayPoint.HostedPayment.RemotePassword", value);
            }
        }

        public static string DigestKey
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PayPoint.HostedPayment.DigestKey");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.PayPoint.HostedPayment.DigestKey", value);
            }
        }

        public static decimal AdditionalFee
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.PayPoint.HostedPayment.AdditionalFee");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParamNative("PaymentMethod.PayPoint.HostedPayment.AdditionalFee", value);
            }
        }
        #endregion
    }
}
