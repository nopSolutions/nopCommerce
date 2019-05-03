
namespace Nop.Plugin.Payments.Qualpay.Domain
{
    /// <summary>
    /// Represents transaction types enumeration
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// An authorization request is used to send cardholder data to the issuing bank for approval.
        /// An approved transaction will continue to be open until it expires or a capture message is received.
        /// </summary>
        Authorization,

        /// <summary>
        /// A sale request is used to perform the function of an authorization and a capture in a single message.
        /// This message is used in retail and card not present environments where no physical goods are being shipped.
        /// </summary>
        Sale
    }
}