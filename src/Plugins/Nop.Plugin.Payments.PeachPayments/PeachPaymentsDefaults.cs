using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.PeachPayments
{
    public class PeachPaymentsDefaults
    {
        public static string ConfigurationRouteName => "Plugin.Payments.PeachPayments.Configure";
        public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";
        public static string ResultRouteName => "Plugin.Payments.PeachPayments.Result";
        public static string CallbackRouteName => "Plugin.Payments.PeachPayments.Callback";


        public const string PAYMENT_INFO_VIEW_COMPONENT_NAME = "PaymentInfo";
        public const string SCRIPT_VIEW_COMPONENT_NAME = "PeachPaymentsScript";

        public const string BUTTONS_VIEW_COMPONENT_NAME = "PeachPaymentsButtons";
        public const string LOGO_VIEW_COMPONENT_NAME = "PeachPaymentsLogo";
        public static int RequestTimeout => 10;

        public static string SystemName => "Payments.PeachPayments";
        public static string ShoppingCartRouteName => "ShoppingCart";

    }

}
