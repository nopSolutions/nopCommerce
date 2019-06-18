using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Models
{
    public class PaymentStatuses
    {
        /// <summary>
        /// The user has not yet completed the payment process
        /// </summary>
        public const string PENDING = "pending";

        /// <summary>
        /// The payment has been approved and accredited
        /// </summary>
        public const string APPROVED = "approved";

        /// <summary>
        /// The payment has been authorized but not captured yet
        /// </summary>
        public const string AUTHORIZED = "authorized";

        /// <summary>
        /// Payment is being reviewed
        /// </summary>
        public const string IN_PROCESS = "in_process";

        /// <summary>
        /// Users have initiated a dispute
        /// </summary>
        public const string IN_MEDIATION = "in_mediation";

        /// <summary>
        /// Payment was rejected. The user may retry payment
        /// </summary>
        public const string REJECTED = "rejected";

        /// <summary>
        /// Payment was cancelled by one of the parties or because time for payment has expired
        /// </summary>
        public const string CANCELLED = "cancelled";

        /// <summary>
        /// Payment was refunded to the user
        /// </summary>
        public const string REFUNDED = "refunded";

        /// <summary>
        /// Was made a chargeback in the buyer’s credit card
        /// </summary>
        public const string CHARGED_BACK = "charged_back";
    }
}
