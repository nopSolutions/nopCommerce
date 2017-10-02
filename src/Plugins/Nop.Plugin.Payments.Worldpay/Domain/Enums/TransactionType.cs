using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Transaction type enumeration. Indicates a type of transaction.
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Authorize
        /// </summary>
        [EnumMember(Value = "AUTH_ONLY")]
        Authorize,

        /// <summary>
        /// Partial authorize
        /// </summary>
        [EnumMember(Value = "PARTIAL_AUTH_ONLY")]
        PartialAuthorize,

        /// <summary>
        /// Authorize and capture
        /// </summary>
        [EnumMember(Value = "AUTH_CAPTURE")]
        AuthorizeAndCapture,

        /// <summary>
        /// Partial authorize and capture
        /// </summary>
        [EnumMember(Value = "PARTIAL_AUTH_CAPTURE")]
        PartialAuthorizeAndCapture,

        /// <summary>
        /// Prior authorize and capture
        /// </summary>
        [EnumMember(Value = "PRIOR_AUTH_CAPTURE")]
        PriorAuthorizeAndCapture,

        /// <summary>
        /// Update transaction info
        /// </summary>
        [EnumMember(Value = "UPDATE_TRANS_INFO")]
        UpdateTransactionInfo,

        /// <summary>
        /// Capture
        /// </summary>
        [EnumMember(Value = "CAPTURE_ONLY")]
        Capture,

        /// <summary>
        /// Void
        /// </summary>
        [EnumMember(Value = "VOID")]
        Void,

        /// <summary>
        /// Partial void
        /// </summary>
        [EnumMember(Value = "PARTIAL_VOID")]
        PartialVoid,

        /// <summary>
        /// Credit
        /// </summary>
        [EnumMember(Value = "CREDIT")]
        Credit,

        /// <summary>
        /// Authorize credit
        /// </summary>
        [EnumMember(Value = "CREDIT_AUTHONLY")]
        CreditAuthorize,

        /// <summary>
        /// Prior authorize and capture credit
        /// </summary>
        [EnumMember(Value = "CREDIT_PRIORAUTHCAPTURE")]
        CreditPriorAuthorizeAndCapture,

        /// <summary>
        /// Force credit
        /// </summary>
        [EnumMember(Value = "FORCE_CREDIT")]
        ForceCredit,

        /// <summary>
        /// Force authorize credit
        /// </summary>
        [EnumMember(Value = "FORCE_CREDIT_AUTHONLY")]
        ForceCreditAuthorize,

        /// <summary>
        /// Force prior authorize and capture credit
        /// </summary>
        [EnumMember(Value = "FORCE_CREDIT_PRIORAUTHCAPTURE")]
        ForceCreditPriorAuthorizeAndCapture,

        /// <summary>
        /// Verify
        /// </summary>
        [EnumMember(Value = "VERIFY")]
        Verify,

        /// <summary>
        /// Account verification
        /// </summary>
        [EnumMember(Value = "ACCOUNT_VERIFICATION")]
        AccountVerification,

        /// <summary>
        /// Authentication increment
        /// </summary>
        [EnumMember(Value = "AUTH_INCREMENT")]
        AuthenticationIncrement,

        /// <summary>
        /// Issue
        /// </summary>
        [EnumMember(Value = "ISSUE")]
        Issue,

        /// <summary>
        /// Activate
        /// </summary>
        [EnumMember(Value = "ACTIVATE")]
        Activate,

        /// <summary>
        /// Redeem
        /// </summary>
        [EnumMember(Value = "REDEEM")]
        Redeem,

        /// <summary>
        /// Partial redeem
        /// </summary>
        [EnumMember(Value = "REDEEM_PARTIAL")]
        PartialRedeem,

        /// <summary>
        /// Deactivate
        /// </summary>
        [EnumMember(Value = "DEACTIVATE")]
        Deactivate,

        /// <summary>
        /// Reactivate
        /// </summary>
        [EnumMember(Value = "REACTIVATE")]
        Reactivate,

        /// <summary>
        /// Inquiry balance
        /// </summary>
        [EnumMember(Value = "INQUIRY_BALANCE")]
        InquiryBalance,

        /// <summary>
        /// Recharge
        /// </summary>
        [EnumMember(Value = "RECHARGE")]
        Recharge,

        /// <summary>
        /// Issue virtual
        /// </summary>
        [EnumMember(Value = "ISSUE_VIRTUAL")]
        IssueVirtual,

        /// <summary>
        /// CashOut
        /// </summary>
        [EnumMember(Value = "CASH_OUT")]
        CashOut
    }
}