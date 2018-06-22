
namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Transaction duplicate check type enumeration. Indicates how checks for duplicate transactions should behave.
    /// </summary>
    public enum TransactionDuplicateCheckType
    {
        /// <summary>
        /// No duplicate check 
        /// </summary>
        NoCheck = 0,

        /// <summary>
        /// Exception code is returned in case of duplicate
        /// </summary>
        ExceptionCode = 1,

        /// <summary>
        /// Previously existing transaction is returned in case of duplicate
        /// </summary>
        UsePreviousTransaction = 2,

        /// <summary>
        /// Check is performed as above but without using order ID, and exception code is returned in case of duplicate
        /// </summary>
        UsePreviousTransactionAndExceptionCode = 3
    }
}