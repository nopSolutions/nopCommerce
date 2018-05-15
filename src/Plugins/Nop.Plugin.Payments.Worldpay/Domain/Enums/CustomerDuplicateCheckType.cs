
namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Customer duplicate check type enumeration. Indicates how should behave if the customer ID already exists. 
    /// </summary>
    public enum CustomerDuplicateCheckType
    {
        /// <summary>
        /// If Customer ID exists then return an error.
        /// </summary>
        Error = 0,

        /// <summary>
        ///  If customer ID exists then do not add account but continue with transaction.
        /// </summary>
        Ignore = 1
    }
}