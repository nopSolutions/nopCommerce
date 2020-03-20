using System.Runtime.Serialization;
using PayPalCheckoutSdk.Orders;

namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Domain
{
    /// <summary>
    /// Represents an extended authorization object. Some important properties were missed in the original object.
    /// </summary>
    [DataContract]
    public class ExtendedAuthorization : Authorization
    {
        /// <summary>
        /// The API caller-provided external ID. Used to reconcile client transactions with PayPal transactions. Appears in transaction and settlement reports but is not visible to the payer.
        /// </summary>
        [DataMember(Name = "custom_id", EmitDefaultValue = false)]
        public string CustomId { get; set; }
    }
}