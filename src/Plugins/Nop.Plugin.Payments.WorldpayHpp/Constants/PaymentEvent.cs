using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.WorldpayHpp.Constants;
public static class PaymentEvent
{
    public const string SentForAuthorization = "sentForAuthorization";

    public const string Authorized = "authorized";

    public const string SentForSettlement = "sentForSettlement";

    public const string Cancelled = "cancelled";

    public const string Error = "error";
        
    public const string Expired = "expired";

    public const string Refused = "refused";

    public const string SentForRefund = "sentForRefund";

    public const string RefundFailed = "refundFailed";

}
